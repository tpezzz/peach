using Tree.Api.Map.CommandMap;
using Tree.Api.Model.Authorization;
using Tree.Api.Model.Claims;
using Tree.App.Authorization.Handler;
using Tree.Domain.Model.User;
using ExpressMapper.Extensions;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http;
using DomainAuthorization = Tree.Domain.Model.Authorization.Authorization;

namespace Tree.Api.Controller {
    public class AuthorizationController : ApiController {
        private readonly IAuthorizationHandler authorizationHandler;
        private readonly IJobPermissionHandler jobPermissionHandler;
        private readonly ICommandMapper<AuthorizationCommand, DomainAuthorization> commandMapper;

        public AuthorizationController(
            IAuthorizationHandler authorizationHandler,
            IJobPermissionHandler jobPermissionHandler,
            ICommandMapper<AuthorizationCommand, DomainAuthorization> commandMapper) {
            this.authorizationHandler = authorizationHandler;
            this.jobPermissionHandler = jobPermissionHandler;
            this.commandMapper = commandMapper;
        }

        [Authorize]
        public IHttpActionResult Get() {
            var result = User.Identity.Map<IIdentity, AuthorizationClaims>();
            return Ok(result);
        }

        [Authorize(Roles = "SystemAdmin,Supervisor")]
        public async Task<IHttpActionResult> Post([FromBody] AuthorizationCommand command) {
            var author = User.Identity.Map<IIdentity, AuthorizationClaims>();

            var domainAuthorization = commandMapper.Map(command);
            var userAuthorization = await authorizationHandler.Update(author.Id, author.Role, author.CompanyId, domainAuthorization);
            var keys = jobPermissionHandler.Update(domainAuthorization, command.Keys);

            var result = userAuthorization.Map<DomainAuthorization, AuthorizationCommandResult>();
            result.Keys = keys;

            return Ok(result);
        }
    }
}