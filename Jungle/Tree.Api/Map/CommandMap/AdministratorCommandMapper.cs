using Tree.Api.Model.Company;
using Tree.App.Administration.Handler;
using Tree.App.Core.Exception;
using ExpressMapper.Extensions;

namespace Tree.Api.Map.CommandMap {
    public class AdministratorCommandMapper : ICommandMapper<AdministratorCommand, Domain.Model.User.Administrator> {
        private readonly IAdministratorHandler administratorHandler;

        public AdministratorCommandMapper(IAdministratorHandler administratorHandler) {
            this.administratorHandler = administratorHandler;
        }

        public Domain.Model.User.Administrator Map(AdministratorCommand command) {
            if (command.Id == null) {
                return MapAsCreateCommand(command);
            }
            else {
                return MapAsUpdateCommand(command);
            }
        }

        private Domain.Model.User.Administrator MapAsCreateCommand(AdministratorCommand command) {
            var domainAdministrator = command.Map<AdministratorCommand, Domain.Model.User.Administrator>();
            return domainAdministrator;
        }

        private Domain.Model.User.Administrator MapAsUpdateCommand(AdministratorCommand command) {
            var source = administratorHandler.Get(command.Id.Value, "User");

            if ((source.User.Email != command.Email && command.Email != null) || (source.User.Name != command.Name && command.Name != null))
                throw new AppValidationException("Validation", "Fields Name and Email cannot be updated.");

            source.Address = command.Address;
            source.City = command.City;
            source.Country = command.Country;
            source.Province = command.Province;
            source.Phone = command.Phone;
            source.ZipCode = command.ZipCode;

            return source;
        }
    }
}