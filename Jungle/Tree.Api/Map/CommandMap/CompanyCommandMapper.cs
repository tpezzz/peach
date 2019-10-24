using Tree.Api.Model.Company;
using Tree.App.Administration.Handler;
using ExpressMapper.Extensions;

namespace Tree.Api.Map.CommandMap {
    public class CompanyCommandMapper : ICommandMapper<CompanyCommand, Domain.Model.Company.Company> {
        private readonly ICompanyHandler companyHandler;

        public CompanyCommandMapper(ICompanyHandler companyHandler) {
            this.companyHandler = companyHandler;
        }

        public Domain.Model.Company.Company Map(CompanyCommand command) {
            if (command.Id == null) {
                return MapAsCreateCommand(command);
            }
            else {
                return MapAsUpdateCommand(command);
            }
        }

        private Domain.Model.Company.Company MapAsCreateCommand(CompanyCommand command) {
            var domainCompany = command.Map<CompanyCommand, Domain.Model.Company.Company>();
            return domainCompany;
        }

        private Domain.Model.Company.Company MapAsUpdateCommand(CompanyCommand command) {
            var source = companyHandler.Get(command.Id.Value);

            source.Address = command.Address;
            source.City = command.City;
            source.CompanyName = command.CompanyName;
            source.Country = command.Country;
            source.Phone = command.Phone;
            source.Province = command.Province;
            source.ZipCode = command.ZipCode;

            return source;
        }
    }
}