﻿// <copyright file="AppConfigRepository.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories;

    /// <summary>
    /// App configuration repository.
    /// </summary>
    public class CustomAppConfigRepository : BaseRepository<CustomAppConfigEntity>, ICustomAppConfigRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAppConfigRepository"/> class.
        /// </summary>
        /// <param name="logger">The logging service.</param>
        /// <param name="repositoryOptions">Options used to create the repository.</param>
        public CustomAppConfigRepository(
            ILogger<CustomAppConfigRepository> logger,
            IOptions<RepositoryOptions> repositoryOptions)
            : base(
                  logger,
                  storageAccountConnectionString: repositoryOptions.Value.StorageAccountConnectionString,
                  tableName: CustomAppConfigTableName.TableName,
                  defaultPartitionKey: CustomAppConfigTableName.SettingsPartition,
                  ensureTableExists: repositoryOptions.Value.EnsureTableExists)
        {
        }
    }
}
