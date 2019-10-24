using System;
using System.Collections.Generic;

namespace Tree.Api.Model.Company {
    public class Company {
        public Guid? Id { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
    }

    public class CompanyQueryResult : Company {
        public string CompanyId { get; set; }
        public IEnumerable<AdministratorQueryResult> Administrators { get; set; }
    }

    public class CompanyCommand : Company {
        public IEnumerable<AdministratorCommand> Administrators { get; set; }
    }

    public class RegisterCompanyCommand : Company {
        public IEnumerable<AdministratorCommand> Administrators { get; set; }
    }

    public class CompanyCommandResult : Company {
        public string CompanyId { get; set; }
        public IEnumerable<AdministratorCommandResult> Administrators { get; set; }
    }
}