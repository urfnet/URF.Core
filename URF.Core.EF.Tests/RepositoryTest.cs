using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using URF.Core.Abstractions;
using URF.Core.EF.Tests.Contexts;
using URF.Core.EF.Tests.Models;
using Xunit;

namespace URF.Core.EF.Tests
{
    [Collection(nameof(NorthwindDbContext))]
    public class RepositoryTest
    {
        private readonly List<Category> _categories;
        private readonly List<Product> _products;
        private readonly NorthwindDbContextFixture _fixture;

        public RepositoryTest(NorthwindDbContextFixture fixture)
        {
            _categories = Factory.Categories();
            _products = Factory.Products();

            _fixture = fixture;
            _fixture.Initialize(true, () =>
            {
                _fixture.Context.Categories.AddRange(_categories);
                _fixture.Context.Products.AddRange(_products);
                _fixture.Context.SaveChanges();
            });
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
                result = await repository.ExistsAsync(new object[] {-1});

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Attach_Should_Set_Entity_State_Unchanged(bool attach)
        {
            // Arrange
            var product = new Product
            {
                ProductId = 80,
                ProductName = "Product 80",
                UnitPrice = 40,
                CategoryId = 1
            };

            var repository = new Repository<Product>(_fixture.Context);

            // Act
            if (attach)
                repository.Attach(product);

            // Assert
            Assert.Equal(attach ? EntityState.Unchanged : EntityState.Detached, _fixture.Context.Entry(product).State);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Detach_Should_Set_Entity_State_Detached(bool detach)
        {
            // Arrange
            var product = new Product
            {
                ProductId = 84,
                ProductName = "Product 84",
                UnitPrice = 40,
                CategoryId = 1
            };

            _fixture.Context.Products.Attach(product);

            var repository = new Repository<Product>(_fixture.Context);

            // Act
            if (detach)
                repository.Detach(product);

            // Assert
            Assert.Equal(detach ? EntityState.Detached : EntityState.Unchanged, _fixture.Context.Entry(product).State);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task DeleteAsync_Should_Set_Entity_State_Deleted(bool useKey)
        {
            // Arrange
            var repository = new Repository<Product>(_fixture.Context);

            // Act
            bool result;
            if (useKey)
                result = await repository.DeleteAsync(1);
            else
                result = await repository.DeleteAsync(new object[] {1});

            // Assert
            Assert.True(result);
            Assert.Equal(EntityState.Deleted, _fixture.Context.Entry(_products[0]).State);
        }

        [Fact]
        public void Insert_Should_Set_Entity_State_Added()
        {
            // Arrange
            var product = new Product
            {
                ProductId = 84,
                ProductName = "Product 84",
                UnitPrice = 40,
                CategoryId = 1
            };

            var repository = new Repository<Product>(_fixture.Context);

            // Act
            repository.Insert(product);

            // Assert
            Assert.Equal(EntityState.Added, _fixture.Context.Entry(product).State);
        }

        [Fact]
        public async Task LoadPropertyAsync_Should_Load_Property()
        {
            // Arrange
            var product = new Product
            {
                ProductId = 80,
                ProductName = "Product 80",
                UnitPrice = 40,
                CategoryId = 1
            };
            _fixture.Context.Products.Attach(product);
            var repository = new Repository<Product>(_fixture.Context);

            // Act
            await repository.LoadPropertyAsync(product, p => p.Category);

            // Assert
            Assert.Same(_categories[0], product.Category);
        }       

        [Fact]
        public async Task Fluent_Api_Should_Support_Paging()
        {
            var repository = new Repository<Product>(_fixture.Context);

            // Arrange
            var expected = new[]
            {
                new Product {ProductId = 67, ProductName = "Laughing Lumberjack Lager", CategoryId = 1, UnitPrice = 14.00m, Discontinued = false},
                new Product {ProductId = 76, ProductName = "Lakkalikööri", CategoryId = 1, UnitPrice = 18.00m, Discontinued = false},
                new Product {ProductId = 43, ProductName = "Ipoh Coffee", CategoryId = 1, UnitPrice = 46.00m, Discontinued = false},
                new Product {ProductId = 44, ProductName = "Gula Malacca", CategoryId = 2, UnitPrice = 19.45m, Discontinued = false},
                new Product {ProductId = 24, ProductName = "Guaraná Fantástica", CategoryId = 1, UnitPrice = 4.50m, Discontinued = true},
                new Product {ProductId = 6, ProductName = "Grandma's Boysenberry Spread", CategoryId = 2, UnitPrice = 25.00m, Discontinued = false},
                new Product {ProductId = 15, ProductName = "Genen Shouyu", CategoryId = 2, UnitPrice = 15.50m, Discontinued = false},
                new Product {ProductId = 38, ProductName = "Côte de Blaye", CategoryId = 1, UnitPrice = 263.50m, Discontinued = false},
                new Product {ProductId = 5, ProductName = "Chef Anton's Gumbo Mix", CategoryId = 2, UnitPrice = 21.35m, Discontinued = true},
                new Product {ProductId = 4, ProductName = "Chef Anton's Cajun Seasoning", CategoryId = 2, UnitPrice = 22.00m, Discontinued = false}
            };

            const int page = 2; 
            const int pageSize = 10; 

            // Act
            var count = await repository
                .Query()
                .Where(p => p.CategoryId == 1 || p.CategoryId == 2)
                .CountAsync();

            // Act
            var products = await repository
                .Query()
                .Where(p => p.CategoryId == 1 || p.CategoryId == 2)
                .Include(p => p.Category)
                .OrderByDescending(p => p.ProductName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .SelectAsync();

            var enumerable = products as Product[] ?? products.ToArray();

            const int assertionCount = 24;

            // Assert
            Action<Product>[] collectionAssertions = {
                p => Assert.Equal(expected[0].ProductId, p.ProductId),
                p => Assert.Equal(expected[1].ProductId, p.ProductId),
                p => Assert.Equal(expected[2].ProductId, p.ProductId),
                p => Assert.Equal(expected[3].ProductId, p.ProductId),
                p => Assert.Equal(expected[4].ProductId, p.ProductId),
                p => Assert.Equal(expected[5].ProductId, p.ProductId),
                p => Assert.Equal(expected[6].ProductId, p.ProductId),
                p => Assert.Equal(expected[7].ProductId, p.ProductId),
                p => Assert.Equal(expected[8].ProductId, p.ProductId),
                p => Assert.Equal(expected[9].ProductId, p.ProductId)
            };

            // Assert
            Assert.NotEmpty(enumerable);
            Assert.Equal(assertionCount, count);
            Assert.Equal(pageSize, enumerable.Length);
            Assert.Collection(enumerable, collectionAssertions);

            // Act
            var paginated = new Page<Product>(count, enumerable);

            // Assert
            Assert.Equal(assertionCount, paginated.Count);
            Assert.Collection(paginated.Value, collectionAssertions);
        }

        [Fact]
        public async Task Queryable_Should_Allow_Composition()
        {
            // Arrange
            var comparer = new MyProductComparer();
            var expected1 = new MyProduct {Id = 1, Name = "Chai", Price = 18.00m, Category = "Beverages"};
            var expected2 = new MyProduct {Id = 2, Name = "Chang", Price = 19.00m, Category = "Beverages"};

            var repository = new Repository<Product>(_fixture.Context);

            // Act
            var query = repository.Queryable();
            var products = await query
                .Take(2)
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

        [Fact]
        public async Task SelectAsync_Should_Return_Entities()
        {
            // Arrange
            var repository = new Repository<Product>(_fixture.Context);

            // Act
            var products = await repository.Query().SelectAsync();
            var enumerable = products as Product[] ?? products.ToArray();

            // Assert
            Assert.Equal(77, enumerable.Length);
        }

        [Fact]
        public void Update_Should_Set_Entity_State_Modified()
        {
            // Arrange
            var product = new Product
            {
                ProductId = 84,
                ProductName = "Product 84",
                UnitPrice = 40,
                CategoryId = 1
            };
            var repository = new Repository<Product>(_fixture.Context);

            // Act
            repository.Update(product);

            // Assert
            Assert.Equal(EntityState.Modified, _fixture.Context.Entry(product).State);
        }
    }
}