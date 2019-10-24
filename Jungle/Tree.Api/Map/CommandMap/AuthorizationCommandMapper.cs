using Tree.Api.Model.Authorization;
using Tree.App.Authorization.Handler;
using Tree.App.Core.Exception;
using DomainAuthorization = Tree.Domain.Model.Authorization.Authorization;

namespace Tree.Api.Map.CommandMap {
    public class AuthorizationCommandMapper : ICommandMapper<AuthorizationCommand, DomainAuthorization> {
        private readonly IAuthorizationHandler authorizationHandler;

        public AuthorizationCommandMapper(IAuthorizationHandler authorizationHandler) {
            this.authorizationHandler = authorizationHandler;
        }

        public DomainAuthorization Map(AuthorizationCommand command) {
            var existingAuthorization = authorizationHandler.GetAuthorizationByUserId(command.UserId);
            if (existingAuthorization == null) {
                throw new AppValidationException("UserId", "There is no existing authorization for provided user id");
            }

            existingAuthorization.IsActive = command.IsRestricted.HasValue ? !command.IsRestricted.Value : existingAuthorization.IsActive;

            return existingAuthorization;
        }
    }
}