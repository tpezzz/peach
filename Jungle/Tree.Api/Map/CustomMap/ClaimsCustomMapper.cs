
using Tree.Api.Model.Claims;
using Tree.Domain.Model.User;
using ExpressMapper;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Tree.Api.Map.CustomMap {
    public class ClaimsCustomMapper : ICustomTypeMapper<IIdentity, AuthorizationClaims> {

        public AuthorizationClaims Map(IMappingContext<IIdentity, AuthorizationClaims> context) {
            var identityClaims = ((ClaimsIdentity)(context.Source)).Claims;

            return new AuthorizationClaims() {
                Id = Guid.Parse(identityClaims.First(c => c.Type == "Id").Value),
                Role = (RoleType)Enum.Parse(typeof(RoleType), identityClaims.First(c => c.Type == ClaimTypes.Role).Value),
                Name = identityClaims.First(c => c.Type == ClaimTypes.Name).Value,
                Email = identityClaims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value,
                CompanyId = Guid.Parse(identityClaims.First(c => c.Type == "CompanyId").Value)
            };
        }
    }
}