
using AspNet.Identity.MySQL;
using BSE.Tunes.WebApi.Identity;
using BSE.Tunes.WebApi.Identity.MySQL;
using BSE.Tunes.WebApi.Providers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;

namespace BSE.Tunes.WebApi2
{
    public partial class Startup
	{
		public static string PublicClientId => "self";

		private static MySQLDatabase _database => new MySQLDatabase();

		public static OAuthAuthorizationServerOptions OAuthOptions => new OAuthAuthorizationServerOptions
		{
			TokenEndpointPath = new PathString("/Token"),
			Provider = new ApplicationOAuthProvider(PublicClientId, UserManagerFactory),
			RefreshTokenProvider = new RefreshTokenProvider(),
			ApplicationCanDisplayErrors = true,
			AllowInsecureHttp = true
		};

  //      public static Func<RefreshTokenManager<RefreshToken>> RefreshTokenManager = () =>
		//{
		//	return new RefreshTokenManager<RefreshToken>(new RefreshTokenStore(_database));
		//};

		public static Func<UserManager<IdentityUser>> UserManagerFactory = () =>
        {
            return new UserManager<IdentityUser>(new UserStore(_database));
        };

		// For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
		public void ConfigureAuth(IAppBuilder app)
		{
			// Enable the application to use bearer tokens to authenticate users
			app.UseOAuthBearerTokens(OAuthOptions);
		}
	}
}