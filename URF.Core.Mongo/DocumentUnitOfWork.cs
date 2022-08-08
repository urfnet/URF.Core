using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using URF.Core.Abstractions;

namespace URF.Core.Mongo;

public class DocumentUnitOfWork : IDocumentUnitOfWork, IDisposable
{
    private bool _disposed;
    
    protected IClientSessionHandle Session;
    protected readonly IMongoDatabase Database;

    public DocumentUnitOfWork(IMongoDatabase database)
    {
        Database = database;
    }

    public virtual async Task StartTransactionAsync()
    {
        Session = await Database.Client.StartSessionAsync();
        Session.StartTransaction();
    }

    public virtual async Task CommitAsync()
    {
        if (Session is not null && Session.IsInTransaction)
            await Session.CommitTransactionAsync();
    }

    public virtual async Task AbortAsync()
    {
        if (Session is not null && Session.IsInTransaction)
            await Session.AbortTransactionAsync();
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        Session?.Dispose();
        _disposed = true;
    }
    
    ~DocumentUnitOfWork() => Dispose(disposing: false);
}