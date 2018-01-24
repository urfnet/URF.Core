using System.Linq;
using Microsoft.EntityFrameworkCore;
using URF.Core.Abstractions.Queryable;

namespace URF.Core.EF.Queryable
{
    public class QueryableRepository<TEntity> : Repository<TEntity>, IQueryableRepository<TEntity>
        where TEntity : class
    {
        public QueryableRepository(DbContext context) : base(context)
        {
        }

        public virtual IQueryable<TEntity> Queryable() => Set;

        public virtual IQueryable<TEntity> QueryableSql(string sql, params object[] parameters)
            => Set.FromSql(sql, parameters);
    }
}
