using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Urf.Core.Abstractions;

namespace URF.Core.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        protected DbContext Context { get; }
        protected ConcurrentDictionary<Type, object> Repositories { get; }

        public UnitOfWork(DbContext context)
        {
            Context = context;
            Repositories = new ConcurrentDictionary<Type, object>();
        }

        public virtual IRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            if (Repositories.TryGetValue(typeof(IRepository<TEntity>), out var repository) && repository is IRepository<TEntity>)
                return (IRepository<TEntity>) repository;
            var repository1 = (IRepository<TEntity>)Activator.CreateInstance(typeof(Repository<TEntity>), Context);
            Repositories.TryAdd(typeof(IRepository<TEntity>), repository1);
            return repository1;
        }

        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await Context.SaveChangesAsync(cancellationToken);

        public virtual async Task<int> ExecuteSqlCommandAsync(string sql, IEnumerable<object> parameters, CancellationToken cancellationToken = default)
            => await Context.Database.ExecuteSqlCommandAsync(sql, parameters, cancellationToken);
    }
}