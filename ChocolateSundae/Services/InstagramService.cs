using ChocolateSundae.Services.Abstractions;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChocolateSundae.Config;
using System.Net.Http;
using InstagramApiSharp.Enums;
using ChocolateSundae.Services.Exceptions;
using ChocolateSundae.Services.Models;
using InstagramApiSharp;

namespace ChocolateSundae.Services
{
    public class InstagramService : IInstagramService
    {
        public const string StateFile = "state.bin";

        protected IInstaApi? InstaApi;

        public InstagramService()
        {
            
        }

        /// <summary>
        /// Try to authenticate, or use authentication that is already stored.
        /// </summary>
        /// <returns></returns>
        public async Task TryAuthenticateAsync()
        {
            var config = ConfigHelper.Config;

            var userSession = new UserSessionData
            {
                UserName = config.Username,
                Password = config.Password
            };

            InstaApi = InstaApiBuilder.CreateBuilder()
                .SetUser(userSession)
                .UseLogger(new DebugLogger(LogLevel.All))
                .Build();
            InstaApi.SetApiVersion(InstaApiVersionType.Version180);

            try
            {
                // load session file if exists
                if (File.Exists(StateFile))
                {
                    Console.WriteLine("Loading state from file");
                    await using var fs = File.OpenRead(StateFile);
                    await InstaApi.LoadStateDataFromStreamAsync(fs);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            if (!InstaApi.IsUserAuthenticated)
            {
                // login
                Console.WriteLine($"Logging in as {userSession.UserName}");
                var logInResult = await InstaApi.LoginAsync();
                if (!logInResult.Succeeded)
                {
                    Console.WriteLine($"Unable to login: {logInResult.Info.Message}");
                    return;
                }
            }
            // save session in file
            var state = await InstaApi.GetStateDataAsStreamAsync();
            await using var fileStream = File.Create(StateFile);
            state.Seek(0, SeekOrigin.Begin);
            await state.CopyToAsync(fileStream);
        }

        public bool IsAuthenticated()
        {
            return InstaApi?.IsUserAuthenticated ?? false;
        }

        public async Task<UserData?> GetUserData(string username)
        {
            try
            {
                var userInfoRequest = await InstaApi!.UserProcessor.GetUserInfoByUsernameAsync(username);
                var instaUserInfo = userInfoRequest.Succeeded
                    ? userInfoRequest.Value
                    : throw new InstagramErrorException(GetErrorMessageFromResultInfo(userInfoRequest.Info));

                var userMediaRequest =
                    await InstaApi!.UserProcessor.GetUserMediaAsync(username,
                        PaginationParameters.MaxPagesToLoad(1000));
                var instaUserMediaInfo = userMediaRequest.Succeeded
                    ? userMediaRequest.Value
                    : throw new InstagramErrorException(GetErrorMessageFromResultInfo(userMediaRequest.Info));

                var userFullRequest = await InstaApi!.UserProcessor.GetFullUserInfoAsync(instaUserInfo.Pk);
                var instaUserFullInfo = userFullRequest.Succeeded
                    ? userFullRequest.Value
                    : throw new InstagramErrorException(GetErrorMessageFromResultInfo(userFullRequest.Info));

                return UserData.CreateFromInstaUserInfo(instaUserInfo, instaUserMediaInfo, instaUserFullInfo);
            }
            catch
            {
                return null;
            }

        }

        private string GetErrorMessageFromResultInfo(ResultInfo requestInfo)
        {
            if(requestInfo == null)
            {
                return "";
            }

            var message = "";
            if (requestInfo.Message != null)
            {
                message += requestInfo.Message;
            }
            if(requestInfo.Exception != null)
            {
                message += requestInfo.Exception;
            }
            return message;
        }

    }
}
