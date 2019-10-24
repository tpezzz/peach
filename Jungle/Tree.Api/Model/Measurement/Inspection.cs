using System;

namespace Tree.Api.Model.Measurement {
    public class Inspection {
        public Guid? Id { get; set; }
        public string Comment { get; set; }
        public bool? IsApproved { get; set; }
    }

    public class InspectionCommand {
        public string Comment { get; set; }
        public bool? IsApproved { get; set; }
    }

    // We will map inspections from MeasurementPackage to this object and treat them seperately
    public class FullInspectionCommand {
        public InspectionCommand Inspection { get; set; }
        public DateTimeOffset MeasurementInstallationDate { get; set; }
        public string MeasurementToolId { get; set; }
        public Guid JobId { get; set; }
        public CommandType CommandType { get; set; }
    }

    public class InspectionQueryResult : Inspection {
        public Guid? UserId { get; set; }
        public string UserName { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}