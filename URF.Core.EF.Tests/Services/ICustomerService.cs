using System.Collections.Generic;
using System.Threading.Tasks;
using URF.Core.EF.Tests.Models;

namespace URF.Core.EF.Tests.Services
{
    public interface ICustomerService
    {
        Task<decimal> CustomerOrderTotalByYear(string customerId, int year);
        Task<IEnumerable<Customer>> CustomersByCompany(string companyName);
        Task<IEnumerable<CustomerOrder>> GetCustomerOrder(string country);
    }
}