using System.Collections.Generic;
using UnityEngine;

namespace Game.Extensions
{
    public static class RandomExtensions
    {
        public static T GetRandom<T>(this ICollection<T> items)
        {
            int count = items.Count;
            var randomElement = Random.Range(0, count);
            int i = 0;

            foreach (T item in items)
            {
                if (i++ == randomElement)
                {
                    return item;
                }
            }

            return default(T);
        }

        public static T GetRandom<T>(this IList<T> items)
        {
            int count = items.Count;
            var randomElement = Random.Range(0, count);

            return items[randomElement];
        }
    }
}
