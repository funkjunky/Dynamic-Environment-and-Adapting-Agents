using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Multimap
{
    public class TwoUniqueKeyMap<TKey, TKey2, TValue> : Dictionary<TKey, Dictionary<TKey2, TValue>>
    {
        public TwoUniqueKeyMap() { }

        public void Add(TKey key1, TKey2 key2, TValue value) 
        {
            //if ((this.ContainsKey(key1) && this[key1].ContainsKey(key2)) ||
            //    (this.ContainsKey(key2) && this[key2].ContainsKey(key1)))
            //if this is true we already have this element, so error? Ya may as well for now.
            if ((this.ContainsKey(key1) && this[key1].ContainsKey(key2)))
                throw new Exception("Can't add an element with the same 2 keys: '" + key1 + "', '" + key2 + "'");

            //grab the container for the keys if it exists. Not sure if this is the most effecient method.
            Dictionary<TKey2, TValue> container = new Dictionary<TKey2,TValue>();
            if(this.ContainsKey(key1))
                container = this[key1];

            //add the container to the... larger container, this.
            container.Add(key2, value);
            this[key1] = container;
        }

        public TValue get(TKey key1, TKey2 key2)
        {
            if (this.ContainsKey(key1) && this[key1].ContainsKey(key2))
                return this[key1][key2];
            else
                throw new Exception("The element you are trying to get doesn't exist. Keys: '" + key1.ToString() + "', '" + key2.ToString() + "'");
        }
    }
}