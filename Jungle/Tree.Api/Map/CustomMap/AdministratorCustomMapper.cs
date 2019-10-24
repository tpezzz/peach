using Tree.Api.Model.Company;
using Tree.Domain.Model.User;
using ExpressMapper;
using System;

namespace Tree.Api.Map.CustomMap {
    public class ToAdministratorQueryResponse : ICustomTypeMapper<Domain.Model.User.Administrator, AdministratorQueryResult> {
        public AdministratorQueryResult Map(
            IMappingContext<Domain.Model.User.Administrator, AdministratorQueryResult> context) {
            var source = context.Source;

            return new AdministratorQueryResult {
                Id = source.Id,
                Email = source.User.Email,
                Address = source.Address,
                City = source.City,
                Country = source.Country,
                Province = source.Province,
                Name = source.User.Name,
                Phone = source.Phone,
                ZipCode = source.ZipCode
            };
        }
    }

    public class ToDomainAdminCustomMapper : ICustomTypeMapper<AdministratorCommand, Domain.Model.User.Administrator> {
        public Domain.Model.User.Administrator Map(
            IMappingContext<AdministratorCommand, Domain.Model.User.Administrator> context) {
            var source = context.Source;

            var creationDate = DateTimeOffset.UtcNow;
            var user = new User {
                Updated = creationDate,
                Created = creationDate,
                Email = source.Email,
                IsActive = true,
                Id = Guid.NewGuid(),
                Name = source.Name,
            };

            return new Domain.Model.User.Administrator {
                Id = user.Id,
                Created = creationDate,
                Address = source.Address,
                City = source.City,
                ZipCode = source.ZipCode,
                Country = source.Country,
                Phone = source.Phone,
                Province = source.Province,
                IsActive = true,
                User = user
            };
        }
    }

    public class FromDomainToAdministratorCommandResult : ICustomTypeMapper<Domain.Model.User.Administrator, AdministratorCommandResult> {
        public AdministratorCommandResult Map(
            IMappingContext<Domain.Model.User.Administrator, AdministratorCommandResult> context) {
            var source = context.Source;

            return new AdministratorCommandResult {
                Id = source.Id,
                Email = source.User.Email,
                Address = source.Address,
                City = source.City,
                Country = source.Country,
                Province = source.Province,
                Name = source.User.Name,
                Phone = source.Phone,
                ZipCode = source.ZipCode
            };
        }
    }
}