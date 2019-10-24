using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Tree.Api.Filter {
    public class DisableODataActionFilterAttribute : ActionFilterAttribute {
        public override void OnActionExecuting(HttpActionContext actionContext) {
            var actionPath = actionContext.Request.RequestUri.AbsolutePath;
            if (actionPath.StartsWith("/odata")) {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    string.Format("This action is not available using OData. Use {0} instead.",
                                  actionPath.Replace("odata", "api"))
                );
            }
        }
    }
}