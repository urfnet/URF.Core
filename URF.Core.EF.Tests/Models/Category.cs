using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace URF.Core.EF.Tests.Models
{
    public partial class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<Product> Products { get; set; }
    }
}
