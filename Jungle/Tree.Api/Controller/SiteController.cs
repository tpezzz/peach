using Tree.Api.Filter;
using Tree.Api.Map.CommandMap;
using Tree.Api.Model.Claims;
using Tree.Api.Model.Site;
using Tree.App.Administration.Handler;
using Tree.Domain.Comparer;
using ExpressMapper.Extensions;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http;

namespace Tree.Api.Controller {
    [DisableODataActionFilter]
    public class SiteController : ApiController {
        private readonly ISiteHandler siteHandler;
        private readonly ICommandMapper<SiteCommand, Domain.Model.Company.Site> siteCommandMapper;

        public SiteController(ISiteHandler siteHandler,
                              ICommandMapper<SiteCommand, Domain.Model.Company.Site> siteCommandMapper) {
            this.siteCommandMapper = siteCommandMapper;
            this.siteHandler = siteHandler;
        }

        [Authorize(Roles = "SystemAdmin,Supervisor,Installer,Inspector,Subscriber")]
        public IHttpActionResult Get([FromUri] SiteQuery query) {
            var author = User.Identity.Map<IIdentity, AuthorizationClaims>();

            var domainSites = siteHandler.Get(author.Id, author.CompanyId, author.Role, query.IncludeCompleted, "Jobs");
            var distinctSites = domainSites.Distinct(new EntityIdEqualityComparer<Domain.Model.Company.Site>());
            var result = distinctSites.Select(x => x.Map<Domain.Model.Company.Site, SiteQueryResult>()).ToList();

            return Ok(result);
        }

        [Authorize(Roles = "SystemAdmin,Supervisor")]
        public async Task<IHttpActionResult> Post([FromBody] SiteCommand command) {
            var author = User.Identity.Map<IIdentity, AuthorizationClaims>();
            var domainSite = siteCommandMapper.Map(command);

            if (command.Id == null)
                await siteHandler.CreateAsync(domainSite, author.CompanyId);
            else
                await siteHandler.UpdateAsync(domainSite);

            return Ok(domainSite.Map<Domain.Model.Company.Site, SiteCommandResult>()); ;
        }
    }
}