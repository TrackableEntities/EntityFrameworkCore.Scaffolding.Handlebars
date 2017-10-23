using System;
using System.Collections.Generic; // Comment

namespace ScaffoldingSample.Models
{ // Comment
    public partial class EmployeeTerritories
    {
        public int EmployeeId { get; set; }
        public string TerritoryId { get; set; }

        public Employee Employee { get; set; }
        public Territory Territory { get; set; }
    }
}
