using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> ExceptBy<T, K>(this IEnumerable<T> source, Func<T, K> selector)
        {
            foreach (var item in source)
            {
                bool isDuplicate = false;
                foreach (var seenItem in source)
                {
                    if (EqualityComparer<K>.Default.Equals(selector(item), selector(seenItem)))
                    {
                        isDuplicate = true;
                        break;
                    }
                }
                if (!isDuplicate)
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<T> IntersectByCount<T, K>(this IEnumerable<T> first, IEnumerable<K> second, Func<T, K> selector)
        {
            var seenKeys = new Dictionary<K, int>();
            foreach (var key in second)
            {
                if (!seenKeys.ContainsKey(key))
                {
                    seenKeys[key] = 0;
                }
                seenKeys[key]++;
            }
            foreach (var item in first)
            {
                var key = selector(item);
                if (seenKeys.TryGetValue(key, out int count) && count > 0)
                {
                    seenKeys[key] = count - 1;
                    yield return item;
                }
            }
        }

        public static IEnumerable<T> IntersectByCount<T, K>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, K> selector)
        {
            return first.IntersectByCount(second.Select(selector), selector);
        }
    }
}
