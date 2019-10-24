using System.Data.Entity;
using System.Web.Http;

namespace Tree.Api.Controller {
    public class DiagnosticController : ApiController {
        private readonly DbContext context;

        public DiagnosticController(DbContext context) {
            this.context = context;
        }

        public IHttpActionResult Get() {
            return Ok(context.Database.Connection.Database);
        }
    }
}