using System;
using System.Collections.Generic;
using System.Text;
using URF.Core.Abstractions;

namespace URF.Core.EF
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
