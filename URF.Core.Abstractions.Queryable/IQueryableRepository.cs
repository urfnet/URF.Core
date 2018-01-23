using System.Linq;
using Urf.Core.Abstractions;

namespace URF.Core.Abstractions.Queryable
{
    public interface IQueryableRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Queryable();
        IQueryable<TEntity> SelectQueryable(string query, params object[] parameters);
    }
}
