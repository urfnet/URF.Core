using System.Linq;
using Microsoft.EntityFrameworkCore;
using TrackableEntities.Common.Core;
using URF.Core.Abstractions.Queryable;
using URF.Core.Abstractions.Trackable;

namespace URF.Core.EF.Trackable
{
    public class TrackableRepository<TEntity> : Repository<TEntity>, IQueryableRepository<TEntity>, ITrackableRepository<TEntity>
        where TEntity : class, ITrackable
    {
        public TrackableRepository(DbContext context) : base(context)
        {
        }

        public IQueryable<TEntity> Queryable()
        {
            throw new System.NotImplementedException();
        }

        public IQueryable<TEntity> SelectQueryable(string query, params object[] parameters)
        {
            throw new System.NotImplementedException();
        }

        public void ApplyChanges(params TEntity[] entity)
        {
            throw new System.NotImplementedException();
        }

        public void AcceptChanges(params TEntity[] entity)
        {
            throw new System.NotImplementedException();
        }

        public void DetachEntities(params TEntity[] entity)
        {
            throw new System.NotImplementedException();
        }

        public void LoadRelatedEntities(params TEntity[] entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
