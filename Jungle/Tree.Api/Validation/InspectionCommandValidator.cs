using Tree.Api.Model.Measurement;
using FluentValidation;

namespace Tree.Domain.Validation {
    public class FullInspectionCommandValidator : AbstractValidator<FullInspectionCommand> {
        public FullInspectionCommandValidator() {
            RuleFor(x => x.JobId).NotEmpty();
            RuleFor(x => x.Inspection).NotNull();
            RuleFor(x => x.MeasurementInstallationDate).NotEmpty();
            RuleFor(x => x.MeasurementToolId).NotEmpty();
            RuleFor(x => x.Inspection).SetValidator(new InspectionCommandValidator()).When(x => x.Inspection != null);
        }
    }

    public class InspectionCommandValidator : AbstractValidator<InspectionCommand> {
        public InspectionCommandValidator() {
            RuleFor(x => x).Must(x => x.IsApproved != null || !string.IsNullOrWhiteSpace(x.Comment))
                .WithMessage("Inspection Comment or Approval information (IsApproved) must be provided");
        }
    }
}