using Tree.Api.Model;
using Tree.App.Authorization.Handler;
using Tree.App.Core.Exception;
using Tree.Domain.Model.Authorization;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Tree.Api.OAuth {
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider {
        private readonly IAuthorizationHandler handler;

        public AuthorizationServerProvider(IAuthorizationHandler handler) {
            this.handler = handler;
        }

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context) {
            await Task.Run(() => { context.Validated(); });
        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context) {
            var result = Task.FromResult<object>(null);
            Authorization authorization;

            if (context.Ticket == null) {
                context.SetError("Bad request");
                return result;

            } else if (context.Ticket.Properties.ExpiresUtc < DateTime.UtcNow) {
                context.SetError("Refresh token expired");
                return result;

            } else {
                var userId = Guid.Parse(context.Ticket.Identity.Claims.First(x => x.Type == "Id").Value);
                try {
                    authorization = handler.GetAuthorization(userId, null, true);
                } catch (PermissionException ex) {
                    context.SetError(string.Join(";", ex.Errors.Select(x => x.Key + ", " + x.Value)));
                    return result;
                }

                if (!authorization.User.IsActive) {
                    context.SetError("User, Inactive User");
                    return result;
                }
            }

            return base.GrantRefreshToken(context);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context) {
            var form = await context.Request.ReadFormAsync();

            if (string.IsNullOrEmpty(form["access_code"])) {
                context.SetError("Invalid access code");
                return;
            }

            var accessCode = form["access_code"];
            var username = form["username"];
            Authorization authorization;

            try {
                authorization = handler.GetAuthorization(accessCode, username, null, true);
            } catch (PermissionException ex) {
                context.SetError(string.Join(";", ex.Errors.Select(x => x.Key + ", " + x.Value)));
                return;
            }

            if (!authorization.User.IsActive) {
                context.SetError("User, Inactive User");
                return;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);

            identity.AddClaims(new[] {
                new Claim(ClaimTypes.Name, authorization.User.Name),
                new Claim(ClaimTypes.Email, authorization.User.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, authorization.Role.ToString()),
                new Claim("CompanyId", authorization.CompanyId.ToString()),
                new Claim("Id", authorization.User.Id.ToString())
            });

            context.Validated(identity);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context) {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary) {
                if (property.Key.Equals(".issued") || property.Key.Equals(".expires")) {
                    DateTimeOffset date;
                    if (DateTimeOffset.TryParse(property.Value, out date))
                        context.AdditionalResponseParameters.Add(property.Key, date.ToString(Format.DateTime));
                }
            }
            return Task.FromResult<object>(null);
        }
    }
}