using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using URF.Core.EF.Trackable;

namespace URF.Core.EF.Tests.Models
{
    public partial class Customer : Entity
    {
        [Key]
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public List<Order> Orders { get; set; }
    }
}
