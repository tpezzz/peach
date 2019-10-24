using System;

namespace Tree.Api.Model.Report {
    public class ReportQuery {
        public Guid? SiteId { get; set; }
        public Guid? JobId { get; set; }
        public string Location { get; set; }
        public bool OwnInspection { get; set; }
        public DateTimeOffset? From { get; set; }
        public DateTimeOffset? To { get; set; }
        public string ConnectionId { get; set; }
        public Pagination Pagination { get; set; }
    }
}