using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Multimap
{
    public class Tuple<TKey, TValue>
    {
        public Tuple(TKey key, TValue value)
        {
            Key = key; Value = value;
        }

        public TKey Key { get; set; }
        public TValue Value { get; set; }
    }
}
