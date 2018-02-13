using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using URF.Core.EF.Trackable;

namespace URF.Core.EF.Tests.Models
{
    public partial class OrderDetail : Entity
    {
        [Key, Column(Order = 1)]
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
        public decimal UnitPrice { get; set; }
        public short Quantity { get; set; }
        public float Discount { get; set; }

    }
}
