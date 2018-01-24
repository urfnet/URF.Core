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
        private readonly List<Category> _categories;
        private readonly List<Product> _products;
        private readonly NorthwindDbContextFixture _fixture;

        public RepositoryTests(NorthwindDbContextFixture fixture)
        {
            _categories = new List<Category>
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
            _fixture.Initialize(true, async () =>
            {
                _fixture.Context.Categories.AddRange(_categories);
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

        [Fact]
        public async Task LoadPropertyAsync_Should_Load_Property()
        {
            // Arrange
            var product = new Product
            {
                ProductId = 4,
                ProductName = "Product 4",
                UnitPrice = 40,
                CategoryId = 1
            };
            var repository = new Repository<Product>(_fixture.Context);
            repository.Attach(product);

            // Act
            await repository.LoadPropertyAsync(product, p => p.Category);

            // Assert
            Assert.NotNull(product.Category);
        }
    }
}
