using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace URF.Core.Mongo
{
    public class MongoDocumentRepository<TEntity> : DocumentRepository<TEntity> where TEntity : class
    {
        public MongoDocumentRepository(IMongoCollection<TEntity> collection) : base(collection) { }

        public virtual async Task<TEntity> FindOneAsync(FilterDefinition<TEntity> filter, CancellationToken cancellationToken = default)
             => await Collection.Find(filter).SingleOrDefaultAsync(cancellationToken);

        public virtual async Task<TEntity> FindOneAndUpdateAsync(Expression<Func<TEntity, bool>> filter, UpdateDefinition<TEntity> update, CancellationToken cancellationToken = default)
        {
            await Collection.FindOneAndUpdateAsync(filter, update, null, cancellationToken);
            return await FindOneAsync(filter, cancellationToken);
        }

        public virtual async Task<TEntity> FindOneAndUpdateAsync(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update, CancellationToken cancellationToken = default)
        {
            await Collection.FindOneAndUpdateAsync(filter, update, null, cancellationToken);
            return await FindOneAsync(filter, cancellationToken);
        }

        public virtual async Task<int> DeleteOneAsync(FilterDefinition<TEntity> filter, CancellationToken cancellationToken = default)
        {
            var result = await Collection.DeleteOneAsync(filter, cancellationToken);
            return (int)result.DeletedCount;
        }
    }
}
