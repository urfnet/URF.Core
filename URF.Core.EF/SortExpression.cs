using System;
using System.ComponentModel;
using System.Linq.Expressions;
using URF.Core.Abstractions;

namespace URF.Core.EF
{
    public class SortExpression<TEntity> : ISortExpression<TEntity>
    {
        public SortExpression(Expression<Func<TEntity, object>> sortBy, ListSortDirection sortDirection)
        {
            SortBy = sortBy;
            SortDirection = sortDirection;
        }

        public Expression<Func<TEntity, object>> SortBy { get; set; }
        public ListSortDirection SortDirection { get; set; }
    }
}