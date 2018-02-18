using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Urf.Core.Abstractions;
using URF.Core.Abstractions;

namespace URF.Core.EF
{
    public class Query<TEntity> : IQuery<TEntity> where TEntity : class
    {
        private int? _skip;
        private int? _take;
        private IQueryable<TEntity> _query;
        private IOrderedQueryable<TEntity> _orderedQuery;

        public Query(IRepository<TEntity> repository) =>_query = repository.Queryable();

        public virtual IQuery<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
            => Set(q => q._query = q._query.Where(predicate));

        public virtual IQuery<TEntity> Include(Expression<Func<TEntity, object>> navigationProperty) 
            => Set(q => q._query = q._query.Include(navigationProperty));

        public virtual IQuery<TEntity> OrderBy(Expression<Func<TEntity, object>> keySelector)
        {
            if (_orderedQuery == null) _orderedQuery = _query.OrderBy(keySelector);
            else _orderedQuery.OrderBy(keySelector);
            return this;
        }

        public virtual IQuery<TEntity> ThenBy(Expression<Func<TEntity, object>> thenBy)
            => Set(q => q._orderedQuery.ThenBy(thenBy));

        public virtual IQuery<TEntity> OrderByDescending(Expression<Func<TEntity, object>> keySelector)
        {
            if (_orderedQuery == null) _orderedQuery = _query.OrderByDescending(keySelector);
            else _orderedQuery.OrderByDescending(keySelector);
            return this;
        }

        public virtual IQuery<TEntity> ThenByDescending(Expression<Func<TEntity, object>> thenByDescending)
            =>Set(q => q._orderedQuery.ThenByDescending(thenByDescending));

        public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default )
            => await _query.CountAsync(cancellationToken);

        public virtual IQuery<TEntity> Skip(int skip)  
            => Set(q => q._skip = skip);

        public virtual IQuery<TEntity> Take(int take) 
            => Set(q => q._take = take);

        public virtual async Task<IEnumerable<TEntity>> SelectAsync(CancellationToken cancellationToken = default )
        {
            _query = _orderedQuery ?? _query;

            if(_skip.HasValue) _query = _query.Skip(_skip.Value);
            if (_take.HasValue) _query = _query.Take(_take.Value);

            return await _query.ToListAsync(cancellationToken);
        }

        public virtual async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
            => await _query.FirstOrDefaultAsync(predicate, cancellationToken);

        public virtual async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
            => await _query.SingleOrDefaultAsync(predicate, cancellationToken);

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
            => await _query.AnyAsync(predicate, cancellationToken);

        public virtual async Task<bool> AnyAsync(CancellationToken cancellationToken)
            => await _query.AnyAsync(cancellationToken);

        public virtual async Task<bool> AllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
            => await _query.AllAsync(predicate, cancellationToken);

        public virtual async Task<IEnumerable<TEntity>> SelectSqlAsync(string sql, object[] parameters, CancellationToken cancellationToken = default)
            => await _query.FromSql(sql, parameters).ToListAsync(cancellationToken);

        private IQuery<TEntity> Set(Action<Query<TEntity>> setParameter)
        {
            setParameter(this);
            return this;
        }
    }
}
