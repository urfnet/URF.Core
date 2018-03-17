using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrackableEntities.Common.Core;
using URF.Core.Abstractions.Trackable;
using URF.Core.EF.Tests.Contexts;
using URF.Core.EF.Tests.Models;
using URF.Core.EF.Trackable;
using Xunit;

namespace URF.Core.EF.Tests
{
    [Collection(nameof(NorthwindDbContext))]
    public class TrackableRepositoryTest
    {
        private readonly NorthwindDbContextFixture _fixture;
        private readonly UnitOfWork _unitOfWork;

        public TrackableRepositoryTest(NorthwindDbContextFixture fixture)
        {       
            _fixture = fixture;
            _fixture.Initialize(true, () =>
            {
                _fixture.Context.Categories.AddRange(Factory.Categories());
                _fixture.Context.Products.AddRange(Factory.Products());
                _fixture.Context.Customers.AddRange(Factory.Customers());
                _fixture.Context.Orders.AddRange(Factory.Orders());
                _fixture.Context.OrderDetails.AddRange(Factory.OrderDetails());
                _fixture.Context.SaveChanges();
            });

            _unitOfWork = new UnitOfWork(_fixture.Context);
        }

        [Fact]
        public void Insert_Should_Set_Entity_State_to_Added()
        {
            // Arrange
            ITrackableRepository<Product> productsRepo = new TrackableRepository<Product>(_fixture.Context);
            var product = new Product {ProductId = 80, ProductName = "Product 80", UnitPrice = 40, CategoryId = 1};

            // Act
            productsRepo.Insert(product);

            // Assert
            Assert.Equal(TrackingState.Added, product.TrackingState);
        }

        [Fact]
        public void Update_Should_Set_Entity_State_to_Modified()
        {
            // Arrange
            ITrackableRepository<Product> productsRepo = new TrackableRepository<Product>(_fixture.Context);
            var product = new Product { ProductId = 81, ProductName = "Product 81", UnitPrice = 40, CategoryId = 1 };

            // Act
            productsRepo.Update(product);

            // Assert
            Assert.Equal(TrackingState.Modified, product.TrackingState);
        }

        [Fact]
        public void Delete_Should_Set_Entity_State_to_Deleted()
        {
            // Arrange
            ITrackableRepository<Product> productsRepo = new TrackableRepository<Product>(_fixture.Context);
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
            ITrackableRepository<Product> productsRepo = new TrackableRepository<Product>(_fixture.Context);
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
            ITrackableRepository<Order> ordersRepo = new TrackableRepository<Order>(_fixture.Context);
            var order = await ordersRepo.FindAsync(10248);
            ordersRepo.DetachEntities(order);

            // updating object in complex object graph
            order.OrderDetails[0].UnitPrice = 17; 
            order.OrderDetails[0].TrackingState = TrackingState.Modified;

            // updating object in complex object graph
            order.OrderDetails[1].Quantity = 2; 
            order.OrderDetails[1].TrackingState = TrackingState.Modified;

            // deleting object in complex object graph
            order.OrderDetails[2].TrackingState = TrackingState.Deleted;

            // adding object in complex object graph
            var addedDetail = new OrderDetail
            {
                //OrderDetailId = 4,
                OrderId = 1,
                ProductId = 3,
                Product = order.OrderDetails[2].Product,
                UnitPrice = 40,
                Quantity = 400,            
                TrackingState = TrackingState.Added 
            };

            // Act
            order.OrderDetails.Add(addedDetail);

            // Act
            ordersRepo.ApplyChanges(order);

            // Assert
            Assert.Equal(EntityState.Unchanged, _fixture.Context.Entry(order).State);
            Assert.Equal(EntityState.Modified, _fixture.Context.Entry(order.OrderDetails[0]).State);
            Assert.Equal(EntityState.Modified, _fixture.Context.Entry(order.OrderDetails[1]).State);
            Assert.Equal(EntityState.Deleted, _fixture.Context.Entry(order.OrderDetails[2]).State);
            Assert.Equal(EntityState.Added, _fixture.Context.Entry(order.OrderDetails[3]).State);

            // Save changes to object graph with different TrackingStates throughout the object graph
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
