#region

using System.Collections.Generic;
using System.Threading.Tasks;
using Urf.Core.Abstractions;
using URF.Core.Abstractions.Trackable;
using URF.Core.EF.Tests.Contexts;
using URF.Core.EF.Tests.Models;
using URF.Core.EF.Tests.Services;
using URF.Core.EF.Trackable;
using Xunit;

#endregion

namespace URF.Core.EF.Tests
{
    [Collection(nameof(NorthwindDbContext))]
    public class ServiceTest
    {
        private readonly List<Order> _orders;
        private readonly List<Product> _products;
        private readonly List<Category> _categories;
        private readonly List<Customer> _customers;
        private readonly List<OrderDetail> _ordersDetails;

        private readonly NorthwindDbContextFixture _fixture;

        public ServiceTest(NorthwindDbContextFixture fixture)
        {
            _orders = Factory.Orders();
            _products = Factory.Products();
            _categories = Factory.Categories();
            _customers = Factory.Customers();
            _ordersDetails = Factory.OrderDetails();

            _fixture = fixture;
            _fixture.Initialize(true, () =>
            {
                _fixture.Context.Categories.AddRange(_categories);
                _fixture.Context.Products.AddRange(_products);
                _fixture.Context.Customers.AddRange(_customers);
                _fixture.Context.Orders.AddRange(_orders);
                _fixture.Context.OrderDetails.AddRange(_ordersDetails);
                _fixture.Context.SaveChanges();
            });
        }

        [Fact]
        public async Task CustomerOrderTotalByYear_Should_Return_Order_Total()
        {
            // Arrange
            ITrackableRepository<Customer> customerRepository = new TrackableRepository<Customer>(_fixture.Context);
            ITrackableRepository<Order> orderRepository = new TrackableRepository<Order>(_fixture.Context);
            var customerService = new CustomerService(customerRepository, orderRepository);

            // Act
            var customerOrderTotalByYear = await customerService.CustomerOrderTotalByYear("ALFKI", 1998);

            // Assert
            Assert.Equal(2302.2000m, customerOrderTotalByYear);
        }

        [Fact]
        public async Task CustomersByCompany_Should_Return_Customer()
        {
            // Arrange
            ITrackableRepository<Customer> customerRepository = new TrackableRepository<Customer>(_fixture.Context);
            ITrackableRepository<Order> orderRepository = new TrackableRepository<Order>(_fixture.Context);
            var customerService = new CustomerService(customerRepository, orderRepository);
            const string company = "Alfreds Futterkiste";

            // Act
            var customers = await customerService.CustomersByCompany(company);

            // Assert
            Assert.Collection(customers, customer
                => Assert.Equal("ALFKI", customer.CustomerId));
        }


        [Fact]
        public async Task Insert_Customer_Should_Save_Changes()
        {
            IUnitOfWork unitOfWork = new UnitOfWork(_fixture.Context);
            ITrackableRepository<Customer> customerRepository = new TrackableRepository<Customer>(_fixture.Context);
            ITrackableRepository<Order> orderRepository = new TrackableRepository<Order>(_fixture.Context);
            var customerService = new CustomerService(customerRepository, orderRepository);

            var cust = new Customer
            {
                CustomerId = "COMP1",
                CompanyName = "Company 1"
            };

            customerService.Insert(cust);
            Assert.Equal(TrackableEntities.Common.Core.TrackingState.Added, cust.TrackingState);

            var savedChanges = await unitOfWork.SaveChangesAsync();
            Assert.Equal<int>(1, savedChanges);
        }
    }
}