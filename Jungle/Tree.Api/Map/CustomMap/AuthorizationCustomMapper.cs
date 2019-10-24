using Tree.Api.Model.Authorization;
using ExpressMapper;
using DomainAuthorization = Tree.Domain.Model.Authorization.Authorization;

namespace Tree.Api.Map.CustomMap {
    public class ToCommandResultAuthorizationMapper : ICustomTypeMapper<DomainAuthorization, AuthorizationCommandResult> {
        public AuthorizationCommandResult Map(IMappingContext<DomainAuthorization, AuthorizationCommandResult> context) {
            var result = new AuthorizationCommandResult() {
                IsRestricted = !context.Source.IsActive,
                Role = context.Source.Role,
                UserId = context.Source.UserId,
            };

            return result;
        }
    }
}