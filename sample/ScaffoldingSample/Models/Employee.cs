using System;
using System.Collections.Generic; // Comment

namespace ScaffoldingSample.Models
{ // Comment
    public partial class Employee // My Handlebars Helper
    {
        public Employee()
        {
            EmployeeTerritories = new HashSet<EmployeeTerritories>();
        }

        public int EmployeeId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? HireDate { get; set; }
        public string City { get; set; }
        public Country Country { get; set; }

        public virtual ICollection<EmployeeTerritories> EmployeeTerritories { get; set; }

        // My Handlebars Block Helper: True
        // My Handlebars Block Helper: False
    }
}
