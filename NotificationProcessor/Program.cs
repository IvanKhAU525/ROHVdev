﻿using Quartz;
using Quartz.Impl;
using ROHV.Core.Database;
using ROHV.Core.Models;
using ROHV.Core.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
 using NotificationProcessor.TriggerModels;

 namespace NotificationProcessor
{
    partial class Program
    {
        public class Service : ServiceBase
        {
            public const string ServiceName = "NotificationProcessor";
            public Service() {
            }

            protected override void OnStart(string[] args) {
                Program.Start(args);
            }

            protected override void OnStop() {
                Program.Stop();
            }
        }
        static void Main(string[] args) {
            if (!Environment.UserInteractive) {
                using (var service = new Service()) {
                    ServiceBase.Run(service);
                }
            } else {
                //Stop();
                 Start(args);
                 Console.ReadLine();
            }
        }
        private static void Stop() {
            Test1();
        }

        public static void Test1() {

            void DisplayTriggers() {
                foreach (var trigger in TriggerNotificationsObserver._triggersInfo) {
                    Console.WriteLine($"{trigger.TriggerId} -> {trigger.DateStart} -> {trigger.RepeatType} -> {string.Join(",", trigger.ConsumerNotificationSettingIds)}");
                }
            }
            
            var date1 = DateTime.Now;
            var date2 = date1.AddDays(1);
            var date3 = date2.AddDays(1);
            var date4 = date3.AddDays(1);

            var setupTriggers = new List<SimpleTriggerModel>() {
                new SimpleTriggerModel() {
                    DateStart = date1,
                    RepeatType = Repeat.Once,
                    ConsumerNotificationSettingIds = new List<int>() {1, 2, 3}
                },
                new SimpleTriggerModel() {
                    DateStart = date1,
                    RepeatType = Repeat.Every6Month,
                    ConsumerNotificationSettingIds = new List<int>() {10, 11, 12}
                },
                new SimpleTriggerModel() {
                    DateStart = date2,
                    RepeatType = Repeat.EveryWeek,
                    ConsumerNotificationSettingIds = new List<int>() {100, 101, 102}
                }
            };
            var updateTriggers = new List<SimpleTriggerModel>() {
                new SimpleTriggerModel() {
                    DateStart = date1,
                    RepeatType = Repeat.Once,
                    ConsumerNotificationSettingIds = new List<int>() {1, 2, 3, 4, 5, 6}
                },
                new SimpleTriggerModel() {
                    DateStart = date1,
                    RepeatType = Repeat.Every6Month,
                    ConsumerNotificationSettingIds = new List<int>() {10}
                },
//                new SimpleTriggerModel() {
//                    DateStart = date2,
//                    RepeatType = Repeat.EveryWeek,
//                    ConsumerNotificationSettingIds = new List<int>() {100, 101, 102}
//                }
                new SimpleTriggerModel() {
                    DateStart = date3,
                    RepeatType = Repeat.EveryYear,
                    ConsumerNotificationSettingIds = new List<int>() {1000, 10001}
                },
            };
            
            // no new triggers
//            var updateTriggers2 = new List<SimpleTriggerModel>() {
//                new SimpleTriggerModel() {
//                    DateStart = date1,
//                    RepeatType = Repeat.Once,
//                    ConsumerNotificationSettingIds = new List<int>() {1, 2, 3}
//                },
//                new SimpleTriggerModel() {
//                    DateStart = date1,
//                    RepeatType = Repeat.Every6Month,
//                    ConsumerNotificationSettingIds = new List<int>() {10, 11, 12}
//                },
//                new SimpleTriggerModel() {
//                    DateStart = date2,
//                    RepeatType = Repeat.EveryWeek,
//                    ConsumerNotificationSettingIds = new List<int>() {100, 101, 102}
//                }
//            };
            TriggerNotificationsObserver.SetUpTriggerNotificationObserver(setupTriggers);
            Console.WriteLine("Settuped triggers...");
            DisplayTriggers();
            TriggerNotificationsObserver.AddEvent();
            TriggerNotificationsObserver.CompareAndUpdate(updateTriggers);
            Console.WriteLine("Updated triggers...");
            DisplayTriggers();
            
        }
        
        private static async void Start(string[] args) {
            await QuartzScheduler.StartScheduler();
            
            var _databaseRequests = new DatabaseRequests();
            var notifications = await _databaseRequests.GetNotificationsAsync();
            
            TriggerNotificationsObserver.SetUpTriggerNotificationObserver(notifications);
            
            var triggersAndJobs = GetJobs(notifications);
            var TAJForNotificationJobsUpdater = SetUpNotificationJobsUpdater();
            
            await QuartzScheduler.ScheduleJobs(triggersAndJobs);
            
            TriggerNotificationsObserver.AddEvent();

            await QuartzScheduler.ScheduleJob(TAJForNotificationJobsUpdater.Item1, TAJForNotificationJobsUpdater.Item2);
        }

        public static (IJobDetail, ITrigger)  SetUpNotificationJobsUpdater() {
            var jobDetail = QuartzJob.CreateJob(Repeat.EveryMinute);
            var trigger = QuartzTrigger.CreateTrigger(Repeat.EveryMinute);
            return (jobDetail, trigger);
        }
        
        private static IReadOnlyDictionary<IJobDetail, IReadOnlyCollection<ITrigger>> GetJobs(IEnumerable<SimpleTriggerModel> notifications) {
            var triggers = new List<ITrigger>();
            var triggersAndJobs = new Dictionary<IJobDetail, IReadOnlyCollection<ITrigger>>();
            var repeatTypes = notifications.Select(x => x.RepeatType).Distinct();
            foreach (Repeat repeatType in repeatTypes) {
                var job = QuartzJob.CreateJob(repeatType);
                var notificationsOfType = notifications.Where(x => x.RepeatType == repeatType);
                foreach (var notification in notificationsOfType) {
                    var triggerId = TriggerNotificationsObserver.GetTriggerId(notification);
                    var trigger = QuartzTrigger.CreateTrigger(notification.DateStart, notification.RepeatType, triggerId.ToString());//notification.TriggerId.ToString());
                    triggers.Add(trigger);
                }
                triggersAndJobs.Add(job, triggers.ToList());
                triggers.Clear();
            }

            return triggersAndJobs;
        }
    }
}
