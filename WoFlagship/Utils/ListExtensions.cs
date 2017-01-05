using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.Utils
{
    public static class ListExtensions
    {
        public static void AddIfNotNull<T>(this List<T> list, T item) where T : class
        {
            if (item != null)
                list.Add(item);
        }
    }
}
