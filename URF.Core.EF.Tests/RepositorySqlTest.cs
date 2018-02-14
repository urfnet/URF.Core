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
    public class RepositorySqlTest
    {
        private readonly List<Product> _products;
        private readonly NorthwindDbContextFixture _fixture;

        public RepositorySqlTest(NorthwindDbContextFixture fixture)
        {
            var categories = new List<Category>
            {
                new Category { CategoryId = 1, CategoryName = "Beverages"},
            };
            _products = new List<Product>
            {
                new Product { ProductId = 1, ProductName = "Product 1", UnitPrice = 10, CategoryId = 1 },
                new Product { ProductId = 2, ProductName = "Product 2", UnitPrice = 20, CategoryId = 1 },
                new Product { ProductId = 3, ProductName = "Product 3", UnitPrice = 30, CategoryId = 1 },
            };
            _fixture = fixture;
            _fixture.Initialize(true, () =>
            {
                _fixture.Context.SeedDataSql(categories, _products);
            });
        }

        [Fact]
        public async Task SelectSqlAsync_Should_Return_Entities()
        {
            // Arrange
            var parameters = new object[] {1, "Product"};
            var sql = "SELECT * FROM Products WHERE CategoryId = {0} AND ProductName LIKE {1} + '%'";
            var repository = new Repository<Product>(_fixture.Context);

            // Act
            var products = await repository.SelectSqlAsync(sql, parameters);

            // Assert
            Assert.Collection(products,
                p => Assert.Equal(_products[0].ProductId, p.ProductId),
                p => Assert.Equal(_products[1].ProductId, p.ProductId),
                p => Assert.Equal(_products[2].ProductId, p.ProductId));
        }

        [Fact]
        public async Task QueryableSql_Should_Allow_Composition()
        {
            // Arrange
            var comparer = new MyProductComparer();
            var expected1 = new MyProduct { Id = 2, Name = "Product 2", Price = 20, Category = "Beverages" };
            var expected2 = new MyProduct { Id = 3, Name = "Product 3", Price = 30, Category = "Beverages" };
            var repository = new Repository<Product>(_fixture.Context);

            // Act
            var query = repository.QueryableSql("SELECT * FROM Products");
            var products = await query
                .Include(p => p.Category)
                .Where(p => p.UnitPrice > 15)
                .Select(p => new MyProduct
                {
                    Id = p.ProductId,
                    Name = p.ProductName,
                    Price = p.UnitPrice,
                    Category = p.Category.CategoryName
                })
                .ToListAsync();

            // Assert
            Assert.Collection(products,
                p => Assert.Equal(expected1, p, comparer),
                p => Assert.Equal(expected2, p, comparer));
        }
    }
}
