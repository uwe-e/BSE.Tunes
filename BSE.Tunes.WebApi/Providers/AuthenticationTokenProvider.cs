using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BSE.Tunes.WebApi.Providers
{
    public class AuthenticationTokenProvider : IAuthenticationTokenProvider
    {
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

            //var refreshTokenTicket = new AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties);
            //_refreshTokens.TryAdd(guid, context.Ticket);
            //_refreshTokens.TryAdd(guid, refreshTokenTicket);
            context.Ticket.Properties.IssuedUtc = DateTime.Now;
            context.Ticket.Properties.ExpiresUtc = new DateTimeOffset(DateTime.Now.AddDays(1));

            context.SetToken(context.SerializeTicket());
            //context.SetToken(refreshTokenTicket.ToString());
        }
        public void Receive(AuthenticationTokenReceiveContext context)
        {
        }
        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);
        }
    }
}