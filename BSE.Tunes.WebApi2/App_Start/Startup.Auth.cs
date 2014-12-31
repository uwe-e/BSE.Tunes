
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Microsoft.AspNet.Identity;
using AspNet.Identity.MySQL;
using BSE.Tunes.WebApi.Providers;

namespace BSE.Tunes.WebApi2
{
	public partial class Startup
	{
		static Startup()
		{
			PublicClientId = "self";

			UserManagerFactory = () => new UserManager<IdentityUser>(new UserStore(new MySQLDatabase()));
			OAuthOptions = new OAuthAuthorizationServerOptions
			{
				TokenEndpointPath = new PathString("/Token"),
				Provider = new ApplicationOAuthProvider(PublicClientId, UserManagerFactory),
				RefreshTokenProvider = new AuthenticationTokenProvider(),
				//AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
				ApplicationCanDisplayErrors = true,
				AllowInsecureHttp = true
			};
		}

		public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
		public static Func<UserManager<IdentityUser>> UserManagerFactory { get; set; }
		public static string PublicClientId { get; private set; }

		// For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
		public void ConfigureAuth(IAppBuilder app)
		{
			// Configure the application for OAuth based flow
			//PublicClientId = "self";
			//OAuthOptions = new OAuthAuthorizationServerOptions
			//{
			//	TokenEndpointPath = new PathString("/Token"),
			//	Provider = new ApplicationOAuthProvider(PublicClientId),
			//	AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
			//	AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
			//	AllowInsecureHttp = true
			//};

			// Enable the application to use bearer tokens to authenticate users
			app.UseOAuthBearerTokens(OAuthOptions);

			// Uncomment the following lines to enable logging in with third party login providers
			//app.UseMicrosoftAccountAuthentication(
			//    clientId: "",
			//    clientSecret: "");

			//app.UseTwitterAuthentication(
			//    consumerKey: "",
			//    consumerSecret: "");

			//app.UseFacebookAuthentication(
			//    appId: "",
			//    appSecret: "");

			//app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
			//{
			//    ClientId = "",
			//    ClientSecret = ""
			//});
		}
	}
}