// <copyright file="ILanguageTranslateService.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Services
{
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// App settings interface.
    /// </summary>
    public interface ILanguageTranslateService
    {
        /// <summary>
        /// Gets custom reply message.
        /// </summary>
        /// <param name="Region"></param>
        /// <param name="SubscriptionKey"></param>
        /// <param name="Endpoint"></param>
        /// <param name="inputText">input Text.</param>
        /// <returns>Messages.</returns>
        public Task<string> TranslateTextRequest(string Region, string SubscriptionKey, string Endpoint, NotificationDataEntity notificationData);
    }
}
