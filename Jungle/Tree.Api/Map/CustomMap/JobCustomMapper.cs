using Tree.Api.Model.Job;
using Tree.Api.Model.Site;
using Tree.Domain.Model.Authorization;
using ExpressMapper;
using ExpressMapper.Extensions;
using System;
using System.Collections.Generic;

namespace Tree.Api.Map.CustomMap {
    public class ToApiJobCustomMapper : ICustomTypeMapper<Domain.Model.Company.Job, Job> {
        public Job Map(IMappingContext<Domain.Model.Company.Job, Job> context) {
            var source = context.Source;

            return new Job {
                Id = source.Id,
                Name = source.Name,
                IsCompleted = source.IsCompleted
            };
        }
    }

    public class ToApiJobQueryResultCustomMapper : ICustomTypeMapper<Domain.Model.Company.Job, JobQueryResult> {
        public JobQueryResult Map(IMappingContext<Domain.Model.Company.Job, JobQueryResult> context) {
            var source = context.Source;

            return new JobQueryResult {
                Id = source.Id,
                Name = source.Name,
                IsCompleted = source.IsCompleted,
                Site = source.Site == null
                    ? null
                    : source.Site.Map<Domain.Model.Company.Site, Site>()
            };
        }
    }

    public class ToApiJobCommandResultCustomMapper : ICustomTypeMapper<Domain.Model.Company.Job, JobCommandResult> {
        public JobCommandResult Map(IMappingContext<Domain.Model.Company.Job, JobCommandResult> context) {
            var source = context.Source;

            return new JobCommandResult {
                Id = source.Id,
                Name = source.Name,
                IsCompleted = source.IsCompleted,
                Site = source.Site == null
                    ? null
                    : source.Site.Map<Domain.Model.Company.Site, Site>()
            };
        }
    }

    public class JobCommandToDomainJobCustomMapper : ICustomTypeMapper<JobCommand, Domain.Model.Company.Job> {
        public Domain.Model.Company.Job Map(IMappingContext<JobCommand, Domain.Model.Company.Job> context) {
            var source = context.Source;

            return new Domain.Model.Company.Job {
                Created = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                IsActive = true,
                IsCompleted = source.IsCompleted == null ? false : source.IsCompleted.Value,
                JobPermissions = new List<JobPermission>(),
                Name = source.Name,
            };
        }
    }
}