// <copyright file="ICustomUserReplyService.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Services
{
    using System.Threading.Tasks;
    using Microsoft.Bot.Schema;

    /// <summary>
    /// App settings interface.
    /// </summary>
    public interface ICustomUserReplyService
    {
        /// <summary>
        /// Gets custom reply message.
        /// </summary>
        /// <param name="rowKeyValue">Row key to filter message. It is conversation ID + user Id</param>
        /// <returns>Messages</returns>
        public Task<string> GetCustomReplyAsync(string rowKeyValue);

        /// <summary>
        /// Updates custom reply message in database.
        /// </summary>
        /// <param name="activity">Message activity.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task<string> SaveCustomReplyAsync(IMessageActivity activity);

        /// <summary>
        /// Check and Get Initial Message for user.
        /// </summary>
        /// <param name="userId">user Id.</param>
        /// <returns>initial message text</returns>
        public Task<string> GetUpdateInitialMessage(string userId);
    }
}
