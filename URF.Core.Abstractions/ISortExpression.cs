using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace URF.Core.Abstractions
{
    public interface ISortExpression<TEntity>
    {
        Expression<Func<TEntity, object>> SortBy { get; set; }
        ListSortDirection SortDirection { get; set; }
    }
}