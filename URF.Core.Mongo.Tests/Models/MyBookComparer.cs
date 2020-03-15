using System;
using System.Collections.Generic;

namespace URF.Core.Mongo.Tests.Models
{
    internal class MyBookComparer : IEqualityComparer<MyBook>
    {
        public bool Equals(MyBook x, MyBook y)
            => x.BookId == y.BookId && string.Compare(x.Name, y.Name, StringComparison.InvariantCulture)
                == 0 && x.UnitPrice == y.UnitPrice && x.Category == y.Category && x.Author == y.Author;

        public int GetHashCode(MyBook x)
            => x.BookId.GetHashCode()
               ^ x.Name.GetHashCode()
               ^ x.UnitPrice.GetHashCode()
               ^ x.Category.GetHashCode()
               ^ x.Author.GetHashCode();
    }
}