using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using URF.Core.Abstractions;

namespace URF.Core.Mongo
{
    public class DocumentRepository<TEntity> : IDocumentRepository<TEntity> where TEntity : class
    {
        protected IMongoCollection<TEntity> Collection { get; }

        public DocumentRepository(IMongoCollection<TEntity> collection)
            => Collection = collection;

        public virtual async Task<List<TEntity>> FindManyAsync(CancellationToken cancellationToken = default)
            => await Collection.Find(e => true).ToListAsync(cancellationToken);

        public virtual async Task<List<TEntity>> FindManyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
            => await Collection.Find(filter).ToListAsync(cancellationToken);

        public virtual async Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
            => await Collection.Find(filter).SingleOrDefaultAsync(cancellationToken);

        public virtual async Task<TEntity> FindOneAndReplaceAsync(Expression<Func<TEntity, bool>> filter, TEntity item, CancellationToken cancellationToken = default)
        {
            await Collection.FindOneAndReplaceAsync(filter, item, null, cancellationToken);
            return await FindOneAsync(filter, cancellationToken);
        }

        public virtual async Task<List<TEntity>> InsertManyAsync(IEnumerable<TEntity> items, CancellationToken cancellationToken = default)
        {
            await Collection.InsertManyAsync(items, null, cancellationToken);
            return items.ToList();
        }

        public virtual async Task<TEntity> InsertOneAsync(TEntity item, CancellationToken cancellationToken = default)
        {
            await Collection.InsertOneAsync(item, null, cancellationToken);
            return item;
        }

        public virtual async Task<int> DeleteManyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
        {
            var result = await Collection.DeleteManyAsync(filter, cancellationToken);
            return (int)result.DeletedCount;
        }

        public virtual async Task<int> DeleteOneAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
        {
            var result = await Collection.DeleteOneAsync(filter, cancellationToken);
            return (int)result.DeletedCount;
        }

        public virtual IQueryable<TEntity> Queryable()
            => Collection.AsQueryable();
    }
}
