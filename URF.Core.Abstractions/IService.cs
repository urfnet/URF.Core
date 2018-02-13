using Urf.Core.Abstractions;

namespace URF.Core.Abstractions
{
    public interface IService<TEntity>: IRepository<TEntity> where TEntity : class
    {
    }
}
