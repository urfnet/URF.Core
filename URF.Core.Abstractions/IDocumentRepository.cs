using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace URF.Core.Abstractions
{
    public interface IDocumentRepository<TEntity> where TEntity : class
    {
        Task<List<TEntity>> FindManyAsync(CancellationToken cancellationToken = default);

        Task<List<TEntity>> FindManyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

        Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

        Task<TEntity> FindOneAndReplaceAsync(Expression<Func<TEntity, bool>> filter, TEntity item, CancellationToken cancellationToken = default);

        Task<TEntity> InsertOneAsync(TEntity item, CancellationToken cancellationToken = default);

        Task<List<TEntity>> InsertManyAsync(IEnumerable<TEntity> items, CancellationToken cancellationToken = default);

        Task<int> DeleteOneAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

        Task<int> DeleteManyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

        IQueryable<TEntity> Queryable();
    }
}