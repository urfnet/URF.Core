using System;
using System.Linq;
using System.Linq.Expressions;

namespace URF.Core.Abstractions.Queryable
{
    public interface IQueryFluent<TEntity> where TEntity : class
    {
        IQueryFluent<TEntity> OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy);
        IQueryFluent<TEntity> Include(Expression<Func<TEntity, object>> expression);
        IQueryFluent<TEntity> SelectPage(int page, int pageSize, out int totalCount);
        IQueryFluent<TResult> Select<TResult>(Expression<Func<TEntity, TResult>> selector = null) where TResult : class;
        IQueryable<TEntity> QuerySql(string query, params object[] parameters);
    }
}