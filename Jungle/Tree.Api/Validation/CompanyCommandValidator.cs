using Tree.Api.Model.Company;
using Tree.App.Authorization.Handler;
using FluentValidation;
using System.Linq;

namespace Tree.Domain.Validation {
    public class CompanyCommandValidator : AbstractValidator<CompanyCommand> {
        public CompanyCommandValidator() {
            RuleFor(x => x.Administrators).NotNull()
                .When(x => x.Id == null);
            RuleFor(x => x.Administrators).NotEmpty()
                .When(x => x.Id == null)
                .WithMessage("At least one administrator is required");
            RuleFor(x => x.Administrators.Count()).LessThanOrEqualTo(CompanyAuthorizationHandler.MaxActiveAdminsInCompany)
                .WithMessage(string.Format("Too many administrators (maximum is {0})", CompanyAuthorizationHandler.MaxActiveAdminsInCompany));
        }
    }
}