using Tree.App.Authorization.Handler;
using Microsoft.Owin.Security.Infrastructure;
using System;

namespace Tree.Api.OAuth {
    public class RefreshTokenProvider : AuthenticationTokenProvider {
        private readonly int tokenExpiration;
        private readonly IAuthorizationHandler handler;

        public RefreshTokenProvider(IAuthorizationHandler handler) {
            tokenExpiration = 24;
            this.handler = handler;
        }

        public override void Create(AuthenticationTokenCreateContext context) {
            if (context.Response.StatusCode == 200) {
                int expire = tokenExpiration;
                context.Ticket.Properties.ExpiresUtc = new DateTimeOffset(DateTime.Now.AddHours(expire));
                context.SetToken(context.SerializeTicket());
            }
        }

        public override void Receive(AuthenticationTokenReceiveContext context) {
            context.DeserializeTicket(context.Token);
        }
    }
}