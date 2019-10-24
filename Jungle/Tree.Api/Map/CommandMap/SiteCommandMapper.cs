using Tree.Api.Model.Site;
using Tree.App.Administration.Handler;
using ExpressMapper.Extensions;

namespace Tree.Api.Map.CommandMap {
    public class SiteCommandMapper : ICommandMapper<SiteCommand, Domain.Model.Company.Site> {
        private readonly ICompanyHandler companyHandler;
        private readonly ISiteHandler siteHandler;

        public SiteCommandMapper(ICompanyHandler companyHandler,
                                 ISiteHandler siteHandler) {
            this.companyHandler = companyHandler;
            this.siteHandler = siteHandler;
        }

        public Domain.Model.Company.Site Map(SiteCommand command) {
            if (command.Id == null) {
                return MapAsCreateCommand(command);
            }
            else {
                return MapAsUpdateCommand(command);
            }
        }

        private Domain.Model.Company.Site MapAsCreateCommand(SiteCommand command) {
            return command.Map<SiteCommand, Domain.Model.Company.Site>();
        }

        private Domain.Model.Company.Site MapAsUpdateCommand(SiteCommand command) {
            var source = siteHandler.Get(command.Id.Value, "Company", "Jobs");

            if (command.IsCompleted.HasValue && command.IsCompleted.Value) {
                foreach (var job in source.Jobs) {
                    job.IsCompleted = command.IsCompleted.Value;
                }
            }

            return new Domain.Model.Company.Site {
                Id = source.Id,
                Company = source.Company,
                Created = source.Created,
                IsActive = source.IsActive,
                IsCompleted = command.IsCompleted == null ? source.IsCompleted : command.IsCompleted.Value,
                Jobs = source.Jobs,
                Name = command.Name == null ? source.Name : command.Name
            };
        }
    }
}