using System;
using System.Collections.Generic;

namespace ScaffoldingSample.Embedded.Models
{
    public partial class Employee
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
        public string Country { get; set; }

        public virtual ICollection<EmployeeTerritories> EmployeeTerritories { get; set; }
    }
}
