using AspNet.Identity.MySQL;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using NLog;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BSE.Tunes.WebApi.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
	{
		private readonly string _publicClientId;
		private readonly Func<UserManager<IdentityUser>> _userManagerFactory;
		private static Logger logger = LogManager.GetCurrentClassLogger();

		public ApplicationOAuthProvider(string publicClientId, Func<UserManager<IdentityUser>> userManagerFactory)
		{
			if (publicClientId == null)
			{
				throw new ArgumentNullException("publicClientId");
			}

			if (userManagerFactory == null)
			{
				throw new ArgumentNullException("userManagerFactory");
			}

			_publicClientId = publicClientId;
			_userManagerFactory = userManagerFactory;
		}

		public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{
			using (UserManager<IdentityUser> userManager = _userManagerFactory())
			{
				IdentityUser user = await userManager.FindAsync(context.UserName, context.Password);

				if (user == null)
				{
					logger.Info($"{nameof(GrantResourceOwnerCredentials)} invalid grant for user {context.UserName} ");
					
					context.SetError("invalid_grant", "The user name or password is incorrect.");
					return;
				}

				ClaimsIdentity oAuthIdentity = await userManager.CreateIdentityAsync(user,
					context.Options.AuthenticationType);
				ClaimsIdentity cookiesIdentity = await userManager.CreateIdentityAsync(user,
					CookieAuthenticationDefaults.AuthenticationType);

				var properties = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                        "as:client_id", (context.ClientId == null) ? string.Empty : context.ClientId
                    },
                    {
                        "userName", user.UserName
                    },
                    {
						"sub", user.Id
					}
                });

				AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
				context.Validated(ticket);
				//context.Request.Context.Authentication.SignIn(cookiesIdentity);
			}
		}


        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var gdgdgd = base.GrantRefreshToken(context);

            return gdgdgd;
        }
        //public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        //{
        //	//var identity = new ClaimsIdentity(context.Ticket.Identity);
        //	//AuthenticationTicket ticket = new AuthenticationTicket(identity, context.Ticket.Properties);
        //	//context.Validated(ticket);

        //	//return Task.FromResult<object>(null);
        //	//return base.GrantRefreshToken(context);
        //}
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
		{
			foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
			{
				context.AdditionalResponseParameters.Add(property.Key, property.Value);
			}

			return Task.FromResult<object>(null);
		}

		public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
		{
			string clientId = string.Empty;
			string clientSecret = string.Empty;

			if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
			{
				context.TryGetFormCredentials(out clientId, out clientSecret);
			}

			//Multiapplication handling should be added
			// Resource owner password credentials does not provide a client ID.
			//if (context.ClientId == null)
			{
				context.Validated();
			}
			return Task.FromResult<object>(null);
		}

		public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
		{
			if (context.ClientId == _publicClientId)
			{
				Uri expectedRootUri = new Uri(context.Request.Uri, "/");

				if (expectedRootUri.AbsoluteUri == context.RedirectUri)
				{
					context.Validated();
				}
			}

			return Task.FromResult<object>(null);
		}

		//public static AuthenticationProperties CreateProperties(string userName)
		//{
		//	IDictionary<string, string> data = new Dictionary<string, string>
  //          {
  //              { "userName", userName }
  //          };
		//	return new AuthenticationProperties(data);
		//}

	}
}