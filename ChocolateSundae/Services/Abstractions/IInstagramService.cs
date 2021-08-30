using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstagramApiSharp.Classes.Models;

namespace ChocolateSundae.Services.Abstractions
{
    interface IInstagramService
    {
        Task TryAuthenticateAsync();

        bool IsAuthenticated();

        Task<string> GetUserProfileOrDefaultAsync(string username);
    }
}
