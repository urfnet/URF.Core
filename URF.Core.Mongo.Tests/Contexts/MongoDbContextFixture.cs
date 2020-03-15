using System;
using Mongo2Go;
using MongoDB.Driver;

namespace URF.Core.EF.Tests.Contexts
{
    public class MongoDbContextFixture : IDisposable
    {
        private MongoClient _client;
        private IMongoDatabase _context;
        private MongoDbRunner _mongoRunner;

        public void Initialize(Action seedData = null)
        {
            _mongoRunner = MongoDbRunner.Start();
            _client = new MongoClient(_mongoRunner.ConnectionString);
            _context = _client.GetDatabase("BookstoreDb");
            seedData?.Invoke();
        }

        public IMongoDatabase Context
        {
            get
            {
                if (_context == null)
                    throw new InvalidOperationException("You must first call Initialize before getting the context.");
                return _context;
            }
        }

        public void Dispose()
        {
            _mongoRunner.Dispose();
            _mongoRunner = null;
            _client = null;
            _context = null;
        }
    }
}
