﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Urf.Core.Abstractions
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> SelectAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> SelectSqlAsync(string query, CancellationToken cancellationToken = default);
        Task<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken = default);
        Task<TEntity> FindAsync<TKey>(TKey keyValue, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(object[] keyValues, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync<TKey>(TKey keyValue, CancellationToken cancellationToken = default);
        Task LoadPropertyAsync(TEntity item, Expression<Func<TEntity, object>> property, CancellationToken cancellationToken = default);
        void Attach(TEntity item);
        void Detach(TEntity item);
        void Insert(TEntity item);
        void Update(TEntity item);
        Task<bool> DeleteAsync(object[] keyValues, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync<TKey>(TKey keyValue, CancellationToken cancellationToken = default);
    }
}
