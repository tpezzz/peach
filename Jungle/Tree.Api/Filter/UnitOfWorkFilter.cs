using Tree.App.Core.UnitOfWork;
using Autofac.Integration.WebApi;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Tree.Api.Filter {
    public class UnitOfWorkFilter : IAutofacActionFilter {
        private readonly IUnitOfWork unitOfWork;

        public UnitOfWorkFilter(IUnitOfWork unitOfWork) {
            this.unitOfWork = unitOfWork;
        }

        public async System.Threading.Tasks.Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, System.Threading.CancellationToken cancellationToken) {
            if (actionExecutedContext.Exception == null) {
                await unitOfWork.CommitAsync();
            }
        }

        public System.Threading.Tasks.Task OnActionExecutingAsync(HttpActionContext actionContext, System.Threading.CancellationToken cancellationToken) {
            return Task.FromResult(true);
        }
    }
}