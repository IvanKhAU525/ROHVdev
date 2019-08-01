using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NotificationProcessor.TriggerModels;
using Quartz;
using ROHV.Core.Consumer;
using ROHV.Core.Database;
using ROHV.Core.Models;
using ROHV.Core.User;

namespace NotificationProcessor
{
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

    internal class UpdateNotificationJobsJob : IJob
    {
        private readonly DatabaseRequests _databaseRequests = new DatabaseRequests();
        
        public async Task Execute(IJobExecutionContext context) {
            var notifications = await _databaseRequests.GetNotificationsAsync();
            TriggerNotificationsObserver.CompareAndUpdate(notifications);
        }
    }
}