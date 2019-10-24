using Tree.Api.Filter;
using Tree.Api.Map.CommandMap;
using Tree.Api.Model.Claims;
using Tree.Api.Model.Job;
using Tree.App.Administration.Handler;
using Tree.Domain.Model.User;
using ExpressMapper.Extensions;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http;

namespace Tree.Api.Controller {
    [DisableODataActionFilter]
    public class JobController : ApiController {
        private readonly IJobHandler jobHandler;
        private readonly ICommandMapper<JobCommand, Domain.Model.Company.Job> jobCommandMapper;

        public JobController(IJobHandler jobHandler,
                             ICommandMapper<JobCommand, Domain.Model.Company.Job> jobCommandMapper) {
            this.jobHandler = jobHandler;
            this.jobCommandMapper = jobCommandMapper;
        }

        [Authorize(Roles = "SystemAdmin,Supervisor,Installer,Inspector,Subscriber")]
        public IHttpActionResult Get() {
            var author = User.Identity.Map<IIdentity, AuthorizationClaims>();

            // for installer don't include completed
            var includeCompleted = author.Role != RoleType.Installer;

            var jobs = jobHandler.Get(author.Id, author.CompanyId, author.Role, includeCompleted, "Site");
            var result = jobs.Map<IEnumerable<Domain.Model.Company.Job>, List<JobQueryResult>>();

            return Ok(result);
        }

        [Authorize(Roles = "SystemAdmin,Supervisor")]
        public async Task<IHttpActionResult> Post([FromBody] JobCommand command) {
            var author = User.Identity.Map<IIdentity, AuthorizationClaims>();
            var domainJob = jobCommandMapper.Map(command);

            if (command.Id == null)
                await jobHandler.CreateAsync(domainJob, author.CompanyId);
            else
                await jobHandler.UpdateAsync(domainJob, author.CompanyId);

            return Ok(domainJob.Map<Domain.Model.Company.Job, JobCommandResult>());
        }
    }
}