using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using URF.Core.Mongo.Tests.Contexts;
using URF.Core.Mongo.Tests.Models;
using Xunit;

namespace URF.Core.Mongo.Tests
{
    [Collection(nameof(MongoClient))]
    public class DocumentRepositoryTest
    {
        private readonly List<Book> _books;
        private readonly IMongoCollection<Book> _collection;

        public DocumentRepositoryTest(MongoDbContextFixture fixture)
        {
            var fixture1 = fixture;
            var books = new List<Book>
            {
                new Book { BookName = "Design Patterns", Price = 54.93M, Category = "Computers", Author = "Ralph Johnson" },
                new Book { BookName = "Clean Code", Price = 43.15M, Category = "Computers", Author = "Robert C. Martin" },
            };
            fixture1.Initialize(() =>
            {
                fixture1.Context.GetCollection<Book>("Books").InsertMany(books);
            });
            _collection = fixture1.Context.GetCollection<Book>("Books");
            _books = _collection.Find(e => true).ToList();
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
                b => Assert.Equal(_books[0].Id, b.Id),
                b => Assert.Equal(_books[1].Id, b.Id));
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
                b => Assert.Equal(_books[0].Id, b.Id),
                b => Assert.Equal(_books[1].Id, b.Id));
        }

        [Fact]
        public async Task FindOneAsync_Should_Return_Entity()
        {
            // Arrange
            var repository = new DocumentRepository<Book>(_collection);

            // Act
            var book = await repository.FindOneAsync(e => e.BookName == _books[0].BookName);

            // Assert
            Assert.Equal(_books[0].Id, book.Id);
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
                b => Assert.Equal(inserted[0].Id, b.Id),
                b => Assert.Equal(inserted[1].Id, b.Id));
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
            Assert.Equal(inserted.Id, book.Id);
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

        [Fact]
        public async Task Queryable_AnyAsync_Should_Return_True()
        {
            // Arrange
            var repository = new DocumentRepository<Book>(_collection);

            // Act
            var result = await repository
                .Queryable()
                .Where(b => b.BookName == _books[0].BookName)
                .AnyAsync();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Queryable_CountAsync_Should_Return_Count()
        {
            // Arrange
            var repository = new DocumentRepository<Book>(_collection);

            // Act
            var result = await repository
                .Queryable()
                .Where(b => b.Category == _books[0].Category)
                .CountAsync();

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task Queryable_LongCountAsync_Should_Return_Count()
        {
            // Arrange
            var repository = new DocumentRepository<Book>(_collection);

            // Act
            var result = await repository
                .Queryable()
                .Where(b => b.Category == _books[0].Category)
                .LongCountAsync();

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task Queryable_FirstAsync_Should_Return_Entity()
        {
            // Arrange
            var repository = new DocumentRepository<Book>(_collection);

            // Act
            var result = await repository
                .Queryable()
                .Where(b => b.Category == _books[0].Category)
                .FirstAsync();

            // Assert
            Assert.Equal(_books[0].Id, result.Id);
        }

        [Fact]
        public async Task Queryable_FirstOrDefaultAsync_Should_Return_Entity()
        {
            // Arrange
            var repository = new DocumentRepository<Book>(_collection);

            // Act
            var result = await repository
                .Queryable()
                .Where(b => b.Category == _books[0].Category)
                .FirstOrDefaultAsync();

            // Assert
            Assert.Equal(_books[0].Id, result.Id);
        }

        [Fact]
        public async Task Queryable_FirstOrDefaultAsync_Should_Return_Null()
        {
            // Arrange
            var repository = new DocumentRepository<Book>(_collection);

            // Act
            var result = await repository
                .Queryable()
                .Where(b => b.Category == "Foo")
                .FirstOrDefaultAsync();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Queryable_MaxAsync_With_Select_Should_Return_Max()
        {
            // Arrange
            var repository = new DocumentRepository<Book>(_collection);

            // Act
            var result = await repository
                .Queryable()
                .Where(b => b.Category == _books[0].Category)
                .Select(b => b.Price)
                .MaxAsync();

            // Assert
            Assert.Equal(_books[0].Price, result);
        }

        [Fact]
        public async Task Queryable_MaxAsync_With_Expression_Should_Return_Max()
        {
            // Arrange
            var repository = new DocumentRepository<Book>(_collection);

            // Act
            var result = await repository
                .Queryable()
                .Where(b => b.Category == _books[0].Category)
                .MaxAsync(b => b.Price);

            // Assert
            Assert.Equal(_books[0].Price, result);
        }

        [Fact]
        public async Task Queryable_MinAsync_With_Select_Should_Return_Min()
        {
            // Arrange
            var repository = new DocumentRepository<Book>(_collection);

            // Act
            var result = await repository
                .Queryable()
                .Where(b => b.Category == _books[0].Category)
                .Select(b => b.Price)
                .MinAsync();

            // Assert
            Assert.Equal(_books[1].Price, result);
        }

        [Fact]
        public async Task Queryable_MinAsync_With_Expression_Should_Return_Min()
        {
            // Arrange
            var repository = new DocumentRepository<Book>(_collection);

            // Act
            var result = await repository
                .Queryable()
                .Where(b => b.Category == _books[0].Category)
                .MinAsync(b => b.Price);

            // Assert
            Assert.Equal(_books[1].Price, result);
        }

        [Fact]
        public async Task Queryable_SingleAsync_Should_Return_Entity()
        {
            // Arrange
            var repository = new DocumentRepository<Book>(_collection);

            // Act
            var result = await repository
                .Queryable()
                .Where(b => b.Id == _books[0].Id)
                .FirstAsync();

            // Assert
            Assert.Equal(_books[0].Id, result.Id);
        }

        [Fact]
        public async Task Queryable_SingleOrDefaultAsync_Should_Return_Entity()
        {
            // Arrange
            var repository = new DocumentRepository<Book>(_collection);

            // Act
            var result = await repository
                .Queryable()
                .Where(b => b.Id == _books[0].Id)
                .SingleOrDefaultAsync();

            // Assert
            Assert.Equal(_books[0].Id, result.Id);
        }

        [Fact]
        public async Task Queryable_SingleOrDefaultAsync_Should_Return_Null()
        {
            // Arrange
            var repository = new DocumentRepository<Book>(_collection);

            // Act
            var result = await repository
                .Queryable()
                .Where(b => b.BookName == "None")
                .SingleOrDefaultAsync();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Queryable_Should_Allow_Composition()
        {
            // Arrange
            var comparer = new MyBookComparer();
            var expected1 = new MyBook
            {
                BookId = _books[0].Id,
                Name = _books[0].BookName,
                UnitPrice = _books[0].Price,
                Category = _books[0].Category,
                Author = _books[0].Author
            };
            var expected2 = new MyBook
            {
                BookId = _books[1].Id,
                Name = _books[1].BookName,
                UnitPrice = _books[1].Price,
                Category = _books[1].Category,
                Author = _books[1].Author
            };
            var repository = new DocumentRepository<Book>(_collection);

            // Act
            var products = await repository
                .Queryable()
                .Take(2)
                .Where(b => b.Price.CompareTo(15.00m) > 0)
                .Select(b => new MyBook
                {
                    BookId = b.Id,
                    Name = b.BookName,
                    UnitPrice = b.Price,
                    Category = b.Category,
                    Author = b.Author
                })
                .ToListAsync();

            // Assert
            Assert.Collection(products,
                p => Assert.Equal(expected1, p, comparer),
                p => Assert.Equal(expected2, p, comparer));
        }

        [Fact]
        public async Task Queryable_Should_Support_Paging()
        {
            // Arrange
            const int page = 2;
            const int pageSize = 2;
            var inserted = new List<Book>
            {
                new Book { BookName = "CLR via C#", Price = 34.73M, Category = ".NET", Author = "Jeffrey Richter" },
                new Book { BookName = "Essential .NET", Price = 23.25M, Category = ".NET", Author = "Don Box" },
                new Book { BookName = "Dependency Injection", Price = 34.4M, Category = "Patterns", Author = "Mark Seeman" },
            };
            var repository = new DocumentRepository<Book>(_collection);
            await repository.InsertManyAsync(inserted);
            var expected = await repository
                .Queryable()
                .Where(b => b.BookName == "Clean Code" || b.BookName == "Dependency Injection")
                .OrderByDescending(b => b.BookName)
                .ToListAsync();

            // Act
            var books = await repository
                .Queryable()
                .OrderByDescending(b => b.BookName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Assert
            Assert.Collection(books,
                b => Assert.Equal(expected[0].Id, b.Id),
                b => Assert.Equal(expected[1].Id, b.Id));
        }
    }
}