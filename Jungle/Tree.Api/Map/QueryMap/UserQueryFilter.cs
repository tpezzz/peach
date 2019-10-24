using Tree.Api.Model.User;
using System.Collections.Generic;
using System.Linq;

namespace Tree.Api.Map.QueryMap {
    public class UserQueryFilter : IQueryFilter<IEnumerable<UserQueryResult>, UserQuery> {
        public IEnumerable<UserQueryResult> Run(IEnumerable<UserQueryResult> entity, UserQuery query) {
            var result = entity;

            if (query.SiteIds != null && query.SiteIds.Count() > 0) {
                result = result.Where(x => x.Sites.Select(a => a.Id).Intersect(query.SiteIds).Count() > 0);
            }

            if (query.JobIds != null && query.JobIds.Count() > 0) {
                result = result.Where(x => x.Sites.Where(a => query.JobIds.Intersect(a.Jobs.Select(y => y.Id)).Count() > 0).Count() > 0);
            }

            return result;
        }
    }
}