using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace URF.Core.Abstractions
{
    public interface IRepositoryFluent<TEntity> where TEntity : class
    {
        IRepositoryFluent<TEntity> Filter(Expression<Func<TEntity, bool>> filter);
        IRepositoryFluent<TEntity> Include(Expression<Func<TEntity, object>> include);
        IRepositoryFluent<TEntity> Page(int page);
        IRepositoryFluent<TEntity> PageSize(int pageSize);
        Task<IEnumerable<TEntity>> SelectAsync(CancellationToken cancellationToken = default );
        IRepositoryFluent<TEntity> OrderBy(Expression<Func<TEntity, object>> sortBy);
        IRepositoryFluent<TEntity> OrderByDescending(Expression<Func<TEntity, object>> sortBy);
    }
}