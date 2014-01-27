using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool SequenceEqualTo<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            if (second == null)
            {
                return false;
            }
            return first.SequenceEqual(second);
        }
    }
}
