using System;
using System.Collections.Generic; // Comment

namespace ScaffoldingSample.Models
{ // Comment
    public partial class Employee : EntityBase // My Handlebars Helper
    {
        public int EmployeeId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? HireDate { get; set; }
        public string City { get; set; }
        public Country Country { get; set; }

        // My Handlebars Block Helper: True
        // My Handlebars Block Helper: False
    }
}
