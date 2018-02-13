using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrackableEntities.Common.Core;
using URF.Core.EF.Tests.Contexts;
using URF.Core.EF.Tests.Models;
using URF.Core.EF.Trackable;
using Xunit;

namespace URF.Core.EF.Tests
{
    [Collection(nameof(NorthwindDbContext))]
    public class TrackableRepositoryTest
    {
        private readonly TrackableUnitOfWork _unitOfWork;
        private readonly NorthwindDbContextFixture _fixture;

        public TrackableRepositoryTest(NorthwindDbContextFixture fixture)
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
            var customer = new Customer
            {
                CustomerId = "ALFKI",
                ContactName = "Maria Anders"
            };
            var order = new Order
            {
                OrderId = 1,
                OrderDate = DateTime.Today,
                CustomerId = "ALFKI",
                Customer = customer,
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail { OrderDetailId = 1, OrderId = 1, ProductId = 1, Product = products[0], UnitPrice = 10, Quantity = 100},
                    new OrderDetail { OrderDetailId = 2, OrderId = 1, ProductId = 2, Product = products[1], UnitPrice = 20, Quantity = 200},
                    new OrderDetail { OrderDetailId = 3, OrderId = 1, ProductId = 3, Product = products[2], UnitPrice = 30, Quantity = 300},
                }
            };

            _fixture = fixture;
            _fixture.Initialize(true, () =>
            {
                _fixture.Context.Categories.AddRange(categories);
                _fixture.Context.Products.AddRange(products);
                _fixture.Context.Customers.Add(customer);
                _fixture.Context.Orders.Add(order);
                _fixture.Context.SaveChanges();
            });
            _unitOfWork = new TrackableUnitOfWork(_fixture.Context);
        }

        [Fact]
        public void Insert_Should_Set_Entity_State_to_Added()
        {
            // Arrange
            var productsRepo = _unitOfWork.TrackableRepository<Product>();
            var product = new Product {ProductId = 4, ProductName = "Product 4", UnitPrice = 40, CategoryId = 1};

            // Act
            productsRepo.Insert(product);

            // Assert
            Assert.Equal(TrackingState.Added, product.TrackingState);
        }

        [Fact]
        public void Update_Should_Set_Entity_State_to_Modified()
        {
            // Arrange
            var productsRepo = _unitOfWork.TrackableRepository<Product>();
            var product = new Product { ProductId = 4, ProductName = "Product 4", UnitPrice = 40, CategoryId = 1 };

            // Act
            productsRepo.Update(product);

            // Assert
            Assert.Equal(TrackingState.Modified, product.TrackingState);
        }

        [Fact]
        public void Delete_Should_Set_Entity_State_to_Deleted()
        {
            // Arrange
            var productsRepo = _unitOfWork.TrackableRepository<Product>();
            var product = new Product { ProductId = 4, ProductName = "Product 4", UnitPrice = 40, CategoryId = 1 };

            // Act
            productsRepo.Delete(product);

            // Assert
            Assert.Equal(TrackingState.Deleted, product.TrackingState);
        }

        [Fact]
        public async Task DeleteAsync_Should_Set_Entity_State_to_Deleted()
        {
            // Arrange
            var productsRepo = _unitOfWork.TrackableRepository<Product>();
            var product = await productsRepo.FindAsync(1);

            // Act
            await productsRepo.DeleteAsync(1);

            // Assert
            Assert.Equal(TrackingState.Deleted, product.TrackingState);
        }

        [Fact]
        public async Task ApplyChanges_Should_Set_Graph_States()
        {
            // Arrange
            var ordersRepo = _unitOfWork.TrackableRepository<Order>();
            var order = await ordersRepo.FindAsync(1);
            ordersRepo.DetachEntities(order);

            order.OrderDetails[0].TrackingState = TrackingState.Unchanged;
            order.OrderDetails[1].TrackingState = TrackingState.Modified;
            order.OrderDetails[2].TrackingState = TrackingState.Deleted;
            var addedDetail = new OrderDetail
            {
                OrderDetailId = 4,
                OrderId = 1,
                ProductId = 3,
                Product = order.OrderDetails[2].Product,
                UnitPrice = 40,
                Quantity = 400,
                TrackingState = TrackingState.Added
            };
            order.OrderDetails.Add(addedDetail);

            // Act
            ordersRepo.ApplyChanges(order);

            // Assert
            Assert.Equal(EntityState.Unchanged, _fixture.Context.Entry(order).State);
            Assert.Equal(EntityState.Unchanged, _fixture.Context.Entry(order.OrderDetails[0]).State);
            Assert.Equal(EntityState.Modified, _fixture.Context.Entry(order.OrderDetails[1]).State);
            Assert.Equal(EntityState.Deleted, _fixture.Context.Entry(order.OrderDetails[2]).State);
            Assert.Equal(EntityState.Added, _fixture.Context.Entry(order.OrderDetails[3]).State);
        }
    }
}
