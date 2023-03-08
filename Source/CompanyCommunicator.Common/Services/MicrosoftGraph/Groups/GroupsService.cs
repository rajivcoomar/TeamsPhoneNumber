// <copyright file="GroupsService.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.MicrosoftGraph
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Graph;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Extensions;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.Teams;
    using static System.Net.Mime.MediaTypeNames;

    /// <summary>
    /// Groups Service.
    /// </summary>
    internal class GroupsService : IGroupsService
    {
        private readonly IGraphServiceClient graphServiceClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupsService"/> class.
        /// </summary>
        /// <param name="graphServiceClient">graph service client.</param>
        internal GroupsService(IGraphServiceClient graphServiceClient)
        {
            this.graphServiceClient = graphServiceClient ?? throw new ArgumentNullException(nameof(graphServiceClient));
        }

        private int MaxResultCount { get; set; } = 25;

        private int MaxRetry { get; set; } = 2;

        /// <summary>
        /// get groups by ids.
        /// </summary>
        /// <param name="groupIds">list of group ids.</param>
        /// <returns>list of groups.</returns>
        public async IAsyncEnumerable<Group> GetByIdsAsync(IEnumerable<string> groupIds)
        {
            foreach (var id in groupIds)
            {
                var group = await this.graphServiceClient
                                .Groups[id]
                                .Request()
                                .WithMaxRetry(this.MaxRetry)
                                .Select(gr => new { gr.Id, gr.Mail, gr.DisplayName, gr.Visibility, })
                                .Header(Common.Constants.PermissionTypeKey, GraphPermissionType.Delegate.ToString())
                                .GetAsync();
                yield return group;
            }
        }

        /// <summary>
        /// check if list has hidden membership group.
        /// </summary>
        /// <param name="groupIds">list of group ids.</param>
        /// <returns>boolean.</returns>
        public async Task<bool> ContainsHiddenMembershipAsync(IEnumerable<string> groupIds)
        {
            var groups = this.GetByIdsAsync(groupIds);
            await foreach (var group in groups)
            {
                if (group.IsHiddenMembership())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Search M365 groups,distribution groups, security groups based on query.
        /// FILTER GROUP NAMES BASED ON LOGGED IN USER.
        /// </summary>
        /// <param name="query">query param.</param>
        /// <param name="userId">logged in user id.</param>
        /// <returns>list of group.</returns>
        public async Task<IList<Group>> SearchAsync(string query, string userId)
        {
            query = Uri.EscapeDataString(query);
            var groupList = await this.SearchM365GroupsAsync(query, this.MaxResultCount);
            groupList.AddRange(await this.AddDistributionGroupAsync(query, this.MaxResultCount - groupList.Count()));
            groupList.AddRange(await this.AddSecurityGroupAsync(query, this.MaxResultCount - groupList.Count()));

            if (groupList.Count > 0)
            {
                groupList = groupList.Where(c => c.DisplayName.ToLower().Contains(query.ToLower())).ToList();
            }
            else
            {
                return groupList;
            }

            if (!string.IsNullOrEmpty(userId))
            {
                var memberAUGroupIDs = await this.GetAUGroups(userId); // "Subramanya@star-knowledge.org"
                if (memberAUGroupIDs.Count > 0)
                {
                    // get/filter groups from administrative units.
                    var userGroupList = groupList.Where(x => memberAUGroupIDs.Any(y => x.Id.Contains(y))).ToList();
                    return userGroupList;
                }
                else
                {
                    // get/filter groups from logged in user
                    var groupIDs = groupList.Select(c => c.Id).ToList();
                    var memberGroup = await this.CheckMemberGroup(groupIDs, userId); // "Subramanya@star-knowledge.org"
                    var memberGroupIDs = memberGroup.Select(c => c).ToList();

                    var userGroupList = groupList.Where(x => memberGroupIDs.Any(y => x.Id.Contains(y))).ToList();
                    return userGroupList;
                }
            }
            else
            {
                return groupList;
            }
        }

        /// <inheritdoc/>
        public async Task<List<string>> GetUserAUGroups(string userId, ILogger log)
        {
            try
            {
                var page = await this.graphServiceClient
                                    .Users[userId]
                                    .MemberOf
                                    .Request()
                                    .GetAsync();

                var names = new List<string>();
                names.AddRange(page
                        .OfType<AdministrativeUnit>()
                        .Select(x => x.DisplayName)
                        .Where(name => !string.IsNullOrEmpty(name)));

                while (page.NextPageRequest != null)
                {
                    page = await page.NextPageRequest.GetAsync();
                    names.AddRange(page
                        .OfType<AdministrativeUnit>()
                        .Select(x => x.DisplayName)
                        .Where(name => !string.IsNullOrEmpty(name)));
                }

                foreach (var name in names)
                {
                    log.LogInformation($"GetUserAUGroups, Group Name: {name}");
                }

                return names;
            }
            catch (Exception ex)
            {
                log.LogError($"GetUserAUGroups, Exception: {ex.Message}");
                return new List<string>();
            }
        }


        /// <summary>
        /// Search M365 groups, distribution groups, security groups based on query and visibilty.
        /// </summary>
        /// <param name="query">query param.</param>
        /// <param name="resultCount">page size.</param>
        /// <param name="includeHiddenMembership">boolean to filter hidden membership.</param>
        /// <returns>list of group.</returns>
        private async Task<List<Group>> SearchM365GroupsAsync(string query, int resultCount, bool includeHiddenMembership = false)
        {
            // string filterQuery = $"groupTypes/any(c:c+eq+'Unified') and mailEnabled eq true and (startsWith(mail,'{query}') or startsWith(displayName,'{query}'))";
            string filterQuery = $"groupTypes/any(c:c+eq+'Unified') and mailEnabled eq true  " + // and Contains(displayName,'{query}')
                $"and (startsWith(mail,'FL') or startsWith(displayName,'FL'))";
            var groupsPaged = await this.SearchAsync(filterQuery, resultCount);
            if (includeHiddenMembership)
            {
                return groupsPaged.CurrentPage.ToList();
            }

            var groupList = groupsPaged.CurrentPage.
                                        Where(group => !group.IsHiddenMembership()).
                                        ToList();
            while (groupsPaged.NextPageRequest != null && groupList.Count() < resultCount)
            {
                groupsPaged = await groupsPaged.NextPageRequest.GetAsync();
                groupList.AddRange(groupsPaged.CurrentPage.
                          Where(group => !group.IsHiddenMembership()));
            }

            return groupList.Take(resultCount).ToList();
        }

        /// <summary>
        /// Search Distribution Groups based on query.
        /// </summary>
        /// <param name="query">query param.</param>
        /// <param name="resultCount">total page size.</param>
        /// <returns>list of distribution group.</returns>
        private async Task<IEnumerable<Group>> AddDistributionGroupAsync(string query, int resultCount)
        {
            if (resultCount == 0)
            {
                return new List<Group>();
            }

            // string filterforDL = $"mailEnabled eq true and (startsWith(mail,'{query}') or startsWith(displayName,'{query}'))";
            string filterforDL = $"mailEnabled eq true and (startsWith(mail,'FL') or startsWith(displayName,'FL'))";
            // string filterforDL = $"mailEnabled eq true and \"displayName:'{query}'\" and (startsWith(mail,'FL') or startsWith(displayName,'FL'))";
            var distributionGroups = await this.SearchAsync(filterforDL, resultCount);

            // Filtering the result only for distribution groups.
            var distributionGroupList = distributionGroups.CurrentPage.
                                                           Where(dg => dg.GroupTypes.IsNullOrEmpty()).ToList();
            while (distributionGroups.NextPageRequest != null && distributionGroupList.Count() < resultCount)
            {
                distributionGroups = await distributionGroups.NextPageRequest.GetAsync();
                distributionGroupList.AddRange(distributionGroups.CurrentPage.Where(dg => dg.GroupTypes.IsNullOrEmpty()));
            }

            return distributionGroupList.Take(resultCount);
        }

        /// <summary>
        /// Search Security Groups based on query.
        /// </summary>
        /// <param name="query">query param.</param>
        /// <param name="resultCount">total page size.</param>
        /// <returns>list of security group.</returns>
        private async Task<IEnumerable<Group>> AddSecurityGroupAsync(string query, int resultCount)
        {
            if (resultCount == 0)
            {
                return new List<Group>();
            }

            //string filterforSG = $"mailEnabled eq false and securityEnabled eq true and startsWith(displayName,'{query}')";
            string filterforSG = $"mailEnabled eq false and securityEnabled eq true and startsWith(displayName,'FL')";
            // string filterforSG = $"mailEnabled eq false and securityEnabled eq true and \"displayName:'{query}'\" and startsWith(displayName,'FL')";
            var sgGroups = await this.SearchAsync(filterforSG, resultCount);
            return sgGroups.CurrentPage.Take(resultCount);
        }

        /// <summary>
        /// Search M365 groups,sistribution groups, security groups based on query.
        /// </summary>
        /// <param name="filterQuery">query param.</param>
        /// <param name="resultCount">page size.</param>
        /// <returns>graph group collection.</returns>
        private async Task<IGraphServiceGroupsCollectionPage> SearchAsync(string filterQuery, int resultCount)
        {
            var grpData = await this.graphServiceClient
                                   .Groups
                                   .Request()
                                   .WithMaxRetry(this.MaxRetry)
                                   .Filter(filterQuery)
                                   .Select(group => new
                                   {
                                       group.Id,
                                       group.Mail,
                                       group.DisplayName,
                                       group.Visibility,
                                       group.GroupTypes,
                                   }).
                                   Top(resultCount)
                                   .Header(Common.Constants.PermissionTypeKey, GraphPermissionType.Delegate.ToString())
                                   .GetAsync();
            return grpData;
        }

        /// <summary>
        /// Check member group of an user.
        /// </summary>
        /// <param name="groupIds">group IDs.</param>
        /// <param name="userId">user Id.</param>
        /// <returns>member related group list.</returns>
        private async Task<IDirectoryObjectCheckMemberGroupsCollectionPage> CheckMemberGroup(List<string> groupIds, string userId)
        {
            return await this.graphServiceClient
                .Users[userId]
                .CheckMemberGroups(groupIds)
                .Request()
                .PostAsync();
        }

        /// <summary>
        /// Get Administrative Unit Groups of user.
        /// </summary>
        /// <param name="userId">user Id.</param>
        /// <returns>member related group list.</returns>
        private async Task<List<string>> GetAUGroups(string userId)
        {
            try
            {
                // to get all administrative units
                var auData = await this.graphServiceClient.Directory.AdministrativeUnits.Request().Filter("startsWith(displayName,'FLA_')").GetAsync();
                var auDataIDs = auData.Select(c => c.Id).ToList();

                // to get each administrative units members
                var auGrpIDs = new List<string>();
                foreach (var id in auDataIDs)
                {
                    var result = await this.graphServiceClient.Directory.AdministrativeUnits[id].Members.Request().GetAsync(); // [userId]
                    if (result != null && result.Count > 0)
                    {
                        // #microsoft.graph.group
                        // #microsoft.graph.user
                        var usrList = result.ToList().Where(c => c.ODataType == "#microsoft.graph.user").ToList();
                        var isUsrExists = usrList.Select(c => c.Id.Contains(userId)).ToList().Count > 0;
                        if (isUsrExists)
                        {
                            var grpList = result.ToList().Where(c => c.ODataType == "#microsoft.graph.group").ToList();
                            auGrpIDs.AddRange(grpList.Select(c => c.Id).ToList());
                        }
                    }
                }

                return auGrpIDs;
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }
    }
}
