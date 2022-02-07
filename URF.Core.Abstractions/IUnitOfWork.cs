using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace URF.Core.Abstractions
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<int> ExecuteSqlCommandAsync(string sql, System.Collections.Generic.IEnumerable<object> parameters, CancellationToken cancellationToken = default );
    }
}