using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using URF.Core.Abstractions.Trackable;
using URF.Core.EF.Tests.Models;
using URF.Core.Services;

namespace URF.Core.EF.Tests.Services
{
    public class CustomerService : Service<Customer>, ICustomerService
    {
        private readonly ITrackableRepository<Order> _ordeRepository;

        public CustomerService(
            ITrackableRepository<Customer> customerRepository,
            ITrackableRepository<Order> ordeRepository) : base(customerRepository)
        {
            _ordeRepository = ordeRepository;
        }

        public async Task<IEnumerable<Customer>> CustomersByCompany(string companyName)
        {
            return await Repository
                .Queryable()
                .Where(x => x.CompanyName.Contains(companyName))
                .ToListAsync();
        }

        public async Task<decimal> CustomerOrderTotalByYear(string customerId, int year)
        {
            return await Repository
                .Queryable()
                .Where(c => c.CustomerId == customerId)
                .SelectMany(c => c.Orders.Where(o => o.OrderDate != null && o.OrderDate.Value.Year == year))
                .SelectMany(c => c.OrderDetails)
                .Select(c => c.Quantity * c.UnitPrice)
                .SumAsync();
        }

        public async Task<IEnumerable<CustomerOrder>> GetCustomerOrder(string country)
        {
            var customers = Repository.Queryable();
            var orders = _ordeRepository.Queryable();

            var query = from c in customers
                join o in orders on new { a = c.CustomerId, b = c.Country }
                    equals new { a = o.CustomerId, b = country }
                select new CustomerOrder
                {
                    CustomerId = c.CustomerId,
                    ContactName = c.ContactName,
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate
                };

            return await query.ToListAsync();
        }
    }
}
 