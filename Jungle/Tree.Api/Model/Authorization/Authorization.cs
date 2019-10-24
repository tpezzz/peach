using Tree.Domain.Model.User;
using System;
using System.Collections.Generic;

namespace Tree.Api.Model.Authorization {
    public class Authorization {
        public Guid UserId { get; set; }
        public IEnumerable<Guid> Keys { get; set; }
    }

    public class AuthorizationResult : Authorization {
        public bool IsRestricted { get; set; }
        public RoleType Role { get; set; }
    }

    public class AuthorizationCommand : Authorization {
        public bool? IsRestricted { get; set; }
        public RoleType? Role { get; set; }
    }

    public class AuthorizationCommandResult : AuthorizationResult {

    }
}