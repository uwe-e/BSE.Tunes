using BSE.Tunes.WebApi.Identity;
using BSE.Tunes.WebApi.Identity.Extensions;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BSE.Tunes.WebApi.Providers
{
    public class AuthenticationTokenProvider : IAuthenticationTokenProvider
    {
        private readonly Func<RefreshTokenManager<RefreshToken>> _refreshTokenManager;

        public AuthenticationTokenProvider(Func<RefreshTokenManager<RefreshToken>> refreshTokenManager)
        {
            _refreshTokenManager = refreshTokenManager;
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
        }
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var clientid = context.Ticket.Properties.Dictionary["as:client_id"];
            if (string.IsNullOrEmpty(clientid))
            {
                return;
            }
            var refreshTokenId = Guid.NewGuid().ToString("n");

            //TODO: support multiple clients
            //
            //http://bitoftech.net/2014/07/16/enable-oauth-refresh-tokens-angularjs-app-using-asp-net-web-api-2-owin/
            //https://github.com/thinktecture/Thinktecture.IdentityServer.v3/blob/1b33386e6ba29242293e20d806c2ad15a5bcf8ec/source/Core/Services/Default/DefaultRefreshTokenService.cs


            // maybe only create a handle the first time, then re-use for same client
            // copy properties and set the desired lifetime of refresh token
            //var refreshTokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary)
            //{
            //    IssuedUtc = context.Ticket.Properties.IssuedUtc,
            //    ExpiresUtc = DateTime.Now.AddMinutes(1)
            //};

            var refreshToken = new RefreshToken
            {
                Id = refreshTokenId,
                UserName = context.Ticket.Identity.Name,
                CreationTime = DateTime.UtcNow,
                LifeTime = 1296000
            };


            //context.Ticket.Properties.IssuedUtc = refreshToken.CreationTime;
            //context.Ticket.Properties.ExpiresUtc = refreshToken.LifeTime;

            refreshToken.SerializedTicket = context.SerializeTicket();

            using (  RefreshTokenManager<RefreshToken> refreshTokenManager = _refreshTokenManager())
            {
                await refreshTokenManager.StoreRefreshToken(refreshToken);
            }


            //context.SetToken(context.SerializeTicket());
            context.SetToken(refreshTokenId);

            //context.SetToken(refreshTokenTicket.ToString());
        }
        public void Receive(AuthenticationTokenReceiveContext context)
        {
        }
        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            var token = context.Token;

            using (RefreshTokenManager<RefreshToken> refreshTokenManager = _refreshTokenManager())
            {
                var refreshToken = await refreshTokenManager.GetRefreshTokenAsync(token);
                if (refreshToken == null)
                {
                    //Invalid refresh token
                    return ;

                }
                
                if (refreshToken.CreationTime.HasExceeded(refreshToken.LifeTime, DateTime.UtcNow))
                {
                    //Refresh token has expired.
                    //isInvalidGrant = true;
                    //return;
                    return;
                }

                context.DeserializeTicket(refreshToken.SerializedTicket);

                //var properties = new Microsoft.Owin.Security.AuthenticationProperties(new Dictionary<string, string>
                //{
                //    {
                //        "expires_at", DateTime.UtcNow.AddSeconds(refreshToken.LifeTime).ToString()
                //    }
                //});

                //var refreshTokenTicket = new Microsoft.Owin.Security.AuthenticationTicket(context.Ticket.Identity, properties);
                //context.SetTicket(refreshTokenTicket);
                await refreshTokenManager.RemoveRefreshTokenAsync(token);

            }
        }

        //context.DeserializeTicket(context.Token);
    }
}