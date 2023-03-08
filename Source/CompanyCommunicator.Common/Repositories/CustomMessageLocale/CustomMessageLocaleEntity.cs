// <copyright file="AppConfigEntity.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories
{
    using Microsoft.Azure.Cosmos.Table;
    using System;

    /// <summary>
    /// App configuration entity.
    /// </summary>
    public class CustomMessageLocaleEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets the id of the notification.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the id of the notification.
        /// </summary>
        public string NotificationId { get; set; }

        /// <summary>
        /// Gets or sets the title text of the notification's content.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the image link of the notification's content.
        /// </summary>
        public string ImageLink { get; set; }

        /// <summary>
        /// Gets or sets the blob name for image in base64 format.
        /// </summary>
        public string ImageBase64BlobName { get; set; }

        /// <summary>
        /// Gets or sets the summary text of the notification's content.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the author text of the notification's content.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the button title of the notification's content.
        /// </summary>
        public string ButtonTitle { get; set; }

        /// <summary>
        /// Gets or sets the button link of the notification's content.
        /// </summary>
        public string ButtonLink { get; set; }

        /// <summary>
        /// Gets or sets the information for the user that created the notification.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the DateTime the notification was created.
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}
