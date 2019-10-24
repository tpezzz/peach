using Tree.Api.Model.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ExpressMapper.Extensions;
using System.Security.Principal;
using Tree.Api.Model.Claims;
using Tree.App.Authorization.Handler;

namespace Tree.Api.Controller {
    public class AccessCodeController : ApiController {
        private readonly IAuthorizationHandler authorizationHandler;
        public AccessCodeController(IAuthorizationHandler authorizationHandler) {
            this.authorizationHandler = authorizationHandler;
        }

        [Authorize(Roles = "SystemAdmin,Supervisor")]
        public async Task<IHttpActionResult> Post([FromBody] ACCommand command) {
            var author = User.Identity.Map<IIdentity, AuthorizationClaims>();

            string accessCode = await authorizationHandler.GenerateAccessCodeAsync(author.Id, author.Role, author.CompanyId, command.UserId);

            return Ok(accessCode);
        }
    }
}