// <copyright file="LanguageTranslateService.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Azure.Amqp.Framing;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.NotificationData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.MessageQueues.SendQueue;
    using Newtonsoft.Json;

    /// <summary>
    /// Language Translate service implementation.
    /// </summary>
    public class LanguageTranslateService : ILanguageTranslateService
    {
        //private const string RegionVar = "TRANSLATOR_SERVICE_REGION";
        //private const string KeyVar = "TRANSLATOR_TEXT_SUBSCRIPTION_KEY";
        //private const string EndpointVar = "TRANSLATOR_TEXT_ENDPOINT";

        private string regionVar = "TRANSLATOR_SERVICE_REGION";
        private string keyVar = "TRANSLATOR_TEXT_SUBSCRIPTION_KEY";
        private string endpointVar = "TRANSLATOR_TEXT_ENDPOINT";

        // private static readonly string Region = "eastus"; // Environment.GetEnvironmentVariable(region_var);
        // private static readonly string SubscriptionKey = "7ddf128378b8449db6ae44917dddd9bd"; // Environment.GetEnvironmentVariable(key_var);
        // private static readonly string Endpoint = "https://api.cognitive.microsofttranslator.com/"; // Environment.GetEnvironmentVariable(endpoint_var);
        private static readonly string Route = "/translate?api-version=3.0&to=es"; // to=de&to=it&to=ja&

        private readonly ICustomMessageLocaleRepository customMessageLocaleRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageTranslateService"/> class.
        /// </summary>
        public LanguageTranslateService(ICustomMessageLocaleRepository customMessageLocaleRepository)
        {
            this.customMessageLocaleRepository = customMessageLocaleRepository ?? throw new ArgumentNullException(nameof(customMessageLocaleRepository));
        }

        /// <inheritdoc/>
        public async Task<string> TranslateTextRequest(string Region, string SubscriptionKey, string Endpoint, NotificationDataEntity notificationData)
        {
            if (Region == null)
            {
                throw new Exception("Please set/export the environment variable: " + this.regionVar);
            }

            if (SubscriptionKey == null)
            {
                throw new Exception("Please set/export the environment variable: " + this.keyVar);
            }

            if (Endpoint == null)
            {
                throw new Exception("Please set/export the environment variable: " + this.endpointVar);
            }

            this.regionVar = Region;
            this.keyVar = SubscriptionKey;
            this.endpointVar = Endpoint;

            var customMessageLocaleData = new CustomMessageLocaleEntity()
            {
                Author = notificationData.Author,
                ButtonLink = notificationData.ButtonLink,
                ButtonTitle = await this.TranslateText(notificationData.ButtonTitle),
                CreatedBy = notificationData.CreatedBy,
                CreatedDate = notificationData.CreatedDate,
                Summary = await this.TranslateText(notificationData.Summary),
                Title = await this.TranslateText(notificationData.Title),
                NotificationId = notificationData.RowKey,
                RowKey = notificationData.RowKey,
                PartitionKey = CustomMessageLocaleTableName.SpanishLangPartition,
            };

            await this.customMessageLocaleRepository.CreateOrUpdateAsync(customMessageLocaleData);

            return string.Empty;
        }

        /// <inheritdoc/>
        public async Task UpdateNewNotificationID(string oldNotificationID, string newNotificationID)
        {
            var cusomLangData = await this.customMessageLocaleRepository.GetAsync(CustomMessageLocaleTableName.SpanishLangPartition, oldNotificationID);

            var newCusomLangData = new CustomMessageLocaleEntity()
            {
                Author = cusomLangData.Author,
                ButtonLink = cusomLangData.ButtonLink,
                ButtonTitle = cusomLangData.ButtonTitle,
                CreatedBy = cusomLangData.CreatedBy,
                CreatedDate = cusomLangData.CreatedDate,
                Summary = cusomLangData.Summary,
                Title = cusomLangData.Title,
                NotificationId = cusomLangData.RowKey,
                RowKey = newNotificationID,
                PartitionKey = CustomMessageLocaleTableName.SpanishLangPartition,
            };
            await this.customMessageLocaleRepository.CreateOrUpdateAsync(newCusomLangData);

            // Delete the draft language translated notification .
            await this.customMessageLocaleRepository.DeleteAsync(cusomLangData);
        }

        private async Task<string> TranslateText(string inputText)
        {
            if (string.IsNullOrEmpty(inputText))
            {
                return string.Empty;
            }

            List<string> retString = new List<string>();

            object[] body = new object[] { new { Text = inputText } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // Build the request.
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(this.endpointVar + Route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", this.keyVar);
                request.Headers.Add("Ocp-Apim-Subscription-Region", this.regionVar);

                // Send the request and get response.
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

                // Read response as a string.
                string result = await response.Content.ReadAsStringAsync();
                TranslationResult[] deserializedOutput = JsonConvert.DeserializeObject<TranslationResult[]>(result);

                // Iterate over the deserialized results.
                foreach (TranslationResult o in deserializedOutput)
                {
                    // Print the detected input languge and confidence score.
                    Console.WriteLine("Detected input language: {0}\nConfidence score: {1}\n", o.DetectedLanguage.Language, o.DetectedLanguage.Score);

                    // Iterate over the results and print each translation.
                    foreach (Translation t in o.Translations)
                    {
                        Console.WriteLine("Translated to {0}: {1}", t.To, t.Text);
                        retString.Add(t.Text);
                    }
                }
            }

            return retString.FirstOrDefault();
        }
    }
}
