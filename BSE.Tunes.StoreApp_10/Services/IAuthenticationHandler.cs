using BSE.Tunes.StoreApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Client;

namespace BSE.Tunes.StoreApp.Services
{
    public interface IAuthenticationHandler
    {
        //TokenResponse TokenResponse
        //{
        //    get;
        //}
        Task<User> AuthenticateAsync(string userName, string password, bool useSecureLogin);
        Task<User> VerifyUserAuthenticationAsync();
        Task<User> VerifyUserCredentialsAsync();
    }
}
