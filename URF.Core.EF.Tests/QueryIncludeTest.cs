using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using URF.Core.EF.Tests.Contexts;
using URF.Core.EF.Tests.Models;
using Xunit;

namespace URF.Core.EF.Tests
{
    [Collection(nameof(NorthwindDbContext))]
    public class QueryIncludeTest
    {
        private const string SkipReason = "Only run locally with MS SQL LocalDb";
        private readonly NorthwindDbContextFixture _fixture;

        public QueryIncludeTest(NorthwindDbContextFixture fixture)
        {
            var categories = Factory.Categories();
            var products = Factory.Products();
            var orders = Factory.Orders();
            var customers = Factory.Customers();
            var orderDetails = Factory.OrderDetails();

            _fixture = fixture;
            _fixture.Initialize(false, () =>
            {
                EnsureEntities(_fixture.Context, categories);
                EnsureEntities(_fixture.Context, products);
                EnsureEntities(_fixture.Context, customers);
                EnsureEntities(_fixture.Context, orders);
                EnsureEntities(_fixture.Context, orderDetails);
            });
        }

        private void EnsureEntities<TEntity>(NorthwindDbContext context, IEnumerable<TEntity> entities)
        {
            context.Database.OpenConnection();
            try
            {
                string tableName;
                if (typeof(TEntity) == typeof(Category))
                {
                    if (context.Categories.Any()) return;
                    context.Categories.AddRange((IEnumerable<Category>)entities);
                    tableName = "Categories";
                }
                else if (typeof(TEntity) == typeof(Product))
                {
                    if (context.Products.Any()) return;
                    context.Products.AddRange((IEnumerable<Product>)entities);
                    tableName = "Products";
                }
                else if (typeof(TEntity) == typeof(Customer))
                {
                    if (context.Customers.Any()) return;
                    context.Customers.AddRange((IEnumerable<Customer>)entities);
                    tableName = "Customers";
                }
                else if (typeof(TEntity) == typeof(Order))
                {
                    if (context.Orders.Any()) return;
                    context.Orders.AddRange((IEnumerable<Order>)entities);
                    tableName = "Orders";
                }
                else if (typeof(TEntity) == typeof(OrderDetail))
                {
                    if (context.OrderDetails.Any()) return;
                    context.OrderDetails.AddRange((IEnumerable<OrderDetail>)entities);
                    tableName = "OrderDetails";
                }
                else
                {
                    return;
                }

                var insertOn = $"SET IDENTITY_INSERT dbo.{tableName} ON";
                var insertOff = $"SET IDENTITY_INSERT dbo.{tableName} OFF";
                if (typeof(TEntity) != typeof(Customer) && typeof(TEntity) != typeof(OrderDetail))
                    context.Database.ExecuteSqlCommand(insertOn);
                context.SaveChanges();
                if (typeof(TEntity) != typeof(Customer) && typeof(TEntity) != typeof(OrderDetail))
                    context.Database.ExecuteSqlCommand(insertOff);
            }
            finally
            {
                context.Database.CloseConnection();
            }
        }

        [Fact(Skip = SkipReason)]
        public async Task Query_Fluent_Api_Should_Not_Load_Included_Entities()
        {
            // Arrange
            int[] ids = { 10248 , 10249 };
            var repository = new Repository<Order>(_fixture.Context);

            // Act
            var orders = (await repository
                .Query()
                .Where(o => ids.Contains(o.OrderId))
                .SelectAsync()).ToList();

            // Assert
            Assert.Collection(orders,
                o => Assert.Null(o.Customer),
                o => Assert.Null(o.Customer));
            Assert.Collection(orders,
                o => Assert.Empty(o.OrderDetails),
                o => Assert.Empty(o.OrderDetails));
        }

        [Fact(Skip = SkipReason)]
        public async Task Query_Fluent_Api_Should_Load_Included_Entities()
        {
            // Arrange
            int[] ids = { 10248, 10249 };
            var repository = new Repository<Order>(_fixture.Context);

            // Act
            var orders = (await repository
                .Query()
                .Where(o => ids.Contains(o.OrderId))
                .Include(o => o.Customer)
                .Include("OrderDetails.Product.Category")
                .SelectAsync()).ToList();

            // Assert
            Assert.Collection(orders,
                o => Assert.NotNull(o.Customer),
                o => Assert.NotNull(o.Customer));
            Assert.Collection(orders,
                o => Assert.NotEmpty(o.OrderDetails),
                o => Assert.NotEmpty(o.OrderDetails));
            Assert.Collection(orders,
                o => Assert.Collection(o.OrderDetails,
                    od => Assert.NotNull(od.Product),
                    od => Assert.NotNull(od.Product.Category),
                    od => Assert.NotNull(od.Product.Category)),
                o => Assert.Collection(o.OrderDetails,
                    od => Assert.NotNull(od.Product),
                    od => Assert.NotNull(od.Product.Category)));
        }
    }
}