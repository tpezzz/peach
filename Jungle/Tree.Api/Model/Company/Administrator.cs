using System;

namespace Tree.Api.Model.Company {
    public class Administrator {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
    }

    public class AdministratorQueryResult : Administrator {
    }

    public class AdministratorCommand : Administrator {
    }

    public class AdministratorCommandResult : Administrator {
        public string AccessCode { get; set; }
    }
}