using Tree.Api.Model;
using Tree.Api.OAuth;
using Tree.App.Authorization.Handler;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Security.Claims;
using System.Web.Http;

namespace Tree.Api.Controller {
    [AllowAnonymous]
    public class TokenController : ApiController {
        private readonly IAuthorizationHandler handler;
        private readonly OAuthBearerAuthenticationOptions options;

        public TokenController(AuthorizationServerProvider provider, IAuthorizationHandler handler, OAuthBearerAuthenticationOptions options) {
            this.handler = handler;
            this.options = options;
        }

        public IHttpActionResult Post([FromBody] Body body) {
            var authorization = handler.GetAuthorization(body.access_code, body.username);

            ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Name, authorization.User.Name));
            identity.AddClaim(new Claim(ClaimTypes.Email, authorization.User.Email ?? string.Empty));
            identity.AddClaim(new Claim(ClaimTypes.Role, authorization.Role.ToString()));
            identity.AddClaim(new Claim("CompanyId", authorization.CompanyId.ToString()));
            identity.AddClaim(new Claim("Id", authorization.User.Id.ToString()));

            var tokenProperties = new AuthenticationProperties {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(60)
            };

            var refreshTokenProperties = new AuthenticationProperties {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(120)
            };

            var ticket = new AuthenticationTicket(identity, tokenProperties);
            var refreshTokenTicket = new AuthenticationTicket(identity, refreshTokenProperties);

            var accessToken = options.AccessTokenFormat.Protect(ticket);
            var refreshToken = options.AccessTokenFormat.Protect(refreshTokenTicket);

            var response = new JObject(
                new JProperty("access_token", accessToken),
                new JProperty("token_type", "bearer"),
                new JProperty("expires_in", TimeSpan.FromMinutes(60).TotalSeconds.ToString(CultureInfo.InvariantCulture)),
                new JProperty("refresh_token", refreshToken),
                new JProperty(".issued", ticket.Properties.IssuedUtc.Value.ToString(Format.DateTime)),
                new JProperty(".expires", ticket.Properties.ExpiresUtc.Value.ToString(Format.DateTime)));

            return Ok(response);
        }

        public class Body {
            public string grant_type { get; set; }
            public string access_code { get; set; }
            public string username { get; set; }
        }
    }
}