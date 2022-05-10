using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP5
{
    public static class Extensions
    {
        public static bool IsEmpty<T>(this T[] items)
        {
            return items.Length == 0;
        }

        public static bool IsEmpty(this string s)
        {
            return s.Equals(string.Empty);
        }

        public static void AddIfNotContains<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
                list.Add(item);
        }

        public static void AddMultipleIfNotContains<T>(this List<T> list, params T[] items)
        {
            foreach (var item in items)
            {
                list.AddIfNotContains(item);
            }
        }
    }
}
