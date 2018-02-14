using TrackableEntities.Common.Core;
using Urf.Core.Abstractions;
using URF.Core.Abstractions.Trackable;

namespace URF.Core.Abstractions.Services
{
    public interface IService<TEntity>: ITrackableRepository<TEntity> where TEntity : class, ITrackable
    {
    }
}
