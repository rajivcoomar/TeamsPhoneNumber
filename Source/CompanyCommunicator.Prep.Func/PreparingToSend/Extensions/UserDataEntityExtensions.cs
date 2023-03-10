// <copyright file="UserDataEntityExtensions.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func.PreparingToSend.Extensions
{
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.SentNotificationData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.UserData;

    /// <summary>
    /// Extension methods for the UserDataEntity class.
    /// </summary>
    public static class UserDataEntityExtensions
    {
        /// <summary>
        /// Creates a SentNotificationDataEntity in an initialized state from the given UserDataEntity
        /// and partition key.
        /// Makes sure to set the correct recipient type for having been created from a UserDataEntity.
        /// </summary>
        /// <param name="userDataEntity">The user data entity.</param>
        /// <param name="notification">The notification data entity.</param>
        /// <returns>The sent notification data entity.</returns>
        public static SentNotificationDataEntity CreateInitialSentNotificationDataEntity(
            this UserDataEntity userDataEntity, NotificationDataEntity notification) // string partitionKey)
        {
            return new SentNotificationDataEntity
            {
                PartitionKey = notification.Id,
                RowKey = userDataEntity.AadId,
                RecipientType = SentNotificationDataEntity.UserRecipientType,
                RecipientId = userDataEntity.AadId,
                StatusCode = SentNotificationDataEntity.InitializationStatusCode,
                ConversationId = userDataEntity.ConversationId,
                TenantId = userDataEntity.TenantId,
                UserId = userDataEntity.UserId,
                ServiceUrl = userDataEntity.ServiceUrl,
                UserType = userDataEntity.UserType,
                MobileNumber = userDataEntity.MobileNumber,
                PresenceStatus = userDataEntity.PresenceStatus ?? string.Empty,
                MessageType = notification.MessageType,
                PreferredLanguage = userDataEntity.PreferredLanguage ?? string.Empty,
            };
        }
    }
}
