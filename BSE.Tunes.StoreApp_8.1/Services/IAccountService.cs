using BSE.Tunes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Client;

namespace BSE.Tunes.StoreApp.Services
{
    public interface IAccountService
    {
        string ServiceUrl { get; set; }
        TunesUser User { get; }

        Task<TunesUser> VerifyUserCredentials();
        Task<TunesUser> VerifyUserAuthentication();
        Task<TunesUser> SignInUser(string userName, string password);
        Task<TokenResponse> RefreshToken();
    }
}
