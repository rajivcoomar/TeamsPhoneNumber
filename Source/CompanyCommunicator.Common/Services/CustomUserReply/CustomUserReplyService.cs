// <copyright file="CustomUserReplyService.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Teams.Apps.CompanyCommunicator.Common.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Schema;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Repositories.UserData;
    using Microsoft.Teams.Apps.CompanyCommunicator.Common.Services.MessageQueues.SendQueue;
    using Newtonsoft.Json;

    /// <summary>
    /// App settings service implementation.
    /// </summary>
    public class CustomUserReplyService : ICustomUserReplyService
    {
        private readonly ICustomUserReplyRepository repository;
        private readonly IUserDataRepository userRepository;
        private readonly ICustomAppConfigRepository customAppConfigRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUserReplyService"/> class.
        /// </summary>
        /// <param name="repository">Custom User Reply repository.</param>
        /// <param name="userRepository">User Data repository.</param>
        /// <param name="customAppConfigRepository">custom App Config Repository</param>
        public CustomUserReplyService(
            ICustomUserReplyRepository repository,
            IUserDataRepository userRepository,
            ICustomAppConfigRepository customAppConfigRepository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.customAppConfigRepository = customAppConfigRepository ?? throw new ArgumentNullException(nameof(customAppConfigRepository));
        }

        /// <inheritdoc/>
        public async Task<string> GetCustomReplyAsync(string rowKeyValue)
        {
            // check in-memory cache.
            if (string.IsNullOrWhiteSpace(rowKeyValue))
            {
                throw new ArgumentNullException(nameof(rowKeyValue));
            }

            var customUserReply = await this.repository.GetAsync(
                partitionKey: CustomUserReplyTableName.UserTeamsReplyPartition,
                rowKey: rowKeyValue);

            return customUserReply?.UserMessage;
        }

        /// <inheritdoc/>
        public async Task<string> SaveCustomReplyAsync(IMessageActivity activity)
        {
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity));
            }

            // to get the earlier message and append the current message to earlier one
            string rowKeyValue = activity.From.AadObjectId; // activity.Conversation.Id + ":" + activity.From.AadObjectId;
            var messagetoUpdate = await this.GetCustomReplyAsync(rowKeyValue);
            if (string.IsNullOrEmpty(messagetoUpdate))
            {
                messagetoUpdate = activity.Text;
            }
            else
            {
                messagetoUpdate += "||" + activity.Text;
            }

            var customUserReply = new CustomUserReplyEntity()
            {
                PartitionKey = CustomUserReplyTableName.UserTeamsReplyPartition,
                RowKey = rowKeyValue, // activity.Conversation.Id + ":" + activity.From.AadObjectId,
                ConversationId = activity.Conversation.Id,
                UserID = activity.From.AadObjectId,
                UserMessage = messagetoUpdate,
            };

            await this.repository.InsertOrMergeAsync(customUserReply);

            var messageResponseDict = await this.UpdateInitialMessageSent(activity);
            var messageResponse = string.Empty;
            var langResponse = messageResponseDict.ContainsKey("Language") ? messageResponseDict["Language"].ToString() : "English";

            if (!messageResponseDict.ContainsKey("Message"))
            {
                if (Constants.StopKeywords.Contains(activity.Text.ToLower()))
                {
                    if (Constants.SpanishKeywords.Contains(langResponse.ToLower()))
                    {
                        messageResponse = await this.GetMessageAsync(CustomAppConfigTableName.StopMessageESRowKey);
                    }
                    else
                    {
                        messageResponse = await this.GetMessageAsync(CustomAppConfigTableName.StopMessageENRowKey);
                    }

                    await this.OnOptOutFunctionAsync(activity.From.AadObjectId);
                }
                else if (Constants.AllLanguageKeywords.Contains(activity.Text.ToLower()))
                {
                    if (Constants.SpanishKeywords.Contains(langResponse.ToLower()))
                    {
                        messageResponse = await this.GetMessageAsync(CustomAppConfigTableName.LanguageChangeESRowKey);
                    }
                    else
                    {
                        messageResponse = await this.GetMessageAsync(CustomAppConfigTableName.LanguageChangeENRowKey);
                    }
                    messageResponse = messageResponse + " " + activity.Text;
                }
                else
                {
                    if (Constants.SpanishKeywords.Contains(langResponse.ToLower()))
                    {
                        messageResponse = await this.GetMessageAsync(CustomAppConfigTableName.ThanksMessageESRowKey);
                    }
                    else
                    {
                        messageResponse = await this.GetMessageAsync(CustomAppConfigTableName.ThanksMessageENRowKey);
                    }
                }
            }
            else
            {
                messageResponse = messageResponseDict["Message"].ToString();
            }

            return messageResponse;
        }

        /// <inheritdoc/>
        public async Task<string> GetUpdateInitialMessage(string userId)
        {
            var messageResponse = string.Empty;

            // TO GET CHECK IF MESSAGE SENT IS UPDATED AND NOTED PREF LANGUAGE
            var userData = await this.userRepository.GetAsync(
                            partitionKey: UserDataTableNames.UserDataPartition,
                            rowKey: userId);

            var isMsgSent = userData?.TeamsInitialMessageSent ?? false;
            if (!isMsgSent)
            {
                if (!string.IsNullOrEmpty(userData.PreferredLanguage) && Constants.SpanishKeywords.Contains(userData.PreferredLanguage.ToLower()))
                {
                    messageResponse = await this.GetMessageAsync(CustomAppConfigTableName.InitialTeamsMessageESRowKey);
                }
                else
                {
                    messageResponse = await this.GetMessageAsync(CustomAppConfigTableName.InitialTeamsMessageESRowKey);
                }

                userData.TeamsInitialMessageSent = true;
                await this.userRepository.InsertOrMergeAsync(userData);
            }

            return messageResponse;
        }

        /// <summary>
        /// remove user from group.
        /// </summary>
        /// <param name="employeeID">employee id.</param>
        /// <returns>A representing the result of the asynchronous operation.</returns>
        public async Task<string> OnOptOutFunctionAsync(string employeeID)
        {
            try
            {
                var url = await this.GetMessageAsync(CustomAppConfigTableName.OptOutFunctionURLRowKey); // "https://sendsmscc.azurewebsites.net/api/Opt-OutFunction?code=KJHBZN1xBno_Fz_5sX0AIs2QV4YPGtZPD8tLfIUulyqPAzFuyzW0EQ==";

                dynamic content = new { UserID = employeeID, UserTypeToDelete = "TEAMS" };

                CancellationToken cancellationToken;

                using (var myclient = new HttpClient())
                using (var myrequest = new HttpRequestMessage(HttpMethod.Post, url))
                using (var httpContent = this.CreateHttpContent(content))
                {
                    myrequest.Content = httpContent;

                    using (var newresponse = await myclient
                        .SendAsync(myrequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                        .ConfigureAwait(false))
                    {
                        //var MyListData = newresponse;
                        return string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// remove user from group.
        /// </summary>
        /// <param name="messageContent">message Content.</param>
        /// <returns>A representing the result of the asynchronous operation.</returns>
        public async Task<string> SendSMSFunctionAsync(SendQueueMessageContent messageContent)
        {
            try
            {
                var url = await this.GetMessageAsync(CustomAppConfigTableName.SendSMSFunctionURLRowKey); // "https://sendsmscc.azurewebsites.net/api/Opt-OutFunction?code=KJHBZN1xBno_Fz_5sX0AIs2QV4YPGtZPD8tLfIUulyqPAzFuyzW0EQ==";

                dynamic content = messageContent; // new { UserID = employeeID, UserTypeToDelete = "TEAMS" };

                CancellationToken cancellationToken;

                using (var myclient = new HttpClient())
                using (var myrequest = new HttpRequestMessage(HttpMethod.Post, url))
                using (var httpContent = this.CreateHttpContent(content))
                {
                    myrequest.Content = httpContent;

                    using (var newresponse = await myclient
                        .SendAsync(myrequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                        .ConfigureAwait(false))
                    {
                        //var MyListData = newresponse;
                        return string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Serialize Json Into Stream.
        /// </summary>
        /// <param name="value">value.</param>
        /// <param name="stream">stream.</param>
        public void SerializeJsonIntoStream(object value, Stream stream)
        {
            using (var stremw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
            using (var jsonw = new JsonTextWriter(stremw) { Formatting = Formatting.None })
            {
                var js = new JsonSerializer();
                js.Serialize(jsonw, value);
                jsonw.Flush();
            }
        }

        /// <summary>
        /// TO UPDATE THE INITIAL MESSAGE SENT STATUS AND PREF LANGUAGE.
        /// </summary>
        /// <param name="activity">message activity.</param>
        /// <exception cref="ArgumentNullException">if no record.</exception>
        private async Task<Dictionary<string, string>> UpdateInitialMessageSent(IMessageActivity activity)
        {
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity));
            }

            Dictionary<string, string> responseMessages = new Dictionary<string, string>();

            // TO GET CHECK IF MESSAGE SENT IS UPDATED AND NOTED PREF LANGUAGE
            var userData = await this.userRepository.GetAsync(
                            partitionKey: UserDataTableNames.UserDataPartition,
                            rowKey: activity.From.AadObjectId);

            var isMsgSent = userData?.TeamsInitialMessageSent ?? false;

            // TO SEND INITIAL MESSAGE AGAIN IF "HELP"
            if (Constants.HelpKeywords.Contains(activity.Text.ToLower()))
            {
                isMsgSent = false;
            }

            if (isMsgSent)
            {
                if (Constants.AllLanguageKeywords.Contains(activity.Text.ToLower()))
                {
                    userData.PreferredLanguage = activity.Text;
                    await this.userRepository.InsertOrMergeAsync(userData);
                }

                responseMessages.Add("Language", userData.PreferredLanguage);
                return responseMessages;
            }

            userData.TeamsInitialMessageSent = true;
            userData.PreferredLanguage = Constants.AllLanguageKeywords.Contains(activity.Text.ToLower()) ? activity.Text :
                (string.IsNullOrEmpty(userData.PreferredLanguage) ? string.Empty : userData.PreferredLanguage);

            await this.userRepository.InsertOrMergeAsync(userData);

            responseMessages.Add("Language", userData.PreferredLanguage);

            var messageResponse = string.Empty;

            if (Constants.SpanishKeywords.Contains(userData.PreferredLanguage.ToLower()))
            {
                messageResponse = await this.GetMessageAsync(CustomAppConfigTableName.InitialTeamsMessageESRowKey);
            }
            else if (Constants.EnglishKeywords.Contains(userData.PreferredLanguage.ToLower()) || string.IsNullOrEmpty(userData.PreferredLanguage))
            {
                messageResponse = await this.GetMessageAsync(CustomAppConfigTableName.InitialTeamsMessageENRowKey);
            }

            responseMessages.Add("Message", messageResponse);

            return responseMessages;
        }

        private async Task<string> GetMessageAsync(string rowKey)
        {
            // check in-memory cache.
            if (string.IsNullOrWhiteSpace(rowKey))
            {
                rowKey = CustomAppConfigTableName.ThanksMessageENRowKey;
            }

            var custAppConfig = await this.customAppConfigRepository.GetAsync(
                CustomAppConfigTableName.SettingsPartition,
                rowKey);

            return custAppConfig?.Value;
        }

        private HttpContent CreateHttpContent(object content)
        {
            HttpContent httpContent = null;

            if (content != null)
            {
                var memorystresm = new MemoryStream();
                this.SerializeJsonIntoStream(content, memorystresm);
                memorystresm.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(memorystresm);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }


            return httpContent;
        }
    }
}
