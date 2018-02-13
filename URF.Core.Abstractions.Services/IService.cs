using Urf.Core.Abstractions;

namespace URF.Core.Abstractions.Services
{
    public interface IService<TEntity>: IRepository<TEntity> where TEntity : class
    {
    }
}
