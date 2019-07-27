using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Quartz.Util;

namespace NotificationProcessor
{
    public static class DateFormatNotificationComparerExtension
    {
        public static IDictionary<T1, IEnumerable<T2>> CompareComplexDictionaries<T1, T2>(this IDictionary<T1, IEnumerable<T2>> x, IDictionary<T1, IEnumerable<T2>> y) {
                var result = new Dictionary<T1, IEnumerable<T2>>();
                if (x is null || y is null)
                    throw new ArgumentNullException("One of arguments are null");
                try {
                    foreach (var kvRight in y) {
                        var kvLeftValue = x.TryGetAndReturn(kvRight.Key);
                        if (kvLeftValue is null) {
                            result.Add(kvRight.Key, kvRight.Value.ToList());
                        }
                        else {
                            var diff = kvRight.Value.Except(kvLeftValue).ToList();
                            if (diff.Any()) {
                                result.Add(kvRight.Key, diff);
                            }
                        }
                    }
                    return result;
                }
                catch (Exception e) {
                    throw new Exception(e.Message);
                }
        }
    }

    public static class ObservableCollectionExtension
    {
        public static void AddOrUpdate<T1, T2>(this ObservableCollection<KeyValuePairMutable<T1, IList<T2>>> collection, IDictionary<T1, IEnumerable<T2>> dictionary) {
            if (collection is null || dictionary is null)
                throw new ArgumentNullException("One of arguments are null");
            try {
                foreach (var dicKV in dictionary) {
                    var colKValueIndex = collection.IndexOf(dicKV.Key);
                    if (colKValueIndex is null) {
                        collection.Add(new KeyValuePairMutable<T1, IList<T2>>(dicKV.Key, dicKV.Value.ToList()));
                    }
                    else {
                        var diff = dicKV.Value.Except(collection[colKValueIndex.Value].Value).ToList();
                        if (diff.Any()) {
                            collection[colKValueIndex.Value].Value.AddRange(diff);
                        }
                    }
                }
            }
            catch (Exception e) {
                throw new Exception(e.Message);
            }
        }

        public static void DeleteOrUpdate<T1, T2>(this ObservableCollection<KeyValuePairMutable<T1, IList<T2>>> collection, IDictionary<T1, IEnumerable<T2>> dictionary) {
            if (collection is null || dictionary is null)
                throw new ArgumentNullException("One of arguments are null");
            try {
                foreach (var dicKV in dictionary) {
                    var colKValueIndex = collection.IndexOf(dicKV.Key);
                    if (colKValueIndex is null)
                        continue;
                    var common = dicKV.Value.Intersect(collection[colKValueIndex.Value].Value).ToList();
                    if (common.Any()) {
                        collection[colKValueIndex.Value].Value.RemoveRange(common);
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        public static int? IndexOf<TKey, TValue>(this ObservableCollection<KeyValuePairMutable<TKey, TValue>> collection, TKey key) {
            var size = collection.Count;
            for (int i = 0; i < size; i++) {
                if (collection[i].Key.Equals(key))
                    return i;
            }
            return null;
        }
    }

    public static class ListExtension
    {
        public static void RemoveRange<T>(this IList<T> collection, IEnumerable<T> itemsToDelete) {
            foreach (var item in itemsToDelete) {
                collection.Remove(item);
            }
        }
    }

    public class KeyValuePairMutable<TKey, TValue>
    {
        public KeyValuePairMutable() {
            Key = default(TKey);
            Value = default(TValue);
        }

        public KeyValuePairMutable(TKey key, TValue value) {
            Key = key;
            Value = value;
        }

        public TKey Key { get; set; }
        public TValue Value { get; set; }
    }
}