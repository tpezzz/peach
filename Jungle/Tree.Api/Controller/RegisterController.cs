using Tree.Api.Map.CommandMap;
using Tree.Api.Model.Company;
using Tree.App.Administration.Handler;
using Tree.App.Authorization.Handler;
using Tree.App.Core.Exception;
using Tree.Domain.Model;
using Tree.Domain.Model.User;
using Tree.Domain.Validation;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Tree.Api.Controller {
    [AllowAnonymous]
    public class RegisterController : ApiController {
        private readonly ICompanyHandler companyHandler;
        private readonly IJobHandler jobHandler;
        private readonly ISiteHandler siteHandler;
        private readonly IAdministratorHandler administratorHandler;
        private readonly ICompanyAuthorizationHandler companyAuthorizationHandler;
        private readonly ICommandMapper<CompanyCommand, Domain.Model.Company.Company> companyCommandMapper;
        private readonly ICommandMapper<AdministratorCommand, Domain.Model.User.Administrator> administratorCommandMapper;
        private readonly ICommandMapper<IEnumerable<Entity>, CompanyCommandResult> companyCommandResultMapper;
        private readonly IValidatorFactory validatorFactory;

        public RegisterController(ICompanyHandler companyHandler,
                                 IJobHandler jobHandler,
                                 ISiteHandler siteHandler,
                                 IAdministratorHandler administratorHandler,
                                 ICompanyAuthorizationHandler companyAuthorizationHandler,
                                 ICommandMapper<CompanyCommand, Domain.Model.Company.Company> companyCommandMapper,
                                 ICommandMapper<AdministratorCommand, Domain.Model.User.Administrator> administratorCommandMapper,
                                 ICommandMapper<IEnumerable<Entity>, CompanyCommandResult> companyCommandResultMapper,
                                 IValidatorFactory validatorFactory) {
            this.companyHandler = companyHandler;
            this.jobHandler = jobHandler;
            this.siteHandler = siteHandler;
            this.administratorHandler = administratorHandler;
            this.companyAuthorizationHandler = companyAuthorizationHandler;
            this.companyCommandMapper = companyCommandMapper;
            this.administratorCommandMapper = administratorCommandMapper;
            this.companyCommandResultMapper = companyCommandResultMapper;
            this.validatorFactory = validatorFactory;
        }

        public async Task<IHttpActionResult> Post([FromBody] CompanyCommand company) {
            var validationResult = await validatorFactory.Validate(company);
            if (!validationResult.IsValid) {
                throw new AppValidationException(validationResult.Errors);
            }

            var domainCompany = companyCommandMapper.Map(company);

            var domainAdmins = company.Administrators.Select(x => administratorCommandMapper.Map(x)).ToList();

            var result = await CreateCompany(company, domainCompany, domainAdmins);

            return Ok(result);
        }

        private async Task<CompanyCommandResult> CreateCompany(CompanyCommand company, Domain.Model.Company.Company domainCompany, IEnumerable<Domain.Model.User.Administrator> domainAdmins) {
            var companyId = await companyHandler.CreateAsync(domainCompany);
            await Task.WhenAll(domainAdmins.Select(async x => await administratorHandler.CreateAsync(x)));

            var defaultSite = await siteHandler.CreateDefaultAsync(domainCompany);
            await jobHandler.CreateDefaultAsync(defaultSite);

            var domainAuthorizations = await Task.WhenAll(domainAdmins
                .Select(async x => await companyAuthorizationHandler.GrantAuthorizationAsync(domainCompany, x.User, RoleType.SystemAdmin)));

            var entityList = GetMapArgument(domainCompany, domainAdmins, domainAuthorizations);
            var result = companyCommandResultMapper.Map(entityList);

            return result;
        }

        private static List<Entity> GetMapArgument(Domain.Model.Company.Company domainCompany, IEnumerable<Domain.Model.User.Administrator> domainAdmins, Domain.Model.Authorization.CompanyAuthorization[] domainAuthorizations) {
            var entityList = new List<Entity>();
            entityList.AddRange(domainAdmins);
            entityList.AddRange(domainAuthorizations);
            entityList.Add(domainCompany);

            return entityList;
        }
    }
}