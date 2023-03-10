// <copyright file="StoreMessageActivity.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func.PreparingToSend
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.AdaptiveCard;

    /// <summary>
    /// Stores the message in sending notification data table.
    /// </summary>
    public class StoreMessageActivity
    {
        private static readonly string CachePrefixImage = "image_";
        private readonly ISendingNotificationDataRepository sendingNotificationDataRepository;
        private readonly ICustomMessageLocaleRepository customMessageLocaleRepository;
        private readonly AdaptiveCardCreator adaptiveCardCreator;
        private readonly IMemoryCache memoryCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreMessageActivity"/> class.
        /// </summary>
        /// <param name="notificationRepo">Sending notification data repository.</param>
        /// <param name="customRepo">Custom Message Locate data repository.</param>
        /// <param name="cardCreator">The adaptive card creator.</param>
        /// <param name="memoryCache">The memory cache.</param>
        public StoreMessageActivity(
            ISendingNotificationDataRepository notificationRepo,
            ICustomMessageLocaleRepository customRepo,
            AdaptiveCardCreator cardCreator,
            IMemoryCache memoryCache)
        {
            this.sendingNotificationDataRepository = notificationRepo ?? throw new ArgumentNullException(nameof(notificationRepo));
            this.customMessageLocaleRepository = customRepo ?? throw new ArgumentNullException(nameof(customRepo));
            this.adaptiveCardCreator = cardCreator ?? throw new ArgumentNullException(nameof(cardCreator));
            this.memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        /// <summary>
        /// Stores the message in sending notification data table.
        /// </summary>
        /// <param name="notification">A notification to be sent to recipients.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        /// <param name="log">Logging service.</param>
        [FunctionName(FunctionNames.StoreMessageActivity)]
        public async Task RunAsync(
            [ActivityTrigger] NotificationDataEntity notification,
            ILogger log)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            // In case we have blob name instead of URL to public image.
            if (!string.IsNullOrEmpty(notification.ImageBase64BlobName)
                && notification.ImageLink.StartsWith(Constants.ImageBase64Format))
            {
                var cacheKeySentImage = CachePrefixImage + notification.Id;
                bool isCacheEntryExists = this.memoryCache.TryGetValue(cacheKeySentImage, out string imageLink);

                if (!isCacheEntryExists)
                {
                    imageLink = await this.sendingNotificationDataRepository.GetImageAsync(notification.ImageBase64BlobName);
                    this.memoryCache.Set(cacheKeySentImage, imageLink, TimeSpan.FromHours(Constants.CacheDurationInHours));

                    log.LogInformation($"Successfully cached the image." +
                                    $"\nNotificationId Id: {notification.Id}");
                }

                notification.ImageLink += imageLink;
            }

            // create Adaptive Card for English language
            var contentEN = notification.Id + NotificationDataTableNames.EnglishLanguageRowKey;
            var serializedContent = this.adaptiveCardCreator.CreateAdaptiveCard(notification).ToJson();

            // Save Adaptive Card with data uri into blob storage. Blob name = notification.Id + English lang.
            await this.sendingNotificationDataRepository.SaveAdaptiveCardAsync(contentEN, serializedContent);

            // create Adaptive Card for Spanish language
            var customMessageLocaleData = await this.customMessageLocaleRepository.GetAsync(
                partitionKey: CustomMessageLocaleTableName.SpanishLangPartition,
                rowKey: notification.Id);

            var notificationES = notification;
            if (customMessageLocaleData != null)
            {
                notificationES.Title = customMessageLocaleData.Title;
                notificationES.Summary = customMessageLocaleData.Summary;
                notificationES.ButtonTitle = customMessageLocaleData.ButtonTitle;
                notificationES.Author = customMessageLocaleData.Author;
            }

            var contentES = notification.Id + NotificationDataTableNames.SpanishLanguageRowKey;
            var serializedContentES = this.adaptiveCardCreator.CreateAdaptiveCard(notificationES).ToJson();

            // Save Adaptive Card with data uri into blob storage. Blob name = notification.Id + Spanish lang.
            await this.sendingNotificationDataRepository.SaveAdaptiveCardAsync(contentES, serializedContentES);

            // APPEND LANGUAGE TO DIFFERENTIATE BETWEEN EN AND ES
            var sendingNotification = new SendingNotificationDataEntity
            {
                PartitionKey = NotificationDataTableNames.SendingNotificationsPartition,
                RowKey = notification.RowKey,
                NotificationId = notification.Id,
                Content = contentEN,
                ContentES = contentES,
            };

            await this.sendingNotificationDataRepository.CreateOrUpdateAsync(sendingNotification);
        }
    }
}
