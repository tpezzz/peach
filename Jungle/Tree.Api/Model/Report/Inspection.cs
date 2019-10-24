using System;

namespace Tree.Api.Model.Report {
    public class Inspection {
        public string UserName { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset Created { get; set; }
        public bool? IsApproved { get; set; }
    }
}