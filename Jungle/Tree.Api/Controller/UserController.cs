using Tree.Api.Filter;
using Tree.Api.Map.CommandMap;
using Tree.Api.Map.QueryMap;
using Tree.Api.Model.Authorization;
using Tree.Api.Model.Claims;
using Tree.Api.Model.Site;
using Tree.Api.Model.User;
using Tree.App.Administration.Handler;
using Tree.App.Authorization.Handler;
using Tree.Domain.Model;
using Tree.Domain.Model.Authorization;
using Tree.Domain.Model.User;
using ExpressMapper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http;
using AuthorizationEntity = Tree.Domain.Model.Authorization.Authorization;
using DomainEntity = Tree.Domain.Model.Entity;
using SiteEntity = Tree.Domain.Model.Company.Site;
using UserEntity = Tree.Domain.Model.User.User;

namespace Tree.Api.Controller {
    [DisableODataActionFilter]
    public class UserController : ApiController {
        private readonly IUserHandler userHandler;
        private readonly ICompanyHandler companyHandler;
        private readonly IJobHandler jobHandler;
        private readonly ISiteHandler siteHandler;
        private readonly ICompanyAuthorizationHandler companyAuthorizationHandler;
        private readonly IJobAuthorizationHandler jobAuthorizationHandler;
        private readonly IQueryFilter<IEnumerable<UserQueryResult>, UserQuery> filter;
        private readonly ICommandMapper<IEnumerable<Entity>, UserCommandResult> resultMapper;

        public UserController(IUserHandler userHandler,
            ICompanyHandler companyHandler,
            IJobHandler jobHandler,
            ISiteHandler siteHandler,
            ICompanyAuthorizationHandler companyAuthorizationHandler,
            IJobAuthorizationHandler jobAuthorizationHandler,
            IQueryFilter<IEnumerable<UserQueryResult>, UserQuery> filter,
            ICommandMapper<IEnumerable<Entity>, UserCommandResult> resultMapper) {
            this.userHandler = userHandler;
            this.companyHandler = companyHandler;
            this.jobHandler = jobHandler;
            this.siteHandler = siteHandler;
            this.companyAuthorizationHandler = companyAuthorizationHandler;
            this.jobAuthorizationHandler = jobAuthorizationHandler;
            this.filter = filter;
            this.resultMapper = resultMapper;
        }

        [Authorize(Roles = "SystemAdmin,Supervisor")]
        public IHttpActionResult Get([FromUri] UserQuery query) {
            var authorization = User.Identity.Map<IIdentity, AuthorizationClaims>();
            query.CompanyId = query.CompanyId.HasValue ? query.CompanyId.Value : authorization.CompanyId;

            var companysites = siteHandler.Get(query.Map<UserQuery, Expression<Func<SiteEntity, bool>>>(), "Jobs");
            var companyAuthorizations = companyAuthorizationHandler.Get(query.Map<UserQuery, Expression<Func<CompanyAuthorization, bool>>>(), true, "User");
            var jobAuthorizations = jobAuthorizationHandler.Get(query.Map<UserQuery, Expression<Func<JobAuthorization, bool>>>(), true, "JobPermissions", "User");

            var resultsFromCompanyAuth = companyAuthorizations.Select(x => {
                var mapresult = (x as AuthorizationEntity).Map<AuthorizationEntity, UserQueryResult>();
                mapresult.Sites = companysites.Select(a => a.Map<SiteEntity, SiteQueryResult>());
                return mapresult;
            });

            var resultsFromJobAuth = jobAuthorizations.Select(x => {
                var mapresult = (x as AuthorizationEntity).Map<AuthorizationEntity, UserQueryResult>();
                mapresult.Sites = x.JobPermissions.Map<IEnumerable<JobPermission>, IEnumerable<SiteQueryResult>>();
                return mapresult;
            });

            var allResults = resultsFromCompanyAuth.Concat(resultsFromJobAuth);
            var filteredResults = filter.Run(allResults, query);

            return Ok(filteredResults);
        }

        [Authorize(Roles = "SystemAdmin,Supervisor")]
        public async Task<IHttpActionResult> Post(UserCommand user) {
            AuthorizationEntity authorization = null;
            var keys = Enumerable.Empty<Guid>();
            var authType = (user.Role == RoleType.SystemAdmin || user.Role == RoleType.Supervisor) ?
                AuthorizationType.Company :
                AuthorizationType.Job;

            var userEntity = user.Map<UserCommand, UserEntity>();
            userHandler.ValidateUserUniqueInCompany(userEntity, user.Keys, authType.Equals(AuthorizationType.Company));
            await userHandler.CreateAsync(userEntity);

            if (authType.Equals(AuthorizationType.Company)) {
                var companyEntity = companyHandler.Get(user.Keys.FirstOrDefault());
                authorization = await companyAuthorizationHandler.GrantAuthorizationAsync(companyEntity, userEntity, user.Role);
            }
            else if (authType.Equals(AuthorizationType.Job)) {
                var jobs = jobHandler.Get(user.Keys, "Site", "Site.Company");
                authorization = await jobAuthorizationHandler.GrantAuthorizationAsync(jobs, userEntity, user.Role);
            }

            var createUserResponse = resultMapper.Map(new DomainEntity[] { userEntity, authorization });
            return Ok(createUserResponse);
        }
    }
}