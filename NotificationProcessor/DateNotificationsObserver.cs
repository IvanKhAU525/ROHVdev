using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Quartz.Util;
using ROHV.Core;

namespace NotificationProcessor
{
    public static class DateNotificationsObserver
    {
        private static ObservableCollection<KeyValuePairMutable<DateTime, IList<int>>> dateTriggers;

        static DateNotificationsObserver() {
            dateTriggers = new ObservableCollection<KeyValuePairMutable<DateTime, IList<int>>>();
        }
        
        public static void AddDate(IEnumerable<int> typeIds, DateTime startDate) {
            try {
                dateTriggers.Add(new KeyValuePairMutable<DateTime, IList<int>>(startDate, typeIds.ToList()));      
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        public static void AddDate(int typeId, DateTime startDate) {
            try {
                dateTriggers.Add(new KeyValuePairMutable<DateTime, IList<int>>(startDate, new List<int>() { typeId }));
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        public static void AddDates(IDictionary<DateTime, IEnumerable<int>> dates) {
            try {
                dateTriggers.AddOrUpdate(dates);
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        public static void DeleteDates(IDictionary<DateTime, IEnumerable<int>> dates) {
            try {
                dateTriggers.DeleteOrUpdate(dates);
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }
        
        public static void AddEvent() => dateTriggers.CollectionChanged += DateTriggersOnCollectionChanged;

        private static void DateTriggersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add: AddNewTriggersToScheduler(e.NewItems);
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
            
            var newNotificationsInDb = dicDateTriggers.CompareComplexDictionaries(dicLSNInDb);
            var deletedNotificationsInDb = dicLSNInDb.CompareComplexDictionaries(dicDateTriggers);
            
            AddDates(newNotificationsInDb);
            DeleteDates(deletedNotificationsInDb);
        }
    }
}