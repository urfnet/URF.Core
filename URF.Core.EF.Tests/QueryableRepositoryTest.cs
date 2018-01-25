using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using URF.Core.EF.Queryable;
using URF.Core.EF.Tests.Contexts;
using URF.Core.EF.Tests.Models;
using Xunit;

namespace URF.Core.EF.Tests
{
    [Collection(nameof(NorthwindDbContext))]
    public class QueryableRepositoryTest
    {
        private readonly NorthwindDbContextFixture _fixture;

        public QueryableRepositoryTest(NorthwindDbContextFixture fixture)
        {
            var categories = new List<Category>
            {
                new Category { CategoryId = 1, CategoryName = "Beverages"},
            };
            var products = new List<Product>
            {
                new Product { ProductId = 1, ProductName = "Product 1", UnitPrice = 10, CategoryId = 1 },
                new Product { ProductId = 2, ProductName = "Product 2", UnitPrice = 20, CategoryId = 1 },
                new Product { ProductId = 3, ProductName = "Product 3", UnitPrice = 30, CategoryId = 1 },
            };
            _fixture = fixture;
            _fixture.Initialize(true, () =>
            {
                _fixture.Context.Categories.AddRange(categories);
                _fixture.Context.Products.AddRange(products);
                _fixture.Context.SaveChanges();
            });
        }

        [Fact]
        public async Task Queryable_Should_Allow_Composition()
        {
            // Arrange
            var comparer = new MyProductComparer();
            var expected1 = new MyProduct {Id = 2, Name = "Product 2", Price = 20, Category = "Beverages"};
            var expected2 = new MyProduct { Id = 3, Name = "Product 3", Price = 30, Category = "Beverages" };
            var repository = new QueryableRepository<Product>(_fixture.Context);

            // Act
            var query = repository.Queryable();
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
