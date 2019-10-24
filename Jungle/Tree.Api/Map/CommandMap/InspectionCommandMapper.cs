using Tree.Api.Model.Measurement;
using Tree.App.Core.Exception;
using Tree.App.Measurement;
using System;
using System.Linq;

namespace Tree.Api.Map.CommandMap {
    public class InspectionCommandMapper : ICommandMapper<FullInspectionCommand, Domain.Model.Measurement.Inspection> {
        private readonly IMeasurementHandler measurementHandler;

        public InspectionCommandMapper(IMeasurementHandler measurementHandler) {
            this.measurementHandler = measurementHandler;
        }

        public Domain.Model.Measurement.Inspection Map(FullInspectionCommand command) {
            var existingMeasurement = measurementHandler.Get(
                   x => x.Job.Id == command.JobId &&
                   x.ToolId == command.MeasurementToolId &&
                   x.Installed == command.MeasurementInstallationDate).FirstOrDefault();

            if (existingMeasurement == null) {
                throw new NoMatchingEntityException("Database", "No matching active measurement found");
            }

            return new Domain.Model.Measurement.Inspection {
                Id = Guid.NewGuid(),
                IsActive = true,
                IsApproved = command.Inspection.IsApproved,
                Measurement = existingMeasurement,
                Created = DateTimeOffset.UtcNow,
                Comment = command.Inspection.Comment,
                Inspector = null // to be filled elsewhere
            };
        }
    }
}