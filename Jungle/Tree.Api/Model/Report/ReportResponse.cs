using System.Collections.Generic;

namespace Tree.Api.Model.Report {
    public class ReportResult {
        public IEnumerable<Measurement> Measurements { get; set; }
        public Pagination Pagination { get; set; }
    }
}