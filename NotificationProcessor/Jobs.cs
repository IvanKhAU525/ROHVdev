using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using ROHV.Core.Database;
using ROHV.Core.User;

namespace NotificationProcessor
{
    internal class FutureJob : IJob
    {
        public async Task Execute(IJobExecutionContext context) {
            var actualTimeUtcFired = context.FireTimeUtc;
            var scheduledTimeUtc = context.ScheduledFireTimeUtc;
            var jobDescription = context.JobDetail.Description;
            var triggerDescription = context.Trigger.Description;
            var triggerkey = context.Trigger.Key;

            Console.WriteLine("actualTimeUtcFired - " + actualTimeUtcFired);
            Console.WriteLine("scheduledTimeUtc - " + scheduledTimeUtc);
            Console.WriteLine("jobDescription - " + jobDescription);
            Console.WriteLine("triggerDescription - " + triggerDescription);
            Console.WriteLine("triggerkey - " + triggerkey);
            Console.ReadLine();
            //var _context = new RayimContext();
            //var userManagment = new UserManagment(_context);
            //var scheduledTime = context.ScheduledFireTimeUtc.;
            //var notifications = await userManagment.GetScheduledNotificaitonsAsync(scheduledTime);

        }
    }

    internal class StoredAutoNotificationJob : IJob
    {
        private readonly RayimContext _context;
        private readonly UserManagment _userManagment;

        public StoredAutoNotificationJob() {
            _context = new RayimContext();
            _userManagment = new UserManagment(_context);
        }
        public async Task Execute(IJobExecutionContext context) {
            var dateNotifications = await _userManagment.GetScheduledNotificationsAsync();
            var dictionaryNotifications = dateNotifications
                .ToLookup(x => x.DateStart, y => y.RepetingTypeId)
                .ToDictionary(x => x.Key, y => y.Distinct());
            DateNotificationsObserver.Compare(dictionaryNotifications);
            
        }
    }
}