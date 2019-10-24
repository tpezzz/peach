using Tree.Api.Model.Measurement;
using ExpressMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tree.Api.Map.CustomMap {
    public class ToFullInspectionCommandsCustomMapper :
        ICustomTypeMapper<MeasurementPackage, IEnumerable<FullInspectionCommand>> {
        public IEnumerable<FullInspectionCommand> Map(
            IMappingContext<MeasurementPackage, IEnumerable<FullInspectionCommand>> context) {
            var source = context.Source;

            return source.Measurements.SelectMany(measurementCommand =>
                measurementCommand.Inspections == null ?
                Enumerable.Empty<FullInspectionCommand>() :
                measurementCommand.Inspections.Select(inspection =>
                    new FullInspectionCommand {
                        Inspection = inspection,
                        JobId = source.JobId,
                        MeasurementInstallationDate = measurementCommand.Installed,
                        MeasurementToolId = measurementCommand.ToolId,
                        CommandType = source.CommandType
                    }
                )
            );
        }
    }

    public class ToApiInspectionQueryResultCustomMapper :
        ICustomTypeMapper<Domain.Model.Measurement.Inspection, Api.Model.Measurement.InspectionQueryResult> {
        public Api.Model.Measurement.InspectionQueryResult Map(
            IMappingContext<Domain.Model.Measurement.Inspection, Api.Model.Measurement.InspectionQueryResult> context) {
            var source = context.Source;

            return new Api.Model.Measurement.InspectionQueryResult {
                Id = source.Id,
                Comment = source.Comment,
                IsApproved = source.IsApproved,
                UserId = source.Inspector == null ? (Guid?)null : source.Inspector.Id,
                UserName = source.Inspector == null ? null : source.Inspector.Name,
                Created = source.Created
            };
        }
    }

    public class ToReportApiInspectionCustomMapper :
    ICustomTypeMapper<Domain.Model.Measurement.Inspection, Api.Model.Report.Inspection> {
        public Api.Model.Report.Inspection Map(
            IMappingContext<Domain.Model.Measurement.Inspection, Api.Model.Report.Inspection> context) {
            var source = context.Source;

            return new Api.Model.Report.Inspection {
                Comment = source.Comment,
                Created = source.Created,
                UserName = source.Inspector.Name,
                IsApproved = source.IsApproved
            };
        }
    }

    public class ToDomainInspectionCustomMapper :
       ICustomTypeMapper<Api.Model.Measurement.InspectionCommand, Domain.Model.Measurement.Inspection> {
        public Domain.Model.Measurement.Inspection Map(
            IMappingContext<Api.Model.Measurement.InspectionCommand, Domain.Model.Measurement.Inspection> context) {
            var source = context.Source;

            return new Domain.Model.Measurement.Inspection {
                Comment = source.Comment,
                Id = Guid.NewGuid(),
                IsActive = true,
                IsApproved = source.IsApproved,
                Created = DateTimeOffset.UtcNow
            };
        }
    }
}