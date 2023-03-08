// <copyright file="AppConfigTableName.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories
{
    /// <summary>
    /// App config table information.
    /// </summary>
    public class CustomAppConfigTableName
    {
        /// <summary>
        /// Table name for app config..
        /// </summary>
        public static readonly string TableName = "CustomAppConfig";

        /// <summary>
        /// App settings partition.
        /// </summary>
        public static readonly string SettingsPartition = "Setting";

        /// <summary>
        /// Initial Teams Message EN row key.
        /// </summary>
        public static readonly string InitialTeamsMessageENRowKey = "InitialTeamsMessage_EN";

        /// <summary>
        /// Initial Teams Message ES row key.
        /// </summary>
        public static readonly string InitialTeamsMessageESRowKey = "InitialTeamsMessage_ES";

        /// <summary>
        /// Language Change row key.
        /// </summary>
        public static readonly string LanguageChangeENRowKey = "LanguageChange";

        /// <summary>
        /// Language Change row key.
        /// </summary>
        public static readonly string LanguageChangeESRowKey = "LanguageChange_ES";

        /// <summary>
        /// Stop Message row key.
        /// </summary>
        public static readonly string StopMessageENRowKey = "StopMessage";

        /// <summary>
        /// Stop Message row key.
        /// </summary>
        public static readonly string StopMessageESRowKey = "StopMessage_ES";

        /// <summary>
        /// Thanks Message row key.
        /// </summary>
        public static readonly string ThanksMessageENRowKey = "ThanksMessage";

        /// <summary>
        /// Thanks Message row key.
        /// </summary>
        public static readonly string ThanksMessageESRowKey = "ThanksMessage_ES";
    }
}
