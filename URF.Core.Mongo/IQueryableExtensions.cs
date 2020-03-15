using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace URF.Core.Mongo
{
    public static class IQueryableExtensions
    {
        public static Task<bool> AnyAsync<TEntity>(this IQueryable<TEntity> source, CancellationToken cancellationToken = default)
            => MongoQueryable.AnyAsync((IMongoQueryable<TEntity>)source, cancellationToken);
        public static Task<int> CountAsync<TEntity>(this IQueryable<TEntity> source, CancellationToken cancellationToken = default)
            => MongoQueryable.CountAsync((IMongoQueryable<TEntity>)source, cancellationToken);
        public static Task<long> LongCountAsync<TEntity>(this IQueryable<TEntity> source, CancellationToken cancellationToken = default)
            => MongoQueryable.LongCountAsync((IMongoQueryable<TEntity>)source, cancellationToken);
        public static Task<TEntity> FirstAsync<TEntity>(this IQueryable<TEntity> source, CancellationToken cancellationToken = default)
            => MongoQueryable.FirstAsync((IMongoQueryable<TEntity>)source, cancellationToken);
        public static Task<TEntity> FirstOrDefaultAsync<TEntity>(this IQueryable<TEntity> source, CancellationToken cancellationToken = default)
            => MongoQueryable.FirstOrDefaultAsync((IMongoQueryable<TEntity>)source, cancellationToken);
        public static Task<TEntity> MaxAsync<TEntity>(this IQueryable<TEntity> source, CancellationToken cancellationToken = default)
            => MongoQueryable.MaxAsync((IMongoQueryable<TEntity>)source, cancellationToken);
        public static Task<TResult> MaxAsync<TEntity, TResult>(this IQueryable<TEntity> source, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default)
            => MongoQueryable.MaxAsync((IMongoQueryable<TEntity>)source, selector, cancellationToken);
        public static Task<TEntity> MinAsync<TEntity>(this IQueryable<TEntity> source, CancellationToken cancellationToken = default)
            => MongoQueryable.MinAsync((IMongoQueryable<TEntity>)source, cancellationToken);
        public static Task<TResult> MinAsync<TEntity, TResult>(this IQueryable<TEntity> source, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default)
            => MongoQueryable.MinAsync((IMongoQueryable<TEntity>)source, selector, cancellationToken);
        public static Task<TEntity> SingleAsync<TEntity>(this IQueryable<TEntity> source, CancellationToken cancellationToken = default)
            => MongoQueryable.SingleAsync((IMongoQueryable<TEntity>)source, cancellationToken);
        public static Task<TEntity> SingleOrDefaultAsync<TEntity>(this IQueryable<TEntity> source, CancellationToken cancellationToken = default)
            => MongoQueryable.SingleOrDefaultAsync((IMongoQueryable<TEntity>)source, cancellationToken);
        public static Task<List<TEntity>> ToListAsync<TEntity>(this IQueryable<TEntity> source, CancellationToken cancellationToken = default)
            => IAsyncCursorSourceExtensions.ToListAsync((IMongoQueryable<TEntity>)source, cancellationToken);
    }
}
