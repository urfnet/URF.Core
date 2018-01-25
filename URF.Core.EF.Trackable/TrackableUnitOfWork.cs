using System;
using Microsoft.EntityFrameworkCore;
using TrackableEntities.Common.Core;
using URF.Core.Abstractions.Trackable;

namespace URF.Core.EF.Trackable
{
    public class TrackableUnitOfWork : UnitOfWork
    {
        public TrackableUnitOfWork(DbContext context) : base(context)
        {
        }

        public virtual ITrackableRepository<TEntity> TrackableRepository<TEntity>() where TEntity : class, ITrackable
        {
            if (Repositories.TryGetValue(typeof(ITrackableRepository<TEntity>), out var repository) && repository is ITrackableRepository<TEntity>)
                return (ITrackableRepository<TEntity>)repository;
            var repository1 = (ITrackableRepository<TEntity>)Activator.CreateInstance(typeof(TrackableRepository<TEntity>), Context);
            Repositories.TryAdd(typeof(ITrackableRepository<TEntity>), repository1);
            return repository1;
        }
    }
}