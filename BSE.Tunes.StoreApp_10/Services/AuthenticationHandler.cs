using BSE.Tunes.StoreApp.Models;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Client;
using Windows.Security.Credentials;

namespace BSE.Tunes.StoreApp.Services
{
    public class AuthenticationHandler : IAuthenticationHandler
    {
        #region Constants
        private const string PasswordVaultResourceName = "7a55257a-b621-4027-a266-af900c21c256";
        private const string OAuthClientSercret = "f2186598-35f4-496d-9de0-41157a27642f";
        #endregion

        #region FieldsPrivate
        private SettingsService m_settingsService;
        private IResourceService m_resourceService;
        private readonly string m_strUnauthorizedAccessExceptionMessage;
        private readonly string m_strEncryptedLoginException;
        #endregion

        #region Properties
        public string TokenEndpointAddress
        {
            get
            {
                return string.Format("{0}/token", m_settingsService.ServiceUrl);
            }
        }
        #endregion

        #region MethodsPublic
        public static AuthenticationHandler Instance
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IAuthenticationHandler>() as AuthenticationHandler;
            }
        }
        public async Task<User> VerifyUserCredentialsAsync()
        {
            User user = null;
            PasswordVault vault = new PasswordVault();
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
                            user = new User
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
            return user;
        }
        public async Task<User> VerifyUserAuthenticationAsync()
        {
            User user = await VerifyUserCredentialsAsync();
            if (user != null)
            {
                if (!string.IsNullOrEmpty(user.UserName) && !string.IsNullOrEmpty(user.Password))
                {
                    try
                    {
                        user = await AuthenticateAsync(user.UserName, user.Password, user.UseSecureLogin);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    throw new UnauthorizedAccessException(this.m_strUnauthorizedAccessExceptionMessage);
                }
            }
            return user;
        }
        public async Task<User> AuthenticateAsync(string userName, string password, bool useSecureLogin)
        {
            User user = null;
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                var client = this.GetHttpClient(useSecureLogin);
                try
                {
                    var tokenResponse = await client.RequestResourceOwnerPasswordAsync(userName, password);
                    if (tokenResponse != null)
                    {
                        if (tokenResponse.IsError)
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
                            user = new User
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
                    NullReferenceException nullReferenceException = exception as NullReferenceException;
                    if (nullReferenceException != null)
                    {
                        //there could be a nullreference exception at account change when the login is encrypted.
                        throw new UnauthorizedAccessException(this.m_strEncryptedLoginException);
                    }
                    throw exception;
                }
            }
            else
            {
                throw new UnauthorizedAccessException(this.m_strUnauthorizedAccessExceptionMessage);
            }
            return user;
        }
        #endregion

        #region MethodsPrivate
        public AuthenticationHandler()
        {
            m_settingsService = SettingsService.Instance;
            m_resourceService = ResourceService.Instance;
            this.m_strUnauthorizedAccessExceptionMessage = m_resourceService.GetString(
                "UnauthorizedAccessExceptionMessage", "The user name or password is incorrect");
            this.m_strEncryptedLoginException = m_resourceService.GetString(
                "EncryptedLoginException", "There is a login error. Please deactivate the encrypted login.");
        }
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
                OAuthClientSercret);

            //return new OAuth2Client(
            //    uriBuilder.Uri,
            //    "BSEtunes",
            //    OAuthClientSercret,
            //    new WindowsRuntime.HttpClientFilters.WinRtHttpClientHandler(filter));
        }
        #endregion
    }
}
