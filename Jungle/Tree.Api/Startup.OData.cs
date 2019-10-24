using Tree.Api.Model.Job;
using Tree.Api.Model.Report;
using Tree.Api.Model.Site;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;

namespace Tree.Api {
    internal static class ODataConfiguration {
        internal static void ConfigureOData(this HttpConfiguration config) {
            var odataBuilder = new ODataConventionModelBuilder();

            config.Count().Filter().OrderBy().Expand().Select().MaxTop(null);

            var measurement = odataBuilder.EntitySet<Measurement>("Measurement");
            measurement.EntityType.HasKey(x => x.Id);
            measurement.EntityType.Expand().Filter().Select().Count().OrderBy().Page();

            var company = odataBuilder.EntitySet<Company>("Company");
            company.EntityType.HasKey(x => x.Id);

            var job = odataBuilder.EntitySet<JobQueryResult>("Job");
            job.EntityType.HasKey(x => x.Id);

            var site = odataBuilder.EntitySet<SiteQueryResult>("Site");
            site.EntityType.HasKey(x => x.Id);

            var user = odataBuilder.EntitySet<User>("User");
            user.EntityType.HasKey(x => x.Id);

            measurement.HasRequiredBinding(x => x.Company, company);
            measurement.HasRequiredBinding(x => x.Job, job);
            measurement.HasRequiredBinding(x => x.Site, site);
            measurement.HasRequiredBinding(x => x.CreatedBy, user);
            measurement.HasRequiredBinding(x => x.UpdatedBy, user);

            config.MapODataServiceRoute("odata", "odata", odataBuilder.GetEdmModel());
        }
    }
}