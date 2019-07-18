﻿using System;
using System.Collections.Generic; // Comment

namespace ScaffoldingSample.Models
{ // Comment
    public partial class EmployeeTerritories // My Handlebars Helper
    {
        public int EmployeeId { get; set; }
        public string TerritoryId { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual Territory Territory { get; set; }

        // My Handlebars Block Helper: True
        // My Handlebars Block Helper: False
    }
}
