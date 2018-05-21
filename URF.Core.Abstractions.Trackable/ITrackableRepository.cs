using System.Threading.Tasks;
using TrackableEntities.Common.Core;

namespace URF.Core.Abstractions.Trackable
{
    public interface ITrackableRepository<TEntity> : IRepository<TEntity> where TEntity : class, ITrackable
    {
        void ApplyChanges(params TEntity[] entities);
        void AcceptChanges(params TEntity[] entities);
        void DetachEntities(params TEntity[] entities);
        Task LoadRelatedEntities(params TEntity[] entities);
    }
}