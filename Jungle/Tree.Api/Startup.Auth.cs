using Tree.Api.OAuth;
using Autofac;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;

namespace Tree.Api {
    public partial class Startup {
        private OAuthBearerAuthenticationOptions oAuthBearerOptions;

        private void ConfigureOAuth(IAppBuilder app, string displayedVersion) {
            var options = new OAuthAuthorizationServerOptions {
                TokenEndpointPath = new PathString("/oauth/token"),
                AuthorizeEndpointPath = new PathString("/oauth/authorization"),
                Provider = container.Resolve<AuthorizationServerProvider>(),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(10),
                AllowInsecureHttp = true,
                RefreshTokenProvider = container.Resolve<RefreshTokenProvider>(),
                ApplicationCanDisplayErrors = true,
                AccessTokenFormat = oAuthBearerOptions.AccessTokenFormat
            };

            app.UseOAuthAuthorizationServer(options);

            app.UseOAuthBearerAuthentication(oAuthBearerOptions);

        }
    }
}