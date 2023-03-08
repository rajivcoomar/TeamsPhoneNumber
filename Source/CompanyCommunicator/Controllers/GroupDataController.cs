// <copyright file="GroupDataController.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.CompanyCommunicator.Authentication;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.MicrosoftGraph;
    using Microsoft.Teams.Apps.CompanyCommunicator.Models;

    /// <summary>
    /// Controller for getting groups.
    /// </summary>
    [Route("api/groupData")]
    [Authorize(PolicyNames.MustBeValidUpnPolicy)]
    public class GroupDataController : Controller
    {
        private readonly INotificationDataRepository notificationDataRepository;
        private readonly IGroupsService groupsService;
        private readonly IHttpClientFactory clientFactory;
        private readonly ILogger<GroupDataController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupDataController"/> class.
        /// </summary>
        /// <param name="notificationDataRepository">Notification data repository instance.</param>
        /// <param name="groupsService">Microsoft Graph service instance.</param>
        /// <param name="clientFactory">the http client factory.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public GroupDataController(
            INotificationDataRepository notificationDataRepository,
            IGroupsService groupsService,
            IHttpClientFactory clientFactory,
            ILoggerFactory loggerFactory)
        {
            this.notificationDataRepository = notificationDataRepository ?? throw new ArgumentNullException(nameof(notificationDataRepository));
            this.groupsService = groupsService ?? throw new ArgumentNullException(nameof(groupsService));
            this.clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            this.logger = loggerFactory?.CreateLogger<GroupDataController>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        /// <summary>
        /// check if user has access.
        /// </summary>
        /// <returns>indicating user access to group.</returns>
        [HttpGet("verifyaccess")]
        [Authorize(PolicyNames.MSGraphGroupDataPolicy)]
        public bool VerifyAccess()
        {
            return true;
        }

        /// <summary>
        /// Action method to get groups.
        /// </summary>
        /// <param name="query">user input.</param>
        /// <returns>list of group data.</returns>
        [HttpGet("search/{query}")]
        [Authorize(PolicyNames.MSGraphGroupDataPolicy)]
        public async Task<IEnumerable<GroupData>> SearchAsync(string query)
        {
            int minQueryLength = 3;
            if (string.IsNullOrEmpty(query) || query.Length < minQueryLength)
            {
                return default;
            }

            var loggedInUser = this.HttpContext.User?.Identity?.Name;

            var groups = await this.groupsService.SearchAsync(query, loggedInUser);

            //foreach (var grp in groups)
            //{
            //    bool isUserInGroup = HttpContext.User?.IsInRole(grp.DisplayName) ?? false;
            //}

            return groups.Select(group => new GroupData()
            {
                Id = group.Id,
                Name = group.DisplayName,
                Mail = group.Mail,
            });
        }

        /// <summary>
        /// Get Group Data by Id.
        /// </summary>
        /// <param name="id">Draft notification Id.</param>
        /// <returns>List of Group Names.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<GroupData>>> GetGroupsAsync(string id)
        {
            var notificationEntity = await this.notificationDataRepository.GetAsync(
                NotificationDataTableNames.DraftNotificationsPartition,
                id);
            if (notificationEntity == null)
            {
                return this.NotFound();
            }

            var groups = await this.groupsService.GetByIdsAsync(notificationEntity.Groups)
                .Select(group => new GroupData()
                {
                    Id = group.Id,
                    Name = group.DisplayName,
                    Mail = group.Mail,
                }).ToListAsync();
            return this.Ok(groups);
        }

        /// <summary>
        /// Get Group Data by Id.
        /// </summary>
        /// <returns>List of Group Names.</returns>
        [HttpGet("getFLAGroupAccess")]
        public async Task<ActionResult<bool>> GetFLAGroupAccess()
        {
            var loggedInUser = this.HttpContext.User?.Identity?.Name;

            var groups = await this.groupsService.GetUserAUGroups(loggedInUser, this.logger);

            if (groups.Count > 0)
            {
                return this.Ok(groups.Where(c => c.Contains("FLA")).Count() > 0);
            }

            return this.Ok(false);
        }

        /// <summary>
        /// Get Group Data by Id.
        /// </summary>
        /// <returns>List of Group Names.</returns>
        [HttpGet("getFLAGroups")]
        public async Task<ActionResult<List<string>>> GetFLAGroups()
        {
            var loggedInUser = this.HttpContext.User?.Identity?.Name;

            var groups = await this.groupsService.GetUserAUGroups(loggedInUser, this.logger);

            return this.Ok(groups);
        }
    }
}
