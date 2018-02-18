using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using TrackableEntities.Common.Core;
using URF.Core.Abstractions;
using URF.Core.Abstractions.Services;
using URF.Core.Abstractions.Trackable;

namespace URF.Core.Services
{
    public abstract class Service<TEntity> : IService<TEntity> where TEntity : class, ITrackable
    {
        protected readonly ITrackableRepository<TEntity> Repository;

        protected Service(ITrackableRepository<TEntity> repository)
            => Repository = repository;

        public virtual void Attach(TEntity item)
            => Repository.Attach(item);

        public virtual void Delete(TEntity item)
            => Repository.Delete(item);

        public virtual async Task<bool> DeleteAsync(object[] keyValues, CancellationToken cancellationToken = default) 
            => await Repository.DeleteAsync(keyValues, cancellationToken);

        public virtual async Task<bool> DeleteAsync<TKey>(TKey keyValue, CancellationToken cancellationToken = default)
            => await Repository.DeleteAsync(keyValue, cancellationToken);

        public virtual void Detach(TEntity item)
            => Repository.Detach(item);

        public virtual async Task<bool> ExistsAsync(object[] keyValues, CancellationToken cancellationToken = default)
            => await Repository.ExistsAsync(keyValues, cancellationToken);

        public virtual async Task<bool> ExistsAsync<TKey>(TKey keyValue, CancellationToken cancellationToken = default) 
            => await Repository.ExistsAsync(keyValue, cancellationToken);

        public virtual async Task<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken = default) 
            => await Repository.FindAsync(keyValues, cancellationToken);

        public virtual async Task<TEntity> FindAsync<TKey>(TKey keyValue, CancellationToken cancellationToken = default) 
            => await Repository.FindAsync(keyValue, cancellationToken);

        public virtual void Insert(TEntity item)
            => Repository.Insert(item);

        public virtual async Task LoadPropertyAsync(TEntity item, Expression<Func<TEntity, object>> property, CancellationToken cancellationToken = default) 
            => await Repository.LoadPropertyAsync(item, property, cancellationToken);

        public virtual IQuery<TEntity> Query()
            => Repository.Query();

        public virtual IQueryable<TEntity> Queryable() 
            => Repository.Queryable();

        public virtual IQueryable<TEntity> QueryableSql(string sql, params object[] parameters) 
            => Repository.QueryableSql(sql, parameters);

        public virtual async Task<IEnumerable<TEntity>> SelectAsync(CancellationToken cancellationToken = default)
            => await Repository.Query().SelectAsync(cancellationToken);

        public virtual async Task<IEnumerable<TEntity>> SelectSqlAsync(string sql, object[] parameters, CancellationToken cancellationToken = default) 
            => await Repository.Query().SelectSqlAsync(sql, parameters, cancellationToken);

        public virtual void Update(TEntity item) 
            => Repository.Update(item);

        public virtual void ApplyChanges(params TEntity[] entities)
            => Repository.ApplyChanges(entities);

        public virtual void AcceptChanges(params TEntity[] entities)
            => Repository.AcceptChanges(entities);

        public virtual void DetachEntities(params TEntity[] entities)
            => Repository.DetachEntities(entities);

        public virtual async Task LoadRelatedEntities(params TEntity[] entities)
            =>  await Repository.LoadRelatedEntities(entities);
    }
}