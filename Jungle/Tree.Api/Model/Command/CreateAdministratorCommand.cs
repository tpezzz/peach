using ABB.BodhiTree.Api.Model.Company;
using System;

namespace ABB.BodhiTree.Api.Model.Command {
    public class CreateAdministratorCommand {
        public string Name { get; set; }
        public string Email { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }

        public AdministratorType Type { get; set; }
    }
}