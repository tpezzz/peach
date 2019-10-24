using Tree.Api.Model.Job;
using Tree.Api.Model.Measurement;
using Tree.Domain.Model.Measurement;
using ExpressMapper;
using ExpressMapper.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Tree.Api.Map.CustomMap {
    public class ToDomainTorqueDimensionCustomMapper :
        ICustomTypeMapper<Domain.Model.Measurement.Dimension<TorqueUnit>, Domain.Model.Measurement.TorqueDimension> {
        public Domain.Model.Measurement.TorqueDimension Map(
            IMappingContext<Dimension<TorqueUnit>, Domain.Model.Measurement.TorqueDimension> context) {
            var source = context.Source;

            if (source == null) {
                return new Domain.Model.Measurement.TorqueDimension {
                    Unit = null,
                    Value = null,
                };
            }
            return new Domain.Model.Measurement.TorqueDimension {
                Unit = (Domain.Model.Measurement.TorqueUnit?)source.Unit,
                Value = (double)source.Value
            };
        }
    }

    public class ToApiTorqueDimensionCustomMapper :
    ICustomTypeMapper<Domain.Model.Measurement.TorqueDimension, Dimension<TorqueUnit>> {
        public Dimension<TorqueUnit> Map(
            IMappingContext<Domain.Model.Measurement.TorqueDimension, Dimension<TorqueUnit>> context) {
            var source = context.Source;

            if (!source.Unit.HasValue || !source.Value.HasValue) {
                return null;
            }

            return new Dimension<TorqueUnit> {
                Unit = (TorqueUnit)source.Unit.Value,
                Value = source.Value.Value
            };
        }
    }

    public class ToFullMeasurementCommandsCustomMapper :
        ICustomTypeMapper<MeasurementPackage, IEnumerable<FullMeasurementCommand>> {
        public IEnumerable<FullMeasurementCommand> Map(
            IMappingContext<MeasurementPackage, IEnumerable<FullMeasurementCommand>> context) {
            var source = context.Source;

            return source.Measurements.Select(x => new FullMeasurementCommand {
                Measurement = x,
                CommandType = source.CommandType,
                JobId = source.JobId
            });
        }
    }

    public class ToApiMeasurementCustomMapper :
        ICustomTypeMapper<Domain.Model.Measurement.Measurement, Api.Model.Measurement.MeasurementResult> {
        public Api.Model.Measurement.MeasurementResult Map(
            IMappingContext<Domain.Model.Measurement.Measurement, Api.Model.Measurement.MeasurementResult> context) {
            var source = context.Source;

            return new Api.Model.Measurement.MeasurementResult {
                Connection = source.Connection.Map<Domain.Model.Measurement.Connection, Model.Measurement.Connection>(),
                ToolId = source.ToolId,
                CrimpLocation = source.CrimpLocation,
                IsMountSuccessfull = source.IsMountSuccessfull,
                Installed = source.Installed,
                Updated = source.Updated,
                FinalDiameter =
                    source.FinalDiameter.Map<Domain.Model.Measurement.DiameterDimension, Dimension<DiameterUnit>>(),
                Pressure = source.Pressure.Map<Domain.Model.Measurement.PressureDimension, Dimension<PressureUnit>>(),
                Torque = source.Torque == null ? null : source.Torque.Map<Domain.Model.Measurement.TorqueDimension, Dimension<TorqueUnit>>(),
                Inspections = source.Inspections == null ? null : source.Inspections.Map<IEnumerable<Domain.Model.Measurement.Inspection>, List<Model.Measurement.InspectionQueryResult>>(),
                CreatedBy = source.CreatedBy == null ? null : source.CreatedBy.Map<Domain.Model.User.User, Api.Model.Report.User>(),
                UpdatedBy = source.UpdatedBy == null ? null : source.UpdatedBy.Map<Domain.Model.User.User, Api.Model.Report.User>(),
            };
        }
    }

    public class ToGetMeasurementQueryResultFromGroupped :
        ICustomTypeMapper<IGrouping<Domain.Model.Company.Job, Domain.Model.Measurement.Measurement>, MeasurementQueryResult> {
        public MeasurementQueryResult Map(
            IMappingContext<IGrouping<Domain.Model.Company.Job, Domain.Model.Measurement.Measurement>, MeasurementQueryResult> context) {
            var source = context.Source;

            return new MeasurementQueryResult {
                Job = source.Key.Map<Domain.Model.Company.Job, JobQueryResult>(),
                Measurements = source.Map<IEnumerable<Domain.Model.Measurement.Measurement>, List<Api.Model.Measurement.MeasurementResult>>()
            };
        }
    }
}