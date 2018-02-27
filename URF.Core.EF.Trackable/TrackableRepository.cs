using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrackableEntities.Common.Core;
using TrackableEntities.EF.Core;
using URF.Core.Abstractions.Trackable;

namespace URF.Core.EF.Trackable
{
    public class TrackableRepository<TEntity> : Repository<TEntity>, ITrackableRepository<TEntity>
        where TEntity : class, ITrackable
    {
        public TrackableRepository(DbContext context) : base(context)
        {
        }

        public override void Insert(TEntity item)
            => TrackStateAndApplyChanges(x => x.TrackingState = TrackingState.Added, item);

        public override void Update(TEntity item)
            => TrackStateAndApplyChanges(x => x.TrackingState = TrackingState.Modified, item);

        public override void Delete(TEntity item)
            => TrackStateAndApplyChanges(x => x.TrackingState = TrackingState.Deleted, item);

        public override async Task<bool> DeleteAsync(object[] keyValues, CancellationToken cancellationToken = default)
        {
            var item = await FindAsync(keyValues, cancellationToken);
            if (item == null) return false;
                TrackStateAndApplyChanges(x => x.TrackingState = TrackingState.Deleted, item);
            return true;
        }

        public virtual void ApplyChanges(params TEntity[] entities)
            => Context.ApplyChanges(entities);

        public virtual void AcceptChanges(params TEntity[] entities)
            => Context.AcceptChanges(entities);

        public virtual void DetachEntities(params TEntity[] entities)
            => Context.DetachEntities(entities);

        public virtual async Task LoadRelatedEntities(params TEntity[] entities)
            => await Context.LoadRelatedEntitiesAsync(entities);

        private void TrackStateAndApplyChanges(Action<TEntity> setParameter, TEntity item)
        {
            setParameter(item);
            ApplyChanges(item);
        }
    }
}
