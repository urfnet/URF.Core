using System.Linq;
using Microsoft.EntityFrameworkCore;
using TrackableEntities.Common.Core;
using URF.Core.Abstractions.Trackable;
using URF.Core.EF.Queryable;

namespace URF.Core.EF.Trackable
{
    public class TrackableRepository<TEntity> : QueryableRepository<TEntity>, ITrackableRepository<TEntity>
        where TEntity : class, ITrackable
    {
        public TrackableRepository(DbContext context) : base(context)
        {
        }

        public virtual void ApplyChanges(params TEntity[] entity)
        {
            throw new System.NotImplementedException();
        }

        public virtual void AcceptChanges(params TEntity[] entity)
        {
            throw new System.NotImplementedException();
        }

        public virtual void DetachEntities(params TEntity[] entity)
        {
            throw new System.NotImplementedException();
        }

        public virtual void LoadRelatedEntities(params TEntity[] entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
