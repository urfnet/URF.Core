using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using URF.Core.EF.Trackable;

namespace URF.Core.EF.Tests.Models
{
    public partial class Customer : Entity
    {
        [Key]
        public string CustomerId { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public List<Order> Orders { get; set; }
    }
}
