using Tree.Api.Model.Company;
using Tree.App.Administration.Generator;
using ExpressMapper;
using System;

namespace Tree.Api.Map.CustomMap {
    public class FromDomainToCompanyQueryResultCustomMapper : ICustomTypeMapper<Domain.Model.Company.Company, CompanyQueryResult> {
        public Model.Company.CompanyQueryResult Map(
            IMappingContext<Domain.Model.Company.Company, Model.Company.CompanyQueryResult> context) {
            var source = context.Source;

            return new Model.Company.CompanyQueryResult {
                City = source.City,
                Address = source.Address,
                CompanyId = source.CompanyId,
                CompanyName = source.CompanyName,
                Country = source.Country,
                Id = source.Id,
                Phone = source.Phone,
                Province = source.Province,
                ZipCode = source.ZipCode
            };
        }
    }

    public class ToDomainCompanyCustomMapper : ICustomTypeMapper<CompanyCommand, Domain.Model.Company.Company> {
        public Domain.Model.Company.Company Map(
            IMappingContext<CompanyCommand, Domain.Model.Company.Company> context) {
            var source = context.Source;

            return new Domain.Model.Company.Company {
                Id = source.Id != null ? (Guid)source.Id : Guid.NewGuid(),
                Created = DateTimeOffset.UtcNow,
                CompanyName = source.CompanyName,
                Address = source.Address,
                Province = source.Province,
                City = source.City,
                CompanyId = new IdGenerator().Generate(),
                Country = source.Country,
                ZipCode = source.ZipCode,
                Phone = source.Phone,
                IsActive = true
            };
        }
    }

    public class FromDomainToCompanyCommandResultCustomMapper : ICustomTypeMapper<Domain.Model.Company.Company, CompanyCommandResult> {
        public CompanyCommandResult Map(
            IMappingContext<Domain.Model.Company.Company, CompanyCommandResult> context) {
            var source = context.Source;

            return new CompanyCommandResult {
                City = source.City,
                Address = source.Address,
                CompanyId = source.CompanyId,
                CompanyName = source.CompanyName,
                Country = source.Country,
                Phone = source.Phone,
                Province = source.Province,
                ZipCode = source.ZipCode,
                Id = source.Id
            };
        }
    }
}