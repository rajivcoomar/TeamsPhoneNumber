// <copyright file="Metadata.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func.Export.Model
{
    using System;

    /// <summary>
    /// Metadata model class.
    /// </summary>
    public class Metadata
    {
        /// <summary>
        /// Gets or sets the message title.
        /// </summary>
        public string NotificationId { get; set; }

        /// <summary>
        /// Gets or sets the message title.
        /// </summary>
        public string MessageTitle { get; set; }

        /// <summary>
        /// Gets or sets the message title.
        /// </summary>
        public string MessageSummary { get; set; }

        /// <summary>
        /// Gets or sets the message title.
        /// </summary>
        public string MessageAuthor { get; set; }

        /// <summary>
        /// Gets or sets the button title of the notification's content.
        /// </summary>
        public string MessageButtonTitle { get; set; }

        /// <summary>
        /// Gets or sets the button link of the notification's content.
        /// </summary>
        public string MessageButtonLink { get; set; }

        /// <summary>
        /// Gets or sets the sent timestamp.
        /// </summary>
        public DateTime? SentTimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the export timestamp.
        /// </summary>
        public DateTime? ExportTimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the exported by user id.
        /// </summary>
        public string ExportedBy { get; set; }

        /// <summary>
        /// Gets or sets Message Type.
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// Gets or sets the number of recipients who have received the notification successfully.
        /// </summary>
        public int SMSSucceeded { get; set; }

        /// <summary>
        /// Gets or sets the number of recipients who failed to receive the notification because
        /// of a failure response in the API call.
        /// </summary>
        public int SMSFailed { get; set; }

        /// <summary>
        /// Gets or sets the number of recipients who have received the notification successfully.
        /// </summary>
        public int Succeeded { get; set; }

        /// <summary>
        /// Gets or sets the number of recipients who failed to receive the notification because
        /// of a failure response in the API call.
        /// </summary>
        public int Failed { get; set; }
    }
}