using Tree.Api.Model;
using Tree.Api.Model.Measurement;
using FluentValidation;

namespace Tree.Domain.Validation {
    public class FullMeasurementCommandValidator : AbstractValidator<FullMeasurementCommand> {
        public FullMeasurementCommandValidator() {
            RuleFor(x => x.JobId).NotEmpty();
            RuleFor(x => x.Measurement).NotNull();
            RuleFor(x => x.Measurement).SetValidator(x => new MeasurementCommandValidator(x.CommandType)).When(x => x.Measurement != null);
        }
    }

    public class MeasurementCommandValidator : AbstractValidator<MeasurementCommand> {
        public MeasurementCommandValidator(CommandType commandType) {
            RuleFor(x => x.Installed).NotEmpty().WithMessage("Field 'Installed' has not UTC format or is empty.");
            RuleFor(x => x.ToolId).NotEmpty();
            if (commandType == CommandType.Create) {
                RuleFor(x => x.Connection).NotNull();
                RuleFor(x => x.FinalDiameter).NotNull();
                RuleFor(x => x.Pressure).NotNull();
            }
            RuleFor(x => x.Connection).SetValidator(new ConnectionValidator()).When(x => x.Connection != null && commandType == CommandType.Create);
        }
    }

    public class ConnectionValidator : AbstractValidator<Connection> {
        public ConnectionValidator() {
            RuleFor(x => x.SerialNumber).NotEmpty();
            RuleFor(x => x.Type).NotEmpty();
        }
    }
}