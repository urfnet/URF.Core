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
    }
}