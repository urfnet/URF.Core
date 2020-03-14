using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using URF.Core.EF.Tests.Contexts;
using URF.Core.Mongo;
using URF.Core.Mongo.Tests.Models;
using Xunit;

namespace URF.Core.EF.Tests
{
    [Collection(nameof(MongoClient))]
    public class DocumentRepositoryTest
    {
        private readonly List<Book> _books;
        private readonly MongoDbContextFixture _fixture;
        private readonly IMongoCollection<Book> _collection;

        public DocumentRepositoryTest(MongoDbContextFixture fixture)
        {
            _fixture = fixture;
            _books = new List<Book>
            {
                new Book { BookName = "Design Patterns", Price = 54.93M, Category = "Computers", Author = "Ralph Johnson" },
                new Book { BookName = "Clean Code", Price = 43.15M, Category = "Computers", Author = "Robert C. Martin" },
            };
            _fixture.Initialize(() =>
            {
                _fixture.Context.GetCollection<Book>("Books").InsertMany(_books);
            });
            _collection = _fixture.Context.GetCollection<Book>("Books");
        }

        [Fact]
        public async Task FindManyAsync_Should_Return_Entities()
        {
            // Arrange
            var repository = new DocumentRepository<Book>(_collection);

            // Act
            var books = await repository.FindManyAsync();

            // Assert
            Assert.Collection(books,
                b => Assert.Equal(_books[0].BookName, b.BookName),
                b => Assert.Equal(_books[1].BookName, b.BookName));
        }

        [Fact]
        public async Task FindManyAsync_Should_Return_Filtered_Entities()
        {
            // Arrange
            var repository = new DocumentRepository<Book>(_collection);

            // Act
            var books = await repository.FindManyAsync(e => e.Category == _books[0].Category);

            // Assert
            Assert.Collection(books,
                b => Assert.Equal(_books[0].BookName, b.BookName),
                b => Assert.Equal(_books[1].BookName, b.BookName));
        }

        [Fact]
        public async Task FindOneAsync_Should_Return_Entity()
        {
            // Arrange
            var repository = new DocumentRepository<Book>(_collection);

            // Act
            var book = await repository.FindOneAsync(e => e.BookName == _books[0].BookName);

            // Assert
            Assert.Equal(_books[0].BookName, book.BookName);
        }

        [Fact]
        public async Task FindOneAndReplaceAsync_Should_Replace_Entity()
        {
            // Arrange
            var updated = new Book
            {
                Id = _books[0].Id,
                BookName = _books[0].BookName,
                Price = _books[0].Price + 10,
                Category = _books[0].Category,
                Author = _books[0].Author
            };
            var repository = new DocumentRepository<Book>(_collection);

            // Act
            var book = await repository.FindOneAndReplaceAsync(e => e.BookName == _books[0].BookName, updated);

            // Assert
            Assert.Equal(updated.Price, book.Price);
        }

        [Fact]
        public async Task InsertManyAsync_Should_Insert_Entities()
        {
            // Arrange
            var inserted = new List<Book>
            {
                new Book { BookName = "CLR via C#", Price = 34.73M, Category = ".NET", Author = "Jeffrey Richter" },
                new Book { BookName = "Essential .NET", Price = 23.25M, Category = ".NET", Author = "Don Box" },
            };
            var repository = new DocumentRepository<Book>(_collection);

            // Act
            var books = await repository.InsertManyAsync(inserted);

            // Assert
            Assert.Collection(books,
                b => Assert.Equal(inserted[0].BookName, b.BookName),
                b => Assert.Equal(inserted[1].BookName, b.BookName));
        }

        [Fact]
        public async Task InsertOneAsync_Should_Insert_Entity()
        {
            // Arrange
            var inserted = new Book
            {
                BookName = "Dependency Injection",
                Price = 34.45M,
                Category = "Patterns",
                Author = "Mark Seeman"
            };
            var repository = new DocumentRepository<Book>(_collection);

            // Act
            var book = await repository.InsertOneAsync(inserted);

            // Assert
            Assert.Equal(inserted.BookName, book.BookName);
        }

        [Fact]
        public async Task DeleteManyAsync_Should_Delete_Entities()
        {
            // Arrange
            var inserted = new List<Book>
            {
                new Book { BookName = "CLR via C#", Price = 34.73M, Category = ".NET", Author = "Jeffrey Richter" },
                new Book { BookName = "Essential .NET", Price = 23.25M, Category = ".NET", Author = "Don Box" },
            };
            var repository = new DocumentRepository<Book>(_collection);
            await repository.InsertManyAsync(inserted);

            // Act
            await repository.DeleteManyAsync(e => e.Category == ".NET");
            var books = await repository.FindManyAsync();

            // Assert
            Assert.DoesNotContain(inserted[0], books);
            Assert.DoesNotContain(inserted[1], books);
        }

        [Fact]
        public async Task DeleteOneAsync_Should_Delete_Entity()
        {
            // Arrange
            var inserted = new Book
            {
                BookName = "Dependency Injection",
                Price = 34.45M,
                Category = "Patterns",
                Author = "Mark Seeman"
            };
            var repository = new DocumentRepository<Book>(_collection);
            await repository.InsertOneAsync(inserted);

            // Act
            await repository.DeleteManyAsync(e => e.Category == "Patterns");
            var books = await repository.FindManyAsync();

            // Assert
            Assert.DoesNotContain(inserted, books);
        }
    }
}