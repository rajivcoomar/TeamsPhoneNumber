// <copyright file="BotOptions.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Services
{
    /// <summary>
    /// Options used for holding meta data for the bot.
    /// </summary>
    public class LanguageTranslateOptions
    {
        /// <summary>
        /// Gets or sets the User app ID for the user bot.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the User app password for the user bot.
        /// </summary>
        public string SubscriptionKey { get; set; }

        /// <summary>
        /// Gets or sets the Author app ID for the author bot.
        /// </summary>
        public string Endpoint { get; set; }

    }
}
