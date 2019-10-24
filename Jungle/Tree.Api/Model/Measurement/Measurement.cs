using Tree.Api.Model.Job;
using Tree.Domain.Model.Measurement;
using System;
using System.Collections.Generic;

namespace Tree.Api.Model.Measurement {
    public class Measurement {
        public Connection Connection { get; set; }
        public string ToolId { get; set; }
        public string CrimpLocation { get; set; }
        public bool IsMountSuccessfull { get; set; }
        public DateTimeOffset Installed { get; set; }

        public Dimension<DiameterUnit> FinalDiameter { get; set; }
        public Dimension<PressureUnit> Pressure { get; set; }
        public Dimension<TorqueUnit> Torque { get; set; }
    }

    public class MeasurementResult : Measurement {
        public Report.User CreatedBy { get; set; }
        public Report.User UpdatedBy { get; set; }
        public DateTimeOffset Updated { get; set; }
        public ICollection<InspectionQueryResult> Inspections { get; set; }
    }

    public class MeasurementQuery {
        public Guid? JobId { get; set; }
    }

    public class MeasurementQueryResult {
        public JobQueryResult Job { get; set; }
        public List<MeasurementResult> Measurements { get; set; }
    }

    public class MeasurementCommand : Measurement {
        public ICollection<InspectionCommand> Inspections { get; set; }
    }

    public class MeasurementPackage {
        public Guid JobId { get; set; }
        public CommandType CommandType { get; set; }
        public List<MeasurementCommand> Measurements { get; set; }
    }

    // We will map measurements from MeasurementPackage to this object and treat them seperately
    public class FullMeasurementCommand {
        public MeasurementCommand Measurement { get; set; }
        public Guid JobId { get; set; }
        public CommandType CommandType { get; set; }
    }
}