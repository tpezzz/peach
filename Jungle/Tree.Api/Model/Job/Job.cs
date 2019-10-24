using System;

namespace Tree.Api.Model.Job {
    public class Job {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class JobQueryResult : Job {
        public Site.Site Site { get; set; }
    }

    public class JobCommand {
        public string Name;
        public Guid? SiteId;
        public Guid? Id;
        public bool? IsCompleted;
    }

    public class JobCommandResult : JobQueryResult { }
}