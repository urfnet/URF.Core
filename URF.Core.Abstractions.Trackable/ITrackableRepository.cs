using TrackableEntities.Common.Core;
using Urf.Core.Abstractions;

namespace URF.Core.Abstractions.Trackable
{
    public interface ITrackableRepository<TEntity> : IRepository<TEntity> where TEntity : class, ITrackable
    {
        void ApplyChanges(params TEntity[] entity);
        void AcceptChanges(params TEntity[] entity);
        void DetachEntities(params TEntity[] entity);
        void LoadRelatedEntities(params TEntity[] entity);
    }
}