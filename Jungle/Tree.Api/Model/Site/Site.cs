using System;
using System.Collections.Generic;

namespace Tree.Api.Model.Site {
    public class Site {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class SiteQuery {
        public bool? IncludeCompleted { get; set; }
    }

    public class SiteQueryResult : Site {
        public IEnumerable<Job.Job> Jobs { get; set; }
    }

    public class SiteCommand {
        public string Name;
        public Guid? Id;
        public bool? IsCompleted;
    }

    public class SiteCommandResult : Site { }
}