// <copyright file="IGroupsService.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.MicrosoftGraph
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Graph;

    /// <summary>
    /// Interface for Groups Service.
    /// </summary>
    public interface IGroupsService
    {
        /// <summary>
        /// get the group by ids.
        /// </summary>
        /// <param name="groupIds">list of group ids.</param>
        /// <returns>list of groups.</returns>
        IAsyncEnumerable<Group> GetByIdsAsync(IEnumerable<string> groupIds);

        /// <summary>
        /// check if list has hidden membership group.
        /// </summary>
        /// <param name="groupIds">list of group ids.</param>
        /// <returns>boolean.</returns>
        Task<bool> ContainsHiddenMembershipAsync(IEnumerable<string> groupIds);

        /// <summary>
        /// Search groups based on query.
        /// </summary>
        /// <param name="query">query param.</param>
        /// <param name="userId"> user id.</param>
        /// <returns>list of group.</returns>
        Task<IList<Group>> SearchAsync(string query, string userId);

        /// <summary>
        /// Get users group.
        /// </summary>
        /// <param name="userId">User's AAD id.</param>
        /// <param name="log">log.</param>
        /// <returns>returns list of groups.</returns>
        Task<List<string>> GetUserAUGroups(string userId, ILogger log);
    }
}
