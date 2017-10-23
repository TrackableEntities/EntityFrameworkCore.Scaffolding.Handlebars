using System;
using System.Collections.Generic; // Comment

namespace ScaffoldingSample.Models
{ // Comment
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

        public ICollection<EmployeeTerritories> EmployeeTerritories { get; set; }
    }
}
