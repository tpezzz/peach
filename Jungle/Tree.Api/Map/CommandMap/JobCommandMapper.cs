using Tree.Api.Model.Job;
using Tree.App.Administration.Handler;
using ExpressMapper.Extensions;

namespace Tree.Api.Map.CommandMap {
    public class JobCommandMapper : ICommandMapper<JobCommand, Domain.Model.Company.Job> {
        private readonly IJobHandler jobHandler;
        private readonly ISiteHandler siteHandler;

        public JobCommandMapper(IJobHandler jobHandler,
                                ISiteHandler siteHandler) {
            this.siteHandler = siteHandler;
            this.jobHandler = jobHandler;
        }

        public Domain.Model.Company.Job Map(JobCommand command) {
            if (command.Id == null) {
                return MapAsCreateCommand(command);
            }
            else {
                return MapAsUpdateCommand(command);
            }
        }

        private Domain.Model.Company.Job MapAsCreateCommand(JobCommand command) {
            var domainJob = command.Map<JobCommand, Domain.Model.Company.Job>();
            if (command.SiteId != null) {
                domainJob.Site = siteHandler.Get(command.SiteId.Value, "Company", "Jobs");
            }
            return domainJob;
        }

        private Domain.Model.Company.Job MapAsUpdateCommand(JobCommand command) {
            var source = jobHandler.Get(command.Id.Value, "Site.Company", "Site.Jobs", "JobPermissions");

            var job = new Domain.Model.Company.Job {
                Id = source.Id,
                JobPermissions = source.JobPermissions,
                IsActive = source.IsActive,
                Created = source.Created,
                IsCompleted = command.IsCompleted == null
                            ? source.IsCompleted
                            : command.IsCompleted.Value,
                Name = string.IsNullOrWhiteSpace(command.Name)
                            ? source.Name
                            : command.Name,
                Site = command.SiteId == null
                     ? source.Site
                     : siteHandler.Get(command.SiteId.Value, "Company", "Jobs")
            };

            if (command.IsCompleted.HasValue && !command.IsCompleted.Value) {
                job.Site.IsCompleted = command.IsCompleted.Value;
            }

            return job;
        }
    }
}