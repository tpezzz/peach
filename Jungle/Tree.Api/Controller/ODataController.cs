using Tree.Api.Model.Claims;
using Tree.App.Measurement;
using ExpressMapper.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;

namespace Tree.Api.Controller {
    [ODataRoutePrefix("Measurement")]
    public class ODataController : System.Web.OData.ODataController {
        private readonly IMeasurementHandler measurementHandler;

        public ODataController(IMeasurementHandler measurementHandler) {
            this.measurementHandler = measurementHandler;
        }

        [ODataRoute]
        [EnableQuery]
        [Authorize]
        public IQueryable<Model.Report.Measurement> Get() {
            var claims = User.Identity.Map<IIdentity, AuthorizationClaims>();

            var measurements = measurementHandler.Get(claims.CompanyId, claims.Id, claims.Role);
            var result =
                measurements.Map<IEnumerable<Domain.Model.Measurement.Measurement>, IEnumerable<Model.Report.Measurement>>()
                    .AsQueryable();

            return result;
        }
    }
}