using System.Collections.Generic;
using System.Threading.Tasks;
using URF.Core.EF.Tests.Contexts;
using URF.Core.EF.Tests.Models;
using Xunit;

namespace URF.Core.EF.Tests
{
    [Collection(nameof(NorthwindDbContext))]
    public class RepositoryTests
    {
        private readonly List<Product> _products;
        private readonly NorthwindDbContextFixture _fixture;

        public RepositoryTests(NorthwindDbContextFixture fixture)
        {
            _products = new List<Product>
            {
                new Product { ProductId = 1, ProductName = "Product 1", UnitPrice = 10, CategoryId = 1 },
                new Product { ProductId = 2, ProductName = "Product 2", UnitPrice = 20, CategoryId = 1 },
                new Product { ProductId = 3, ProductName = "Product 3", UnitPrice = 30, CategoryId = 1 }
            };
            _fixture = fixture;
            _fixture.Initialize(true, async () =>
            {
                _fixture.Context.Categories.Add(new Category { CategoryId = 1, CategoryName = "Beverages"});
                _fixture.Context.Products.AddRange(_products);
                await _fixture.Context.SaveChangesAsync();
            });
        }

        [Fact]
        public async Task SelectAsync_Should_Return_Entities()
        {
            // Arrange
            var repository = new Repository<Product>(_fixture.Context);

            // Act
            var products = await repository.SelectAsync();

            // Assert
            Assert.Collection(products,
                p => Assert.Equal(_products[0].ProductId, p.ProductId),
                p => Assert.Equal(_products[1].ProductId, p.ProductId),
                p => Assert.Equal(_products[2].ProductId, p.ProductId));
        }

        [Fact]
        public async Task SelectSqlAsync_Should_Return_Entities()
        {
            // Arrange
            var repository = new Repository<Product>(_fixture.Context);

            // Act
            var products = await repository.SelectSqlAsync("SELECT * FROM Products");

            // Assert
            Assert.Collection(products,
                p => Assert.Equal(_products[0].ProductId, p.ProductId),
                p => Assert.Equal(_products[1].ProductId, p.ProductId),
                p => Assert.Equal(_products[2].ProductId, p.ProductId));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task FindAsync_Should_Return_Entity(bool useKey)
        {
            // Arrange
            var repository = new Repository<Product>(_fixture.Context);

            // Act
            Product product;
            if (useKey)
                product = await repository.FindAsync(1);
            else
                product = await repository.FindAsync(new object[] {1});

            // Assert
            Assert.Equal(_products[0].ProductId, product.ProductId);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task ExistsAsync_Should_Return_True(bool useKey)
        {
            // Arrange
            var repository = new Repository<Product>(_fixture.Context);

            // Act
            bool result;
            if (useKey)
                result = await repository.ExistsAsync(1);
            else
                result = await repository.ExistsAsync(new object[] {1});

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task ExistsAsync_Should_Return_False(bool useKey)
        {
            // Arrange
            var repository = new Repository<Product>(_fixture.Context);

            // Act
            bool result;
            if (useKey)
                result = await repository.ExistsAsync(-1);
            else
                result = await repository.ExistsAsync(new object[] { -1 });

            // Assert
            Assert.False(result);
        }
    }
}
