using Tree.Api.Model.Claims;
using Tree.Api.Model.Report;
using Tree.App.Measurement;
using ExpressMapper.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web.Http;

namespace Tree.Api.Controller {
    public class ReportController : ApiController {
        private readonly IMeasurementHandler measurementHandler;

        public ReportController(IMeasurementHandler measurementHandler) {
            this.measurementHandler = measurementHandler;
        }

        [Authorize]
        public IHttpActionResult Get([FromUri] ReportQuery query) {
            var authorizationClaims = User.Identity.Map<IIdentity, AuthorizationClaims>();

            var authorizedGroupedMeasurements = measurementHandler.Get(authorizationClaims.CompanyId, authorizationClaims.Id, authorizationClaims.Role, query.JobId).GroupBy(x => x.Job);

            //temporary filtering to keep old response schema
            var nonGroupedMeasurements = new List<Domain.Model.Measurement.Measurement>();
            foreach (var group in authorizedGroupedMeasurements)
                foreach (var measurementInGroup in group)
                    nonGroupedMeasurements.Add(measurementInGroup);

            var filteredMeasurements = nonGroupedMeasurements.
                                       Where(x => query.ConnectionId == null || x.Connection.SerialNumber == query.ConnectionId). //TODO: is this correct?
                                       Where(x => query.JobId == null || x.Job.Id == query.JobId).
                                       Where(x => query.SiteId == null || x.Job.Site.Id == query.SiteId).
                                       Where(x => string.IsNullOrEmpty(query.Location) || x.CrimpLocation == query.Location).
                                       Where(x => query.From == null || x.Created >= query.From).
                                       Where(x => query.To == null || x.Created <= query.To).ToList();
            //TODO: Pagination, Field "OwnInspection"

            var response = new ReportResult {
                Measurements = filteredMeasurements.Select(x => x.Map<Domain.Model.Measurement.Measurement, Api.Model.Report.Measurement>()),
                Pagination = new Pagination {
                    Current = 1,
                    Size = 1
                }
            };
            return Ok(response);
        }
    }
}