using Tree.Api.Model.Job;
using Tree.Api.Model.Site;
using ExpressMapper;
using ExpressMapper.Extensions;
using System.Linq;

namespace Tree.Api.Map.CustomMap {
    public class ToReportMeasurementCustomMapper : ICustomTypeMapper<Domain.Model.Measurement.Measurement, Api.Model.Report.Measurement> {
        public Api.Model.Report.Measurement Map(
            IMappingContext<Domain.Model.Measurement.Measurement, Api.Model.Report.Measurement> context) {
            var source = context.Source;

            return new Api.Model.Report.Measurement {
                Id = source.Id,
                Company = (source.Job == null || source.Job.Site == null || source.Job.Site.Company == null)
                           ? null : source.Job.Site.Company.Map<Domain.Model.Company.Company, Api.Model.Report.Company>(),
                Connection = source.Connection == null ? null : source.Connection.Map<Domain.Model.Measurement.Connection, Api.Model.Report.Connection>(),
                CrimpLocation = source.CrimpLocation,
                FinalDiameter = source.FinalDiameter == null ? null : source.FinalDiameter.Map<Domain.Model.Measurement.DiameterDimension, Api.Model.Report.Dimension>(),
                Inspections = source.Inspections == null ? null : source.Inspections.Select(x => x.Map<Domain.Model.Measurement.Inspection, Api.Model.Report.Inspection>()),
                Installed = source.Installed,
                Updated = source.Updated,
                CreatedBy = source.CreatedBy == null ? null : source.CreatedBy.Map<Domain.Model.User.User, Api.Model.Report.User>(),
                UpdatedBy = source.UpdatedBy == null ? null : source.UpdatedBy.Map<Domain.Model.User.User, Api.Model.Report.User>(),
                IsMountSuccessfull = source.IsMountSuccessfull,
                Job = source.Job == null ? null : source.Job.Map<Domain.Model.Company.Job, JobQueryResult>(),
                Site = (source.Job == null || source.Job.Site == null)
                       ? null : source.Job.Site.Map<Domain.Model.Company.Site, SiteQueryResult>(),
                ToolId = source.ToolId,
                Torque = source.Torque == null ? null : source.Torque.Map<Domain.Model.Measurement.TorqueDimension, Api.Model.Report.Dimension>(),
                Pressure = source.Pressure == null ? null : source.Pressure.Map<Domain.Model.Measurement.PressureDimension, Api.Model.Report.Dimension>()
            };
        }
    }
}