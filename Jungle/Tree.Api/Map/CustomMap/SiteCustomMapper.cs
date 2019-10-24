using Tree.Api.Model.Job;
using Tree.Api.Model.Site;
using Tree.Domain.Comparer;
using Tree.Domain.Model.Authorization;
using ExpressMapper;
using ExpressMapper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tree.Api.Map.CustomMap {
    public class ToApiSiteCustomMapper : ICustomTypeMapper<Domain.Model.Company.Site, Site> {
        public Site Map(IMappingContext<Domain.Model.Company.Site, Site> context) {
            var source = context.Source;

            return new Site {
                Id = source.Id,
                Name = source.Name,
                IsCompleted = source.IsCompleted
            };
        }
    }

    public class ToApiSiteQueryResponseCustomMapper : ICustomTypeMapper<Domain.Model.Company.Site, SiteQueryResult> {
        public SiteQueryResult Map(IMappingContext<Domain.Model.Company.Site, SiteQueryResult> context) {
            var source = context.Source;

            return new SiteQueryResult {
                Id = source.Id,
                Name = source.Name,
                IsCompleted = source.IsCompleted,
                Jobs = source.Jobs.Select(x => x.Map<Domain.Model.Company.Job, Job>()).ToList()
            };
        }
    }

    public class SiteCommandToDomainSiteCustomMapper : ICustomTypeMapper<SiteCommand, Domain.Model.Company.Site> {
        public Domain.Model.Company.Site Map(IMappingContext<SiteCommand, Domain.Model.Company.Site> context) {
            var source = context.Source;

            return new Domain.Model.Company.Site {
                Created = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                IsActive = true,
                IsCompleted = source.IsCompleted == null ? false : source.IsCompleted.Value,
                Name = source.Name,
                Company = null,
                Jobs = new List<Domain.Model.Company.Job>()
            };
        }
    }

    public class ToApiSiteListQueryResponseCustomMapper : ICustomTypeMapper<IEnumerable<JobPermission>, IEnumerable<SiteQueryResult>> {
        public IEnumerable<SiteQueryResult> Map(IMappingContext<IEnumerable<JobPermission>, IEnumerable<SiteQueryResult>> context) {
            var source = context.Source.Where(y => y.IsActive);
            var sites = source.Select(x => x.Job.Site).Distinct(new EntityIdEqualityComparer<Domain.Model.Company.Site>());

            return sites.Select(x =>
                new SiteQueryResult {
                    Id = x.Id,
                    Name = x.Name,
                    IsCompleted = x.IsCompleted,
                    Jobs = source.Where(a => a.Job.Site.Id == x.Id).Select(y => y.Job.Map<Domain.Model.Company.Job, Job>()).ToList()
                });
        }
    }
}