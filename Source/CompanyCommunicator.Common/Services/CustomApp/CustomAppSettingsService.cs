// <copyright file="AppSettingsService.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Services
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories;

    /// <summary>
    /// App settings service implementation.
    /// </summary>
    public class CustomAppSettingsService : ICustomAppSettingsService
    {
        private readonly ICustomAppConfigRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAppSettingsService"/> class.
        /// </summary>
        /// <param name="repository">App configuration repository.</param>
        public CustomAppSettingsService(ICustomAppConfigRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <inheritdoc/>
        public async Task<string> GetMessageAsync(string rowKey)
        {
            // check in-memory cache.
            if (string.IsNullOrWhiteSpace(rowKey))
            {
                rowKey = "";
            }

            var custAppConfig = await this.repository.GetAsync(
                CustomAppConfigTableName.SettingsPartition,
                rowKey);

            return custAppConfig?.Value;
        }

    }
}
