// <copyright file="SentNotification.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Sent notification model class.
    /// </summary>
    public class SentNotification : BaseNotification
    {
        /// <summary>
        /// Gets or sets the sending started date time.
        /// </summary>
        public DateTime? SendingStartedDate { get; set; }

        /// <summary>
        /// Gets or sets the Sent DateTime value.
        /// </summary>
        public DateTime? SentDate { get; set; }

        /// <summary>
        /// Gets or sets the number of recipients who have received the notification successfully.
        /// </summary>
        public int Succeeded { get; set; }

        /// <summary>
        /// Gets or sets the number of recipients who failed in receiving the notification.
        /// </summary>
        public int Failed { get; set; }

        /// <summary>
        /// Gets or sets the number of recipients whose delivery status is unknown.
        /// </summary>
        public int? Unknown { get; set; }

        /// <summary>
        /// Gets or sets the number of recipients whose delivery status is canceled.
        /// </summary>
        public int? Canceled { get; set; }

        /// <summary>
        /// Gets or sets Teams audience name collection.
        /// </summary>
        public IEnumerable<string> TeamNames { get; set; }

        /// <summary>
        /// Gets or sets Rosters audience name collection.
        /// </summary>
        public IEnumerable<string> RosterNames { get; set; }

        /// <summary>
        /// Gets or sets Groups audience name collection.
        /// </summary>
        public IEnumerable<string> GroupNames { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a notification was sent to all users.
        /// </summary>
        public bool AllUsers { get; set; }

        /// <summary>
        /// Gets or sets error message.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets warning message.
        /// </summary>
        public string WarningMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether user can download notification.
        /// </summary>
        public bool CanDownload { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether sending is in progress.
        /// </summary>
        public bool SendingCompleted { get; set; }

        /// <summary>
        /// Gets or sets a value of created by.
        /// </summary>
        public string CreatedBy { get; set; }

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
        /// Gets or sets the number of not found recipients.
        /// </summary>
        public int? SMSRecipientNotFound { get; set; }

        /// <summary>
        /// Gets or sets the number or recipients who have an unknown status - this means a status
        /// that has not changed from the initial initialization status after the notification has
        /// been force completed.
        /// </summary>
        public int? SMSUnknown { get; set; }

        /// <summary>
        /// Gets or sets the number or recipients who have canceled status.
        /// </summary>
        public int? SMSCanceled { get; set; }
    }
}
