using Tree.App.Core.Exception;
using Autofac.Integration.WebApi;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using ILogger = Autofac.Extras.NLog.ILogger;

namespace Tree.Api.Filter {
    public class ExceptionFilter : IAutofacExceptionFilter {
        private readonly ILogger logger;

        public ExceptionFilter(ILogger logger) {
            this.logger = logger;
        }

        public static string FormatErrors(IDictionary<string, string> errorsDictionary) {
            var formattedErrors = errorsDictionary.Select(error => string.Format("{0}: {1}", error.Key, error.Value));
            var responseMessage = string.Join("\n", formattedErrors);
            return responseMessage;
        }

        public async Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken) {
            await Task.Run(() => {
                if (actionExecutedContext.Exception is AppException) {
                    var ex = (actionExecutedContext.Exception as AppException);

                    var formattedExc = FormatErrors(ex.Errors);
                    actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(ex.HttpStatusCode, formattedExc);

                    logger.LogException(LogLevel.Error, string.Format("{0} {1}",
                        actionExecutedContext.Request.RequestUri.AbsoluteUri, formattedExc),
                   actionExecutedContext.Exception);
                }
                else {
                    logger.LogException(LogLevel.Error, actionExecutedContext.Request.RequestUri.AbsoluteUri,
                       actionExecutedContext.Exception);
                    actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Internal server error");
                }
            });
        }
    }
}