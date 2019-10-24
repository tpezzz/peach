using Autofac.Extras.NLog;
using Autofac.Integration.WebApi;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Tree.Api.Filter {
    public class LoggingFilter : IAutofacActionFilter {
        private readonly ILogger logger;

        public LoggingFilter(ILogger logger) {
            this.logger = logger;
        }

        public async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext,
            CancellationToken cancellationToken) {
            logger.Info(actionExecutedContext.Request);
            if (actionExecutedContext.Request.Method != HttpMethod.Get)
                logger.Info(await actionExecutedContext.Request.Content.ReadAsStringAsync());

            if (actionExecutedContext.Response != null) {
                logger.Info(actionExecutedContext.Response);
            }
        }

        public Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken) {
            return Task.FromResult(true);
        }
    }
}