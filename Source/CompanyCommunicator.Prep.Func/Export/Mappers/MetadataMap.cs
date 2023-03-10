// <copyright file="MetadataMap.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func.Export.Mappers
{
    using System;
    using CsvHelper.Configuration;
    using Microsoft.Extensions.Localization;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Resources;
    using Microsoft.Teams.Apps.CompanyCommunicator.Prep.Func.Export.Model;

    /// <summary>
    /// Mapper class for MetaData.
    /// </summary>
    public sealed class MetadataMap : ClassMap<Metadata>
    {
        private readonly IStringLocalizer<Strings> localizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataMap"/> class.
        /// </summary>
        /// <param name="localizer">Localization service.</param>
        public MetadataMap(IStringLocalizer<Strings> localizer)
        {
            this.localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            this.Map(x => x.MessageTitle).Name(this.localizer.GetString("ColumnName_MessageTitle"));
            this.Map(x => x.MessageSummary).Name(this.localizer.GetString("ColumnName_MessageSummary"));
            this.Map(x => x.MessageAuthor).Name(this.localizer.GetString("ColumnName_MessageAuthor"));
            this.Map(x => x.MessageButtonLink).Name(this.localizer.GetString("ColumnName_MessageButtonLink"));
            this.Map(x => x.MessageButtonTitle).Name(this.localizer.GetString("ColumnName_MessageButtonTitle"));
            this.Map(x => x.MessageType).Name(this.localizer.GetString("ColumnName_MessageType"));
            this.Map(x => x.Succeeded).Name(this.localizer.GetString("ColumnName_Succeeded"));
            this.Map(x => x.Failed).Name(this.localizer.GetString("ColumnName_Failed"));
            this.Map(x => x.SMSSucceeded).Name(this.localizer.GetString("ColumnName_SMSSucceeded"));
            this.Map(x => x.SMSFailed).Name(this.localizer.GetString("ColumnName_SMSFailed"));
            this.Map(x => x.SentTimeStamp).Name(this.localizer.GetString("ColumnName_SentTimeStamp"));
            this.Map(x => x.ExportTimeStamp).Name(this.localizer.GetString("ColumnName_ExportTimeStamp"));
            this.Map(x => x.ExportedBy).Name(this.localizer.GetString("ColumnName_ExportedBy"));
        }
    }
}
