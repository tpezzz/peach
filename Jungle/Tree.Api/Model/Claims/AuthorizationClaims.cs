using Tree.Domain.Model.User;
using System;

namespace Tree.Api.Model.Claims {
    public class AuthorizationClaims {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public RoleType Role { get; set; }
        public Guid CompanyId { get; set; }
    }
}