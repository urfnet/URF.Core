using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Urf.Core.Abstractions;
using URF.Core.Abstractions;

namespace URF.Core.Abstractions.Services
{
    public abstract class Service<TEntity> : IService<TEntity> where TEntity : class
    {
        protected readonly IRepository<TEntity> Repository;

        protected Service(IRepository<TEntity> repository)
            => Repository = repository;

        public virtual void Attach(TEntity item)
            => Repository.Attach(item);

        public virtual void Delete(TEntity item)
            => Repository.Delete(item);

        public virtual Task<bool> DeleteAsync(object[] keyValues, CancellationToken cancellationToken = default) 
            => Repository.DeleteAsync(keyValues);

        public virtual Task<bool> DeleteAsync<TKey>(TKey keyValue, CancellationToken cancellationToken = default)
            => Repository.DeleteAsync(keyValue, cancellationToken);

        public virtual void Detach(TEntity item)
            => Repository.Detach(item);

        public virtual Task<bool> ExistsAsync(object[] keyValues, CancellationToken cancellationToken = default)
            => Repository.ExistsAsync(keyValues, cancellationToken);

        public virtual Task<bool> ExistsAsync<TKey>(TKey keyValue, CancellationToken cancellationToken = default) 
            => Repository.ExistsAsync(keyValue, cancellationToken);

        public virtual Task<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken = default) 
            => Repository.FindAsync(keyValues, cancellationToken);

        public virtual Task<TEntity> FindAsync<TKey>(TKey keyValue, CancellationToken cancellationToken = default) 
            => Repository.FindAsync(keyValue, cancellationToken);

        public virtual void Insert(TEntity item)
            => Repository.Insert(item);

        public virtual Task LoadPropertyAsync(TEntity item, Expression<Func<TEntity, object>> property, CancellationToken cancellationToken = default) 
            => Repository.LoadPropertyAsync(item, property, cancellationToken);

        public virtual IQuery<TEntity> Query()
            => Repository.Query();

        public virtual IQueryable<TEntity> Queryable() 
            => Repository.Queryable();

        public virtual IQueryable<TEntity> QueryableSql(string sql, params object[] parameters) 
            => Repository.QueryableSql(sql, parameters);

        public virtual Task<IEnumerable<TEntity>> SelectSqlAsync(string sql, object[] parameters, CancellationToken cancellationToken = default) 
            => Repository.SelectSqlAsync(sql, parameters, cancellationToken);

        public virtual void Update(TEntity item) 
            => Repository.Update(item);
    }
}