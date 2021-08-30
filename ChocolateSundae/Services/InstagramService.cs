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

        public async Task<string> GetUserProfileOrDefaultAsync(string username)
        {
            var currentUserRequest = await InstaApi!.UserProcessor.GetUserInfoByUsernameAsync(username);
            var info = currentUserRequest.Succeeded 
                ? currentUserRequest.Value
                : throw new InstagramErrorException(GetErrorMessageFromResultInfo(currentUserRequest.Info));
            return "User profile download success.";
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
