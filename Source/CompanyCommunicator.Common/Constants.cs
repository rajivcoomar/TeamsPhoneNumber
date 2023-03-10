// <copyright file="Constants.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common
{
    /// <summary>
    /// Constants.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// get the groupMember read all scope.
        /// </summary>
        public const string ScopeGroupMemberReadAll = "GroupMember.Read.All";

        /// <summary>
        /// AppCatalog Read All scope.
        /// </summary>
        public const string ScopeAppCatalogReadAll = "AppCatalog.Read.All";

        /// <summary>
        /// get the user read scope.
        /// </summary>
        public const string ScopeUserRead = "User.Read";

        /// <summary>
        /// scope claim type.
        /// </summary>
        public const string ClaimTypeScp = "scp";

        /// <summary>
        /// authorization scheme.
        /// </summary>
        public const string BearerAuthorizationScheme = "Bearer";

        /// <summary>
        /// claim type user id.
        /// </summary>
        public const string ClaimTypeUserId = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        /// <summary>
        /// claim type tenant id.
        /// </summary>
        public const string ClaimTypeTenantId = "http://schemas.microsoft.com/identity/claims/tenantid";

        /// <summary>
        /// blob container name.
        /// </summary>
        public const string BlobContainerName = "exportdatablobs";

        /// <summary>
        /// get the group type Hidden Membership.
        /// </summary>
        public const string HiddenMembership = "HiddenMembership";

        /// <summary>
        /// get the header key for graph permission type.
        /// </summary>
        public const string PermissionTypeKey = "x-api-permission";

        /// <summary>
        /// get the default graph scope.
        /// </summary>
        public const string ScopeDefault = "https://graph.microsoft.com/.default";

        /// <summary>
        /// get the OData next page link.
        /// </summary>
        public const string ODataNextPageLink = "@odata.nextLink";

        /// <summary>
        /// get the maximum number of recipients in a batch.
        /// </summary>
        public const int MaximumNumberOfRecipientsInBatch = 1000;

        /// <summary>
        /// get the Microsoft Graph api batch request size.
        /// https://docs.microsoft.com/en-us/graph/known-issues#limit-on-batch-size.
        /// </summary>
        public const int MaximumGraphAPIBatchSize = 15;

        /// <summary>
        /// prefix for data uri image.
        /// </summary>
        public const string ImageBase64Format = "data:image/";

        /// <summary>
        /// cache duration in hours.
        /// </summary>
        public const int CacheDurationInHours = 6;

        /// <summary>
        /// All Key words in the app.
        /// </summary>
        public static string[] AllKeywords = new string[] { "español", "española", "spanish", "inglés", "inglesa", "english", "help", "ayuda", "stop", "detener" };

        /// <summary>
        /// English Key words.
        /// </summary>
        public static string[] EnglishKeywords = new string[] { "english", "inglés", "inglesa" };

        /// <summary>
        /// Spanish Key words.
        /// </summary>
        public static string[] SpanishKeywords = new string[] { "español", "española", "spanish" };

        /// <summary>
        /// Help Key words.
        /// </summary>
        public static string[] HelpKeywords = new string[] { "help", "ayuda" };

        /// <summary>
        /// Stop Key words.
        /// </summary>
        public static string[] StopKeywords = new string[] { "stop", "detener" };

        /// <summary>
        /// All Language Key words.
        /// </summary>
        public static string[] AllLanguageKeywords = new string[] { "español", "española", "inglés", "inglesa", "english", "spanish" };
    }
}
