using System.Collections.Generic;

namespace URF.Core.EF.Tests.Models
{
    class Page<TEntity>
    {
        public Page(int count, IEnumerable<TEntity> value)
        {
            Value = value;
            Count = count;
        }
        public IEnumerable<TEntity> Value { get; set; }
        public int Count { get; set; }
    }
}
