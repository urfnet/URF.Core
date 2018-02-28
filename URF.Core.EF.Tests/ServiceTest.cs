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
        public async Task Service_Insert_Should_Insert_Into_Database()
        {
            // Arrange
            IUnitOfWork unitOfWork = new UnitOfWork(_fixture.Context);
            ITrackableRepository<Customer> customerRepository = new TrackableRepository<Customer>(_fixture.Context);
            ITrackableRepository<Order> orderRepository = new TrackableRepository<Order>(_fixture.Context);
            var customerService = new CustomerService(customerRepository, orderRepository);

            const string customerId = "COMP1";
            const string companyName = "Company 1";

            var customer = new Customer
            {
                CustomerId = customerId,
                CompanyName = companyName
            };

            // Act
            customerService.Insert(customer);

            // Assert
            Assert.Equal(TrackableEntities.Common.Core.TrackingState.Added, customer.TrackingState);

            // Act
            var savedChanges = await unitOfWork.SaveChangesAsync();

            // Assert
            Assert.Equal(1, savedChanges);

            // Act
            var newCustomer = await customerRepository.FindAsync(customerId);

            // Assert
            Assert.Equal(newCustomer.CustomerId, customerId);
            Assert.Equal(newCustomer.CompanyName, companyName);
        }
    }
}
