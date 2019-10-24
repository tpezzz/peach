using System;
using System.Collections.Generic;

namespace ABB.BodhiTree.Api.Model.Command {
    public class CreateCompanyCommand {
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }

        public IEnumerable<CreateAdministratorCommand> Administrators { get; set; }
    }
}