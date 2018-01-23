using Microsoft.EntityFrameworkCore;
using URF.Core.EF.Tests.Models;

namespace URF.Core.EF.Tests.Contexts
{
    public class NorthwindDbContext : DbContext
    {
        public NorthwindDbContext() { }

        public NorthwindDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}