using System.Linq;
using System.Threading.Tasks;
using NotificationProcessor.Quartz.Trigger;
using NotificationProcessor.Utils;
using Quartz;
using ROHV.EmailServiceCore;
using ROHV.EmailServiceCore.boundModels;

namespace NotificationProcessor.Quartz.Jobs {
    internal class RepeatedEmailNotificationsJob : IJob
    {
        private readonly DatabaseRequests _databaseRequests = new DatabaseRequests();

        public async Task Execute(IJobExecutionContext context) {
            var success = long.TryParse(context.Trigger.Description, out var triggerId);
            if (!success) return;
            var consumerNotificationSettingsIds = TriggerNotificationsObserver.GetIds(triggerId);
            var emailsData = await _databaseRequests.GetNotificationRecipientsAsync(consumerNotificationSettingsIds);
            var preparedEmails = emailsData.Select(x => new BoundEmailModel(x)).ToList(); 
            await EmailService.SendBoundEmails(preparedEmails);
        }
    }
}