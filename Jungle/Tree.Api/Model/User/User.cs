using Tree.Api.Model.Site;
using Tree.Domain.Model.User;
using System;
using System.Collections.Generic;

namespace Tree.Api.Model.User {
    public class User {
        public string Name { get; set; }
        public string Email { get; set; }
        public RoleType Role { get; set; }
    }

    public class UserQuery {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public IEnumerable<Guid> SiteIds { get; set; }
        public IEnumerable<Guid> JobIds { get; set; }
        public IEnumerable<RoleType> Roles { get; set; }
        public bool? IncludeRestricted { get; set; }
        public Guid? CompanyId { get; set; }
    }

    public class UserQueryResult : User {
        public Guid Id { get; set; }
        public bool IsRestricted { get; set; }
        public DateTimeOffset Updated { get; set; }
        public IEnumerable<SiteQueryResult> Sites { get; set; }
    }

    public class UserCommand : User {
        public IEnumerable<Guid> Keys { get; set; }
    }

    public class UserCommandResult : User {
        public Guid Id { get; set; }
        public IEnumerable<Guid> Keys { get; set; }
        public DateTimeOffset Updated { get; set; }
        public string AccessCode { get; set; }
    }
}