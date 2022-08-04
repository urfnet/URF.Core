using System.Threading.Tasks;
using URF.Core.Abstractions;
using URF.Core.Abstractions.Trackable;
using URF.Core.EF.Tests.Contexts;
using URF.Core.EF.Tests.Models;
using URF.Core.EF.Tests.Services;
using URF.Core.EF.Trackable;
using Xunit;

namespace URF.Core.EF.Tests
{
    [Collection(nameof(NorthwindDbContext))]
    public class ServiceTest
    {
        private readonly NorthwindDbContextFixture _fixture;

        public ServiceTest(NorthwindDbContextFixture fixture)
        {
            var orders = Factory.Orders();
            var products = Factory.Products();
            var categories = Factory.Categories();
            var customers = Factory.Customers();
            var ordersDetails = Factory.OrderDetails();

            _fixture = fixture;
            _fixture.Initialize(true, () =>
            {
                _fixture.Context.Categories.AddRange(categories);
                _fixture.Context.Products.AddRange(products);
                _fixture.Context.Customers.AddRange(customers);
                _fixture.Context.Orders.AddRange(orders);
                _fixture.Context.OrderDetails.AddRange(ordersDetails);
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
