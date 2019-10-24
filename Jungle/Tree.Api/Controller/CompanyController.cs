using Tree.Api.Filter;
using Tree.Api.Map.CommandMap;
using Tree.Api.Model.Claims;
using Tree.Api.Model.Company;
using Tree.App.Administration.Handler;
using Tree.App.Authorization.Handler;
using Tree.App.Core.Exception;
using Tree.Domain.Model;
using Tree.Domain.Model.User;
using Tree.Domain.Validation;
using ExpressMapper.Extensions;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http;

namespace Tree.Api.Controller {
    [DisableODataActionFilter]
    public class CompanyController : ApiController {
        private readonly ICompanyHandler companyHandler;
        private readonly IAdministratorHandler administratorHandler;
        private readonly ICompanyAuthorizationHandler companyAuthorizationHandler;
        private readonly ICommandMapper<CompanyCommand, Domain.Model.Company.Company> companyCommandMapper;
        private readonly ICommandMapper<AdministratorCommand, Domain.Model.User.Administrator> administratorCommandMapper;
        private readonly ICommandMapper<IEnumerable<Entity>, CompanyCommandResult> companyCommandResultMapper;
        private readonly IValidatorFactory validatorFactory;

        public CompanyController(ICompanyHandler companyHandler,
                                 IAdministratorHandler administratorHandler,
                                 ICompanyAuthorizationHandler companyAuthorizationHandler,
                                 ICommandMapper<CompanyCommand, Domain.Model.Company.Company> companyCommandMapper,
                                 ICommandMapper<AdministratorCommand, Domain.Model.User.Administrator> administratorCommandMapper,
                                 ICommandMapper<IEnumerable<Entity>, CompanyCommandResult> companyCommandResultMapper,
                                 IValidatorFactory validatorFactory) {
            this.companyHandler = companyHandler;
            this.administratorHandler = administratorHandler;
            this.companyAuthorizationHandler = companyAuthorizationHandler;
            this.companyCommandMapper = companyCommandMapper;
            this.administratorCommandMapper = administratorCommandMapper;
            this.companyCommandResultMapper = companyCommandResultMapper;
            this.validatorFactory = validatorFactory;
        }

        [Authorize(Roles = "SystemAdmin,Supervisor")]
        public IHttpActionResult Get() {
            var author = User.Identity.Map<IIdentity, AuthorizationClaims>();

            var company = companyHandler.Get(author.CompanyId);
            var administrators = administratorHandler.GetByCompanyId(author.CompanyId, false, "User");

            var result = company.Map<Domain.Model.Company.Company, CompanyQueryResult>();
            result.Administrators = administrators.Select(x => x.Map<Domain.Model.User.Administrator, AdministratorQueryResult>());

            return Ok(new CompanyQueryResult[] { result }.AsEnumerable());
        }

        [Authorize(Roles = "SystemAdmin")]
        public async Task<IHttpActionResult> Post([FromBody] CompanyCommand company) {
            var author = User.Identity.Map<IIdentity, AuthorizationClaims>();

            var validationResult = await validatorFactory.Validate(company);
            if (!validationResult.IsValid) {
                throw new AppValidationException(validationResult.Errors);
            }
            if (author.CompanyId != company.Id)
                throw new PermissionException("Permission", "You cannot edit this data.");

            var domainCompany = companyCommandMapper.Map(company);

            var domainAdmins = company.Administrators.Select(x => administratorCommandMapper.Map(x));

            var result = await UpdateCompany(company, domainCompany, domainAdmins);

            return Ok(result);
        }

        private async Task<CompanyCommandResult> UpdateCompany(CompanyCommand company, Domain.Model.Company.Company domainCompany, IEnumerable<Domain.Model.User.Administrator> domainAdmins) {
            await companyHandler.UpdateAsync(domainCompany);
            var newAdmins = await administratorHandler.SaveAsync(domainAdmins);

            var domainAuthorizations = await companyAuthorizationHandler.GrantManyAuthorizationsAsync(domainCompany, newAdmins.Select(x => x.User), RoleType.SystemAdmin);

            var entityList = GetMapArgument(domainCompany, domainAdmins, domainAuthorizations);
            var updateResult = companyCommandResultMapper.Map(entityList);

            return updateResult;
        }

        private static List<Entity> GetMapArgument(Domain.Model.Company.Company domainCompany, IEnumerable<Domain.Model.User.Administrator> domainAdmins, IEnumerable<Domain.Model.Authorization.CompanyAuthorization> domainAuthorizations) {
            var entityList = new List<Entity>();
            entityList.AddRange(domainAdmins);
            entityList.AddRange(domainAuthorizations);
            entityList.Add(domainCompany);

            return entityList;
        }
    }
}