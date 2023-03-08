// <copyright file="IAppSettingsService.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Services
{
    using System.Threading.Tasks;

    /// <summary>
    /// App settings interface.
    /// </summary>
    public interface ICustomAppSettingsService
    {
        /// <summary>
        /// Gets message from settings.
        /// </summary>
        /// <param name="rowKey">Row Key to get message</param>
        /// <returns>message.</returns>
        public Task<string> GetMessageAsync(string rowKey);
    }
}
