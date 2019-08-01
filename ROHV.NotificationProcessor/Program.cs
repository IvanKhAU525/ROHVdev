using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using NotificationProcessor.Quartz;
using NotificationProcessor.Quartz.Trigger;
using NotificationProcessor.Quartz.Trigger.TriggerModels;
using NotificationProcessor.Utils;
using Quartz;

namespace NotificationProcessor
{
    partial class Program
    {
        public class Service : ServiceBase
        {
            public const string ServiceName = "ROHV.NotificationProcessor";
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
                 Start(args);
                 Console.ReadLine();
            }
        }
        private static void Stop() {
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
