using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using URF.Core.Abstractions;

namespace URF.Core.Mongo;

public class DocumentUnitOfWork : IDocumentUnitOfWork, IDisposable
{
    private IClientSessionHandle _session;
    private readonly IMongoDatabase _database;

    public DocumentUnitOfWork(IMongoDatabase database)
    {
        _database = database;
    }

    public virtual async Task StartTransactionAsync()
    {
        _session = await _database.Client.StartSessionAsync();
        _session.StartTransaction();
    }

    public virtual async Task CommitAsync()
    {
        if (_session is not null && _session.IsInTransaction)
        {
            await _session.CommitTransactionAsync();
            Dispose();
        }
    }

    public virtual async Task AbortAsync()
    {
        if (_session is not null && _session.IsInTransaction)
        {
            await _session.AbortTransactionAsync();
            Dispose();
        }
    }

    public void Dispose()
    {
        _session?.Dispose();
    }
}