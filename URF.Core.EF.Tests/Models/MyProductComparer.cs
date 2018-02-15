using System;
using System.Collections.Generic;

namespace URF.Core.EF.Tests.Models
{
    internal class MyProductComparer : IEqualityComparer<MyProduct>
    {
        public bool Equals(MyProduct x, MyProduct y)
            => x.Id == y.Id && (string.Compare(x.Name, y.Name, StringComparison.InvariantCulture) == 0) && x.Price == y.Price && x.Category == y.Category;

        public int GetHashCode(MyProduct x)
            => x.Id.GetHashCode()
               ^ x.Name.GetHashCode()
               ^ x.Price.GetHashCode()
               ^ x.Category.GetHashCode()
               ^ x.Category.GetHashCode();
    }
}