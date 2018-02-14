using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Urf.Core.Abstractions;
using URF.Core.EF.Tests.Contexts;
using URF.Core.EF.Tests.Models;
using Xunit;

namespace URF.Core.EF.Tests
{
    [Collection(nameof(NorthwindDbContext))]
    public class UnitOfWorkTest
    {
        private readonly NorthwindDbContextFixture _fixture;

        public UnitOfWorkTest(NorthwindDbContextFixture fixture)
        {
            _fixture = fixture;
            _fixture.Initialize();
        }

        [Fact]
        public async Task SaveChangesAsync_Should_Save_Changes()
        {
            // Arrange
            var productId = 1;
            var category = new Category { CategoryId = 1, CategoryName = "Beverages" };
            var product = new Product
            {
                ProductId = productId,
                ProductName = "Product 1",
                UnitPrice = 10,
                CategoryId = 1
            };
            _fixture.Context.Categories.Add(category);
            _fixture.Context.Products.Add(product);
            var unitOfWork = new UnitOfWork(_fixture.Context);

            // Act
            var affected = await unitOfWork.SaveChangesAsync();

            // Assert
            Assert.Equal(2, affected);
            _fixture.Context.Entry(product).State = EntityState.Detached;
            var product1 = await _fixture.Context.Products.FindAsync(productId);
            Assert.NotNull(product1);
        }

        [Fact]
        public async Task ExecuteSqlCommandAsync_Should_Execute_Sql()
        {
            // Arrange
            var productId = 1;
            var category = new Category {CategoryId = 1, CategoryName = "Beverages"};
            var product = new Product
            {
                ProductId = productId,
                ProductName = "Product 1",
                UnitPrice = 10,
                CategoryId = 1
            };
            _fixture.Context.Categories.Add(category);
            _fixture.Context.Products.Add(product);
            await _fixture.Context.SaveChangesAsync();
            var unitOfWork = new UnitOfWork(_fixture.Context);

            // Act
            var price = 50;
            var affected = await unitOfWork.ExecuteSqlCommandAsync(
                "UPDATE Products SET UnitPrice = {0} WHERE ProductId = {1}",
                new object[] { price, productId });

            // Assert
            Assert.Equal(1, affected);
            _fixture.Context.Entry(product).State = EntityState.Detached;
            var product1 = await _fixture.Context.Products.FindAsync(productId);
            Assert.Equal(price, product1.UnitPrice);
        }
    }
}