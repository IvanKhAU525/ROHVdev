using Quartz;
using Quartz.Impl;
using ROHV.Core.Database;
using ROHV.Core.Models;
using ROHV.Core.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
                Start(args);
            }
        }
        private static void Stop() {
        }
        private static async void Start(string[] args) {
            await QuartzScheduler.StartScheduler();
            var _context = new RayimContext();
            var userManagement = new UserManagment(_context);

            var notifications = userManagement.GetScheduledNotificationsAsync().Result;
            var triggersAndJobs = GetJobs(notifications);
            await QuartzScheduler.ScheduleJobs(triggersAndJobs);
            Console.ReadLine();
        }
        
        private static IReadOnlyDictionary<IJobDetail, IReadOnlyCollection<ITrigger>> GetJobs(IEnumerable<ConsumerNotificationSettingModel> notifications) {
            var triggersAndJobs = new Dictionary<IJobDetail, IReadOnlyCollection<ITrigger>>();
            var onceFiredNotifications = notifications.Where(x => x.RepetingTypeId == 1).Select(x => x.DateStart).Distinct();
            var onceFiredTriggersAndJobs = SetUpOnceFiredNotifications(dates: onceFiredNotifications);
            var restFiredNotifications = notifications.Where(x => x.RepetingTypeId != 1).ToList();
            var restFiredTriggersAndJobs = SetUpRestFiredNotifications(restFiredNotifications);
            triggersAndJobs.AddRange(onceFiredTriggersAndJobs);
            triggersAndJobs.AddRange(restFiredTriggersAndJobs);
            return triggersAndJobs;
        }

        private static IDictionary<IJobDetail, IReadOnlyCollection<ITrigger>> SetUpOnceFiredNotifications(IEnumerable<DateTime> dates) {
            var triggers = new List<ITrigger>();
            var groupTriggersName = "triggers_once";
            var groupJobsName = "jobs_once";
            var jobName = "job_once";
            foreach (var fireDate in dates) {
                var triggerName = "trigger_" + fireDate.ToString();
                var trigger = QuartzTrigger.CreateTrigger(triggerName, groupTriggersName, fireDate);
                triggers.Add(trigger);
            }
            var job = QuartzJob.CreateJob<FutureJob>(jobName, groupJobsName);
            return new Dictionary<IJobDetail, IReadOnlyCollection<ITrigger>>() { [job] = triggers };
        }

        private static IDictionary<IJobDetail, IReadOnlyCollection<ITrigger>> SetUpRestFiredNotifications(IEnumerable<ConsumerNotificationSettingModel> notifications) {
            //DateNotificationsObserver.AddEvent();
            //TODO:
//            DateNotificationsObserver.AddDates(notifications
//                .ToLookup(d => d.DateStart, t => t.RepetingTypeId)
//                .ToDictionary(x => x.Key, y => y.ToArray()));
            
            var jobsAndTriggers = new Dictionary<IJobDetail, IReadOnlyCollection<ITrigger>>();
            var triggers = new List<ITrigger>();
            var typeIds = notifications.Select(x => x.RepetingTypeId).Distinct();
            foreach (var typeId in typeIds) { 
                var dates = notifications.Where(x => x.RepetingTypeId == typeId).Select(x => x.DateStart).Distinct();
                var groupTriggersName = "triggers_" + (Repeat)typeId;
                var jobName = "job_" + (Repeat)typeId;
                var groupJobsName = "jobs_" + (Repeat)typeId;
                foreach (var fireDate in dates) {
                    var triggerName = "trigger_" + fireDate.ToString();
                    var cronExpression = CronJob.GetCronExpression(typeId, fireDate);
                    var trigger = QuartzTrigger.CreateTrigger(triggerName, groupTriggersName, cronExpression);
                    triggers.Add(trigger);
                }
                var job = QuartzJob.CreateJob<FutureJob>(jobName, groupJobsName);
                jobsAndTriggers.Add(job, triggers.ToList());
                triggers.Clear();
            }
            return jobsAndTriggers;
        }
    }
}
