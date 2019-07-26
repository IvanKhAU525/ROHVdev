using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using Quartz.Util;

namespace NotificationProcessor
{
    public static class DateNotificationsObserver
    {
        private static ObservableCollection<KeyValuePairMutable<DateTime, IEnumerable<int>>> dateTriggers;

        static DateNotificationsObserver() {
            dateTriggers = new ObservableCollection<KeyValuePairMutable<DateTime, IEnumerable<int>>>();
        }
        
        public static void AddDate(IEnumerable<int> typeIds, DateTime startDate) {
            try {
                dateTriggers.Add(new KeyValuePairMutable<DateTime, IEnumerable<int>>(startDate, typeIds));      
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        public static void AddDate(int typeId, DateTime startDate) {
            try {
                dateTriggers.Add(new KeyValuePairMutable<DateTime, IEnumerable<int>>(startDate, new List<int>() { typeId }));
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        public static void AddDates(IDictionary<DateTime, IEnumerable<int>> dates) {
            try {
                //TODO: dateTriggers.AddRange(dates);
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }
        
        public static void AddDates(IDictionary<DateTime, int> dates) {
            try {
                var newDateFormat = dates.Select(x => new KeyValuePairMutable<DateTime, IEnumerable<int>>(x.Key, new List<int>() { x.Value }));
                dateTriggers.AddRange(newDateFormat);
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }
        
        public static void TryAddOrUpdateDates(IDictionary<DateTime, IEnumerable<int>> addedDates) {
            try {
                foreach (var addedDate in addedDates) {
                    
                }
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        public static void TryDeleteDates(IDictionary<DateTime, IEnumerable<int>> dates) {
            try {
                //date
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        public static void AddEvent() {
            dateTriggers.CollectionChanged += DateTriggersOnCollectionChanged;
        }

        private static void DateTriggersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    AddNewTriggersToScheduler(e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove : RemoveOldTriggersFromScheduler(e.OldItems);
                    break;
            }
        }

        private static void RemoveOldTriggersFromScheduler(IList oldItems) {
        }

        private static void AddNewTriggersToScheduler(IList newItems) {
            foreach (KeyValuePairMutable<DateTime, int[]> VARIABLE in newItems) {
                Console.WriteLine("int - " + VARIABLE.Key + " date - " + VARIABLE.Value);
            }
        }

        public static void Compare(IDictionary<DateTime, IEnumerable<int>> lastStateOfNotificationsInDb) {
            var dicLSNInDb = lastStateOfNotificationsInDb.ToDictionary(x => x.Key, x => x.Value.ToList().AsEnumerable());
            var dicDateTriggers = dateTriggers.ToDictionary(x => x.Key, x => x.Value.ToList().AsEnumerable());
            
            var newNotificationsInDb = dicDateTriggers.CompareComplexDictionaries<DateTime, IEnumerable<int>, int>(dicLSNInDb);
            var deletedNotificationsInDb = dicLSNInDb.CompareComplexDictionaries<DateTime, IEnumerable<int>, int>(dicDateTriggers);
            
            TryAddOrUpdateDates(newNotificationsInDb);
            TryDeleteDates(deletedNotificationsInDb);
        }
    }

    public static class DateFormatNotificationComparerExtension 
    {
        public static  IDictionary<TN, TEnumerable> CompareComplexDictionaries<TN, TEnumerable, TZ>(this IDictionary<TN, TEnumerable> x, IDictionary<TN, TEnumerable> y) where TEnumerable : IEnumerable<TZ>
        {
            try { 
                var result = new Dictionary<TN, TEnumerable>();
                if (x is null || y is null) 
                    throw new ArgumentNullException("One of arguments are null");

                foreach (var kvRight in y) {
                    var kvLeftValue = x.TryGetAndReturn(kvRight.Key);
                    if (kvLeftValue.Equals(null)) {
                        result.Add(kvRight.Key, kvRight.Value);
                    }
                    else {
                        var diff = kvRight.Value.Except(kvLeftValue);
                        if (diff.Any()) {
                            result.Add(kvRight.Key, (TEnumerable) diff);
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
        public static void AddOrUpdate<T1, T2, T3>(this ObservableCollection<KeyValuePairMutable<T1, T2>> collection, IDictionary<T1, T2> dictionary) where T2 : IEnumerable<T3> {
            if (collection is null || dictionary is null) 
                throw new ArgumentNullException("One of arguments are null");
            
            foreach (var dicKV in dictionary) {
                var colKValue = dictionary.TryGetAndReturn(dicKV.Key);
                if (colKValue.Equals(null)){// is null) {
                    //collection.Add(dicKV);
                }
                else {
                    var diff = dicKV.Value.Except(colKValue);
                    if (diff.Any()) {
                        var index = collection.IndexOf(dicKV.Key);
                        collection[index].Value = colKValue;
                    }
                }
            }
        }

        public static int IndexOf<TKey, TValue>(this ObservableCollection<KeyValuePairMutable<TKey, TValue>> collection, TKey key) {
            var size = collection.Count;
            for (int i = 0; i < size; i++) {
                if (collection[i].Key.Equals(key))
                    return i;
            }
//TODO:
            return 0;
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