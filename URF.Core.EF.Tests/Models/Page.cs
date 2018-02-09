using System.Collections.Generic;

namespace URF.Core.EF.Tests.Models
{
    class Page<TEntity>
    {
        public Page(IEnumerable<TEntity> value, int count)
        {
            Value = value;
            Count = count;
        }
        public IEnumerable<TEntity> Value { get; set; }
        public int Count { get; set; }
    }
}
