using System;
using System.Linq;
using System.Threading.Tasks;
using NotificationProcessor.Quartz.Trigger;
using NotificationProcessor.Utils;
using Quartz;
using ROHV.Core.Database;
using ROHV.Core.Models;
using ROHV.EmailServiceCore;
using ROHV.EmailServiceCore.boundModels;

namespace NotificationProcessor.Quartz.Jobs {
    internal class OnceFiredEmailNotificationsJob : IJob
    {
        private readonly DatabaseRequests _databaseRequests = new DatabaseRequests();

        public async Task Execute(IJobExecutionContext context) {
            var success = long.TryParse(context.Trigger.Description, out var triggerId);
            if (!success) return;
            var consumerNotificationSettingsIds = TriggerNotificationsObserver.GetIds(triggerId);
            if (consumerNotificationSettingsIds is null) 
                throw new Exception($"The trigger with id: {triggerId} doesn't exist in the {nameof(TriggerNotificationsObserver)}");
            var emailsData = await _databaseRequests.GetNotificationRecipientsAsync(consumerNotificationSettingsIds);
            var consumerNotificationSettingsFromEmails = emailsData.Select(x => x.ConsumerNotificationSetting).ToList();
            var preparedEmails = emailsData.Select(x => new BoundEmailModel(x)).ToList();
            await EmailService.SendBoundEmails(preparedEmails);
            var markedAsCompletedConsumerNotificationSettings = ITCraftFrame.CustomMapper.MapList<ConsumerNotificationSetting, ConsumerNotificationSettingModel>(consumerNotificationSettingsFromEmails);
            markedAsCompletedConsumerNotificationSettings.ForEach(x => x.StatusId = 3);
            await _databaseRequests.UpdateAsync(markedAsCompletedConsumerNotificationSettings);
        }
    }
}