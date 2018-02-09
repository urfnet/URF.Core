using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Urf.Core.Abstractions;
using URF.Core.Abstractions;

namespace URF.Core.EF
{
    class Query<TEntity> : IQuery<TEntity> where TEntity : class
    {
        private int? _skip;
        private int? _take;
        private IQueryable<TEntity> _queryable;
        private IOrderedQueryable<TEntity> _orderedQuery;

        public Query(IRepository<TEntity> repository)
        {
            _queryable = repository.Queryable();
        }

        public IQuery<TEntity> Where(Expression<Func<TEntity, bool>> filter)
        {
            _queryable =_queryable.Where(filter);
            return this;
        }

        public IQuery<TEntity> Include(Expression<Func<TEntity, object>> include)
        {
            _queryable =_queryable.Include(include);
            return this;
        }

        public IQuery<TEntity> OrderBy(Expression<Func<TEntity, object>> sortBy)
        {
            if (_orderedQuery == null) _orderedQuery = _queryable.OrderBy(sortBy);
            else _orderedQuery.OrderBy(sortBy);
            return this;
        }

        public IQuery<TEntity> ThenBy(Expression<Func<TEntity, object>> sortBy)
        {
            _orderedQuery.ThenBy(sortBy);
            return this;
        }

        public IQuery<TEntity> OrderByDescending(Expression<Func<TEntity, object>> sortBy)
        {
            if (_orderedQuery == null) _orderedQuery = _queryable.OrderByDescending(sortBy);
            else _orderedQuery.OrderByDescending(sortBy);
            return this;
        }
        public IQuery<TEntity> ThenByDescending(Expression<Func<TEntity, object>> sortBy)
        {
            _orderedQuery.ThenByDescending(sortBy);
            return this;
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default )
        {
            return await _queryable.CountAsync(cancellationToken);
        }

        public IQuery<TEntity> Skip(int skip)
        {
            _skip = skip;
            return this;
        }

        public IQuery<TEntity> Take(int take)
        {
            _take = take;
            return this;
        }
        
        public virtual async Task<IEnumerable<TEntity>> SelectAsync(CancellationToken cancellationToken = default )
        {
            _queryable = _orderedQuery ?? _queryable;

            if(_skip.HasValue) _queryable = _queryable.Skip(_skip.Value);
            if (_take.HasValue) _queryable = _queryable.Take(_take.Value);

            return await _queryable.ToListAsync(cancellationToken);
        }
    }
}
