using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using NotificationProcessor.TriggerModels;
using Quartz;
using Quartz.Util;
using ROHV.Core;


namespace NotificationProcessor
{
    public static class TriggerNotificationsObserver
    {
        public static ObservableCollection<TriggerModel> _triggersInfo;

        static TriggerNotificationsObserver() {
            _triggersInfo = new ObservableCollection<TriggerModel>();
        }

        public static long AddTriggerInfo(DateTime startDate, Repeat repeatType, IEnumerable<int> consumerNotificationSettingIds) {
            var triggerModel = new TriggerModel() {
                DateStart = startDate,
                ConsumerNotificationSettingIds = consumerNotificationSettingIds.ToList(),
                RepeatType = repeatType
            };
            _triggersInfo.Add(triggerModel);
            return triggerModel.TriggerId;
        }

        public static void ReplaceTriggerInfo(SimpleTriggerModel triggerToReplace) {
            var localTrigger = _triggersInfo.First(x => x.DateStart == triggerToReplace.DateStart && x.RepeatType == triggerToReplace.RepeatType);
            var index = _triggersInfo.IndexOf(localTrigger);
            _triggersInfo[index].ConsumerNotificationSettingIds = triggerToReplace.ConsumerNotificationSettingIds; 
        }
        
        public static void DeleteTriggerInfo(TriggerModel trigger) => _triggersInfo.Remove(trigger);
        
        public static void DeleteTriggersInfo(IEnumerable<TriggerModel> triggers) {
            foreach (var trigger in triggers) {
                DeleteTriggerInfo(trigger);
            }
        }

        public static long GetTriggerId(SimpleTriggerModel notification) => 
            _triggersInfo.First(x => x.DateStart == notification.DateStart && x.RepeatType == notification.RepeatType).TriggerId;

        public static void SetUpTriggerNotificationObserver(IEnumerable<SimpleTriggerModel> triggers) {
            foreach (var trigger in triggers) {
                AddTriggerInfo(trigger.DateStart, trigger.RepeatType, trigger.ConsumerNotificationSettingIds);
            }
        }
        public static void AddEvent() => _triggersInfo.CollectionChanged += TriggersInfoOnCollectionChanged;

        public static IEnumerable<int> GetIds(long triggerId) => _triggersInfo.FirstOrDefault(x => x.TriggerId == triggerId)?.ConsumerNotificationSettingIds;

        private static void TriggersInfoOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add: AddNewTriggersToScheduler(e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove : RemoveOldTriggersFromScheduler(e.OldItems);
                    break;
//                case NotifyCollectionChangedAction.Replace : UpdateTriggersInScheduler(e.NewItems, e.OldItems);
//                    break;
            }
        }

//        private static void UpdateTriggersInScheduler(IList newItems, IList oldItems) {
//                foreach (TriggerModel item in newItems) {
//                    var oldTriggerKey = QuartzTrigger.GetTriggerKey(item.DateStart, item.RepeatType);
//                    var newTriggerKey = QuartzTrigger.CreateTriggerForExistedJob(item.DateStart, item.RepeatType,item.TriggerId.ToString());
//                    QuartzScheduler.UpdateTrigger(oldTriggerKey, newTriggerKey);
//                }
//        }

        private static void RemoveOldTriggersFromScheduler(IList oldItems) {
            foreach (TriggerModel item in oldItems) {
                var triggerKey = QuartzTrigger.GetTriggerKey(item.DateStart, item.RepeatType);
                QuartzScheduler.DetachTrigger(triggerKey);
            }
        }

        private static void AddNewTriggersToScheduler(IList newItems) {
            foreach (TriggerModel item in newItems) {
                var trigger = QuartzTrigger.CreateTriggerForExistedJob(item.DateStart, item.RepeatType, item.TriggerId.ToString());
                QuartzScheduler.AttachTrigger(trigger);
            }
        }

        public static void CompareAndUpdate(IEnumerable<SimpleTriggerModel> lastStateOfNotificationsInDb) {
            var disposedTriggers = FindDisposedTriggers(lastStateOfNotificationsInDb);
            DeleteTriggersInfo(disposedTriggers);
            foreach (var trigger in lastStateOfNotificationsInDb) {
                // -1 - trigger not found. So need to add
                //  0 - trigger need to replace
                //  1 - triggers are the same
                
                var result = FindAndCompare(trigger);
                switch (result) {
                    case -1: AddTriggerInfo(trigger.DateStart, trigger.RepeatType, trigger.ConsumerNotificationSettingIds); break;
                    case  0: ReplaceTriggerInfo(trigger); break;
                    case  1: continue;
                }
            }
        }

        private static int FindAndCompare(SimpleTriggerModel trigger) {
            var localTrigger = _triggersInfo.FirstOrDefault(x => trigger.DateStart == x.DateStart && trigger.RepeatType == x.RepeatType);
            if (localTrigger is null) 
                return -1;
            if (trigger.ConsumerNotificationSettingIds.Count != localTrigger.ConsumerNotificationSettingIds.Count) 
                return 0;
            var diff = localTrigger.ConsumerNotificationSettingIds.Except(trigger.ConsumerNotificationSettingIds);
            return diff.Any() ? 0 : 1;
        }

        private static IEnumerable<TriggerModel> FindDisposedTriggers(IEnumerable<SimpleTriggerModel> dbTriggers) {
            var disposedTriggers = new List<TriggerModel>();
            foreach (var localTrigger in _triggersInfo) {
                var trigger = dbTriggers.FirstOrDefault(x => x.DateStart == localTrigger.DateStart && x.RepeatType == localTrigger.RepeatType);
                if (trigger is null)
                    disposedTriggers.Add(localTrigger);
            }

            return disposedTriggers;
        }

    }
}