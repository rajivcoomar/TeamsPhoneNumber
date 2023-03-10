// <copyright file="ILanguageTranslateService.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Services
{
    using System.Threading.Tasks;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData;

    /// <summary>
    /// App settings interface.
    /// </summary>
    public interface ILanguageTranslateService
    {
        /// <summary>
        /// Gets custom reply message.
        /// </summary>
        /// <param name="Region">Region.</param>
        /// <param name="SubscriptionKey">SubscriptionKey.</param>
        /// <param name="Endpoint">Endpoint.</param>
        /// <param name="notificationData">notification Data.</param>
        /// <returns>Messages.</returns>
        public Task<string> TranslateTextRequest(string Region, string SubscriptionKey, string Endpoint, NotificationDataEntity notificationData);

        /// <summary>
        /// Update New Notification ID.
        /// </summary>
        /// <param name="oldNotificationID">old NotificationID.</param>
        /// <param name="newNotificationID">new NotificationID.</param>
        /// <returns>updates id.</returns>
        public Task UpdateNewNotificationID(string oldNotificationID, string newNotificationID);
    }
}
