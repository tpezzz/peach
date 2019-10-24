using Tree.Api.Model.User;
using Tree.Domain.Model;
using Tree.Domain.Model.Authorization;
using Tree.Domain.Model.User;
using ExpressMapper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using AuthorizationEntity = Tree.Domain.Model.Authorization.Authorization;
using UserEntity = Tree.Domain.Model.User.User;

namespace Tree.Api.Map.CommandMap {
    public class UserCommandResultMapper : ICommandMapper<IEnumerable<Entity>, UserCommandResult> {
        public UserCommandResult Map(IEnumerable<Entity> entities) {
            var userEntity = entities.Single(x => x is UserEntity) as UserEntity;
            var authorizationEntity = entities.Single(x => x is AuthorizationEntity) as AuthorizationEntity;

            var createUserResponse = userEntity.Map<UserEntity, UserCommandResult>();
            createUserResponse.AccessCode = authorizationEntity.AccessCode;
            createUserResponse.Role = authorizationEntity.Role;
            createUserResponse.Keys = (authorizationEntity.Role == RoleType.SystemAdmin || authorizationEntity.Role == RoleType.Supervisor) ?
                new Guid[] { authorizationEntity.CompanyId } :
                (authorizationEntity as JobAuthorization).JobPermissions.Select(x => x.Job.Id).ToArray();

            return createUserResponse;
        }
    }
}