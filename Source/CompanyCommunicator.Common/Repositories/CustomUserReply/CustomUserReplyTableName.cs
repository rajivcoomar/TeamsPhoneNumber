// <copyright file="CustomUserReplyTableName.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories
{
    /// <summary>
    /// App config table information.
    /// </summary>
    public class CustomUserReplyTableName
    {
        /// <summary>
        /// Table name for app config..
        /// </summary>
        public static readonly string TableName = "CustomUserReply";

        /// <summary>
        /// User Teams partition.
        /// </summary>
        public static readonly string UserTeamsReplyPartition = "User Teams Reply";

        /// <summary>
        /// User SMS partition.
        /// </summary>
        public static readonly string UserSMSReplyPartition = "User SMS Reply";
    }
}
