using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationProcessor
{
    public static class CollectionMethodExtension
    {
        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source) {
            if (target is null)
                throw new ArgumentNullException(nameof(target));
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            foreach (var item in source) {
                target.Add(item);
            }
        }
    }
}
