using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scaffolding.Handlebars.Tests.Models
{
    [Table("Customer")]
    public partial class Customer
    {
        [Key]
        [StringLength(5)]
        public string CustomerKey { get; set; }
        [Required]
        [StringLength(40)]
        public string CompanyName { get; set; }
        [StringLength(30)]
        public string ContactName { get; set; }
        [StringLength(15)]
        public string City { get; set; }
        [StringLength(15)]
        public string Country { get; set; }
    }
}