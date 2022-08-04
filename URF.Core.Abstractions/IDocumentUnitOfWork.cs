using System.Threading.Tasks;

namespace URF.Core.Abstractions;

public interface IDocumentUnitOfWork
{
    Task StartTransactionAsync();
    Task CommitAsync();
    Task AbortAsync();
}