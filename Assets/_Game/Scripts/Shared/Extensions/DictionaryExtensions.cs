using System.Collections.Generic;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class DictionaryExtensions
    {
        public static bool TryGetNonDefaultValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, out TValue value)
        {
            if (dictionary.TryGetValue(key, out value))
            {
                if (!EqualityComparer<TValue>.Default.Equals(value, default(TValue)))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
