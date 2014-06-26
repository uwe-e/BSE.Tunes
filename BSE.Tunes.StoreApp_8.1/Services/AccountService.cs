using BSE.Tunes.StoreApp.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Client;
using Windows.Security.Credentials;

namespace BSE.Tunes.StoreApp.Services
{
    public class AccountService : IAccountService
    {
        #region Constants
        private const string PasswordVaultResourceName = "7a55257a-b621-4027-a266-af900c21c256";
        private const string OAuthClientSercret = "f2186598-35f4-496d-9de0-41157a27642f";
        #endregion

        #region FieldsPrivate
        private IResourceService m_resourceService;
        private readonly string m_strUnauthorizedAccessExceptionMessage;
        #endregion

        #region Properties
        public string ServiceUrl
        {
            get;
            set;
        }
        public string TokenEndpointAddress
        {
            get
            {
                return string.Format("{0}/token", this.ServiceUrl);
            }
        }
        public TokenResponse TokenResponse
        {
            get;
            private set;
        }
        public TunesUser User
        {
            get;
            private set;
        }
        #endregion

        #region MethodsPublic
        public AccountService(IResourceService resourceService)
        {
            this.m_resourceService = resourceService;
            this.m_strUnauthorizedAccessExceptionMessage = this.m_resourceService.GetString(
                "UnauthorizedAccessExceptionMessage", "The user name or password is incorrect");
        }
        public async Task<TunesUser> VerifyUserCredentials()
        {
            TunesUser tunesUser = null;
            Windows.Security.Credentials.PasswordVault vault = new Windows.Security.Credentials.PasswordVault();
            try
            {
                await Task.Run(() =>
                {
                    var userName = (string)Windows.Storage.ApplicationData.Current.RoamingSettings.Values["username"];
                    if (!string.IsNullOrEmpty(userName))
                    {
                        var passwordCredential = vault.Retrieve(PasswordVaultResourceName, userName);
                        if (passwordCredential != null)
                        {
                            tunesUser = this.User = new TunesUser
                            {
                                UserName = userName,
                                Password = vault.Retrieve(PasswordVaultResourceName, passwordCredential.UserName).Password,
                                UseSecureLogin = Windows.Storage.ApplicationData.Current.RoamingSettings.Values["usesecurelogin"] != null ? (bool)Windows.Storage.ApplicationData.Current.RoamingSettings.Values["usesecurelogin"] : false
                            };
                        }
                    }
                });
            }
            catch { }
            return tunesUser;
        }
        public async Task<TunesUser> VerifyUserAuthentication()
        {
            TunesUser tunesUser = await VerifyUserCredentials();
            if (tunesUser != null)
            {
                if (!string.IsNullOrEmpty(tunesUser.UserName) && !string.IsNullOrEmpty(tunesUser.Password))
                {
                    try
                    {
                        tunesUser = await SignInUser(tunesUser.UserName, tunesUser.Password, tunesUser.UseSecureLogin);
                    }
                    catch(Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    throw new UnauthorizedAccessException(this.m_strUnauthorizedAccessExceptionMessage);
                }
            }
            return tunesUser;
        }
        public async Task<TunesUser> SignInUser(string userName, string password, bool useSecureLogin)
        {
            TunesUser tunesUser = null;
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                var client = this.GetHttpClient(useSecureLogin);
                try
                {
                    this.TokenResponse = await client.RequestResourceOwnerPasswordAsync(userName, password);
                    if (this.TokenResponse != null)
                    {
                        if (this.TokenResponse.IsError)
                        {
                            throw new UnauthorizedAccessException(this.m_strUnauthorizedAccessExceptionMessage);
                        }

                        Windows.Storage.ApplicationData.Current.RoamingSettings.Values["username"] = userName;
                        Windows.Storage.ApplicationData.Current.RoamingSettings.Values["usesecurelogin"] = useSecureLogin;

                        Windows.Security.Credentials.PasswordVault vault = new Windows.Security.Credentials.PasswordVault();
                        PasswordCredential passwordCredential = new PasswordCredential(PasswordVaultResourceName, userName, password);
                        vault.Add(passwordCredential);

                        if (passwordCredential != null)
                        {
                            tunesUser = this.User = new TunesUser
                            {
                                UserName = userName,
                                Password = vault.Retrieve(PasswordVaultResourceName, passwordCredential.UserName).Password,
                                UseSecureLogin = useSecureLogin
                            };
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    throw new UnauthorizedAccessException(this.m_strUnauthorizedAccessExceptionMessage);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
            else
            {
                throw new UnauthorizedAccessException(this.m_strUnauthorizedAccessExceptionMessage);
            }
            return tunesUser;
        }
        public async Task<TokenResponse> RefreshToken()
        {
            var client = this.GetHttpClient(false);
            this.TokenResponse = await client.RequestRefreshTokenAsync(this.TokenResponse.RefreshToken);
            return this.TokenResponse;
        }
        #endregion

        #region MethodsPrivate
        private OAuth2Client GetHttpClient(bool useSecureLogin)
        {
            var filter = new Windows.Web.Http.Filters.HttpBaseProtocolFilter();
            filter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Untrusted);
            filter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.InvalidName);

            UriBuilder uriBuilder = new UriBuilder(this.TokenEndpointAddress);
            if (useSecureLogin)
            {
                if (string.Compare(uriBuilder.Scheme, "http", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    uriBuilder.Scheme = "https";
                    uriBuilder.Port = 443;
                }
            }

            return new OAuth2Client(
                uriBuilder.Uri,
                "BSEtunes",
                OAuthClientSercret,
                new WindowsRuntime.HttpClientFilters.WinRtHttpClientHandler(filter));
        }
        #endregion
    }
}
