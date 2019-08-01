using System.Threading.Tasks;
using NotificationProcessor.Quartz.Trigger;
using NotificationProcessor.Utils;
using Quartz;

namespace NotificationProcessor.Quartz.Jobs {
    internal class UpdateNotificationJobsJob : IJob
    {
        private readonly DatabaseRequests _databaseRequests = new DatabaseRequests();
        
        public async Task Execute(IJobExecutionContext context) {
            var notifications = await _databaseRequests.GetNotificationsAsync();
            TriggerNotificationsObserver.CompareAndUpdate(notifications);
        }
    }
}