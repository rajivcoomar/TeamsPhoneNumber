// <copyright file="CustomUserReplyEntity.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories
{
    using Microsoft.Azure.Cosmos.Table;

    /// <summary>
    /// App configuration entity.
    /// </summary>
    public class CustomUserReplyEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets the conversation id for the recipient.
        /// </summary>
        public string ConversationId { get; set; }

        /// <summary>
        /// Gets or sets the user id for the recipient.
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// Gets or sets the user message for the recipient.
        /// </summary>
        public string UserMessage { get; set; }
    }
}
