using Tree.Api.Model.Company;
using Tree.Domain.Model;
using Tree.Domain.Model.Authorization;
using ExpressMapper.Extensions;
using System.Collections.Generic;
using System.Linq;
using AdministratorEntity = Tree.Domain.Model.User.Administrator;
using CompanyEntity = Tree.Domain.Model.Company.Company;

namespace Tree.Api.Map.CommandMap {
    public class CompanyCommandResultMapper : ICommandMapper<IEnumerable<Entity>, CompanyCommandResult> {
        public CompanyCommandResult Map(IEnumerable<Entity> entities) {
            var companyEntity = entities.Single(x => x is CompanyEntity) as CompanyEntity;

            var administratorEntities = entities.Where(x => x is AdministratorEntity).Select(x => { return x as AdministratorEntity; });
            var administratorAuthorizations = entities.Where(x => x is CompanyAuthorization).Select(x => { return x as CompanyAuthorization; });

            var companyResponse = companyEntity.Map<CompanyEntity, CompanyCommandResult>();

            companyResponse.Administrators = administratorEntities.Select(x => {
                var adminResponse = x.Map<AdministratorEntity, AdministratorCommandResult>();
                var matchingAuthorization = administratorAuthorizations.FirstOrDefault(y => y.UserId == x.User.Id);
                adminResponse.AccessCode = matchingAuthorization != null ? matchingAuthorization.AccessCode : string.Empty;
                return adminResponse;
            });

            return companyResponse;
        }
    }
}