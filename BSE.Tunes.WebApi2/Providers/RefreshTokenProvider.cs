using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Threading.Tasks;

namespace BSE.Tunes.WebApi.Providers
{
    public class RefreshTokenProvider : IAuthenticationTokenProvider
    {
        void IAuthenticationTokenProvider.Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            context.Ticket.Properties.IssuedUtc = DateTime.Now;
            context.Ticket.Properties.ExpiresUtc = new DateTimeOffset(DateTime.Now.AddDays(10));

            context.SetToken(context.SerializeTicket());

            return Task.FromResult<object>(null);
        }

        void IAuthenticationTokenProvider.Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }

        public Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);
            return Task.FromResult<object>(null);
        }
    }
}