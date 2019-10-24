using Tree.Api.Model.User;
using Tree.Domain.Model.User;
using ExpressMapper;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Tree.Api.Map.CustomMap {
    public class ToDomainUserCustomMapper : ICustomTypeMapper<UserCommand, Domain.Model.User.User> {
        public Domain.Model.User.User Map(IMappingContext<UserCommand, Domain.Model.User.User> context) {
            var creationDate = DateTimeOffset.UtcNow;
            return new Domain.Model.User.User {
                Created = creationDate,
                Updated = creationDate,
                Email = context.Source.Email,
                Name = context.Source.Name,
                Id = Guid.NewGuid(),
                IsActive = true
            };
        }
    }

    public class ToApiUserCustomMapper : ICustomTypeMapper<Domain.Model.Authorization.Authorization, UserQueryResult> {
        public UserQueryResult Map(IMappingContext<Domain.Model.Authorization.Authorization, UserQueryResult> context) {
            return new UserQueryResult() {
                Id = context.Source.User.Id,
                Name = context.Source.User.Name,
                Role = context.Source.Role,
                Email = context.Source.User.Email,
                Updated = context.Source.User.Updated,
                IsRestricted = !context.Source.IsActive
            };
        }
    }

    public class ToCreateUserResponse : ICustomTypeMapper<Domain.Model.User.User, UserCommandResult> {
        public UserCommandResult Map(IMappingContext<Domain.Model.User.User, UserCommandResult> context) {
            return new UserCommandResult() {
                Id = context.Source.Id,
                Name = context.Source.Name,
                Email = context.Source.Email,
                Updated = context.Source.Updated
            };
        }
    }

    public class ToCompanyAuthorizationWhereExpressionMapper : ICustomTypeMapper<UserQuery, Expression<Func<Domain.Model.Authorization.CompanyAuthorization, bool>>> {
        public Expression<Func<Domain.Model.Authorization.CompanyAuthorization, bool>> Map(IMappingContext<UserQuery, Expression<Func<Domain.Model.Authorization.CompanyAuthorization, bool>>> context) {
            var source = context.Source;
            source.Roles = source.Roles ?? Enumerable.Empty<RoleType>();

            return x =>
                (string.IsNullOrEmpty(source.Name) || x.User.Name.Contains(source.Name)) &&
                (string.IsNullOrEmpty(source.Email) || x.User.Email.Contains(source.Email)) &&
                (!source.Id.HasValue || x.User.Id == source.Id) &&
                (source.Roles.Count() == 0 || source.Roles.Contains(x.Role)) &&
                (source.IncludeRestricted.HasValue && source.IncludeRestricted.Value || x.IsActive == true) &&
                (!source.CompanyId.HasValue || x.CompanyId == source.CompanyId.Value) &&
                x.User.IsActive
                ;
        }

    }

    public class ToJobAuthorizationWhereExpressionMapper : ICustomTypeMapper<UserQuery, Expression<Func<Domain.Model.Authorization.JobAuthorization, bool>>> {
        public Expression<Func<Domain.Model.Authorization.JobAuthorization, bool>> Map(IMappingContext<UserQuery, Expression<Func<Domain.Model.Authorization.JobAuthorization, bool>>> context) {
            var source = context.Source;
            source.Roles = source.Roles ?? Enumerable.Empty<RoleType>();

            return x =>
                (string.IsNullOrEmpty(source.Name) || x.User.Name.Contains(source.Name)) &&
                (string.IsNullOrEmpty(source.Email) || x.User.Email.Contains(source.Email)) &&
                (!source.Id.HasValue || x.User.Id == source.Id) &&
                (source.Roles.Count() == 0 || source.Roles.Contains(x.Role)) &&
                (source.IncludeRestricted.HasValue && source.IncludeRestricted.Value || x.IsActive == true) &&
                (!source.CompanyId.HasValue || x.CompanyId == source.CompanyId.Value) &&
                x.User.IsActive
                ;
        }

    }

    public class ToJobWhereExpressionMapper : ICustomTypeMapper<UserQuery, Expression<Func<Domain.Model.Company.Site, bool>>> {
        public Expression<Func<Domain.Model.Company.Site, bool>> Map(IMappingContext<UserQuery, Expression<Func<Domain.Model.Company.Site, bool>>> context) {
            var source = context.Source;
            return x =>
                x.Company.Id == source.CompanyId.Value &&
                x.IsActive
                ;
        }
    }
}