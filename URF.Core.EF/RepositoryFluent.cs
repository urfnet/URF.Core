using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Urf.Core.Abstractions;
using URF.Core.Abstractions;

namespace URF.Core.EF
{
    class RepositoryFluent<TEntity> : IRepositoryFluent<TEntity> where TEntity : class
    {
        private readonly IRepository<TEntity> _repository;
        private readonly List<Expression<Func<TEntity, object>>> _includes;
        private readonly List<ISortExpression<TEntity>> _sorts;
        private Expression<Func<TEntity, bool>> _filter;
        private int? _page;
        private int? _pageSize;

        public RepositoryFluent(IRepository<TEntity> repository)
        {
            _repository = repository;
            _filter = null;
            _includes = new List<Expression<Func<TEntity, object>>>();
            _sorts = new List<ISortExpression<TEntity>>();
        }

        public IRepositoryFluent<TEntity> Filter(Expression<Func<TEntity, bool>> filter)
        {
            _filter = filter;
            return this;
        }

        public IRepositoryFluent<TEntity> Include(Expression<Func<TEntity, object>> include)
        {
            _includes.Add(include);
            return this;
        }

        public IRepositoryFluent<TEntity> OrderBy(Expression<Func<TEntity, object>> sortBy)
        {
            _sorts.Add(new SortExpression<TEntity>(sortBy, ListSortDirection.Ascending));
            return this;
        }

        public IRepositoryFluent<TEntity> OrderByDescending(Expression<Func<TEntity, object>> sortBy)
        {
            _sorts.Add(new SortExpression<TEntity>(sortBy, ListSortDirection.Descending));
            return this;
        }

        public IRepositoryFluent<TEntity> Page(int page)
        {
            _page = page;
            return this;
        }

        public IRepositoryFluent<TEntity> PageSize(int pageSize)
        {
            _pageSize = pageSize;
            return this;
        }

        public virtual async Task<IEnumerable<TEntity>> SelectAsync(CancellationToken cancellationToken = default )
        {
            return await _repository
                .SelectAsync(
                    filter: _filter,
                    includes: _includes.ToArray(),
                    sortExpressions: _sorts.ToArray(),
                    page: 2,
                    pageSize: 10,
                    cancellationToken: cancellationToken);
        }
    }
}
