using Tree.Api.Model.Job;
using Tree.Api.Model.Site;
using System;
using System.Collections.Generic;

namespace Tree.Api.Model.Report {
    public class Measurement {
        public Guid Id { get; set; }
        public Connection Connection { get; set; }
        public string ToolId { get; set; }
        public string CrimpLocation { get; set; }
        public bool IsMountSuccessfull { get; set; }
        public DateTimeOffset Installed { get; set; }
        public DateTimeOffset Updated { get; set; }

        public Dimension FinalDiameter { get; set; }
        public Dimension Pressure { get; set; }
        public Dimension Torque { get; set; }

        public Company Company { get; set; }
        public JobQueryResult Job { get; set; }
        public SiteQueryResult Site { get; set; }

        public User CreatedBy { get; set; }
        public User UpdatedBy { get; set; }

        public IEnumerable<Inspection> Inspections { get; set; }
    }

    public class Dimension {
        public double Value { get; set; }
        public string Unit { get; set; }
    }
}