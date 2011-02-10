using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extensions
{
    public static class DictionaryExtensions
    {
        //toString not ToString...
        //ya sorry, can't override using method extensions >=(
        public static string toString<TKey, TValue>(this Dictionary<TKey, TValue> set)
        {
            string retString = "Dictionary<" + typeof(TKey).Name + ", "+typeof(TValue).Name+">\n{\n";
            foreach (KeyValuePair<TKey, TValue> pair in set)
                retString += " '" + pair.Key.ToString() + "'  \t=> '"+pair.Value.ToString() +"'\n";
            return retString + "}\n";
        }
    }
}
