using System.Collections.Generic;

public static class ExtendDictionaries
{
    public static void Set<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
    {
        if(dict.ContainsKey(key))
        {
            dict[key] = value;
        }
        else
        {
            dict.Add(key, value);
        }
    }
}
