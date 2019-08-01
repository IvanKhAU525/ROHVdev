using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NotificationProcessor.TriggerModels;
using ROHV.Core.Consumer;
using ROHV.Core.Database;
using ROHV.Core.Models;
using ROHV.Core.User;

namespace NotificationProcessor
{
    public class DatabaseRequests
    {
        private readonly RayimContext _context;
        private readonly UserManagment _userManagement;
        private readonly ConsumerNotificationsManagement _consumerNotificationsManagement;

        public DatabaseRequests() {
            _context = new RayimContext();
            _userManagement = new UserManagment(_context);
            _consumerNotificationsManagement = new ConsumerNotificationsManagement(_context);
        }

        public async Task<IEnumerable<SimpleTriggerModel>> GetNotificationsAsync() {
            var notificationsFromDb = await _userManagement.GetScheduledNotificationsAsync();
            var notifications = notificationsFromDb.GroupBy(x => new {
                x.DateStart,
                x.RepetingTypeId
            }).Select(x => new SimpleTriggerModel() {
                DateStart = x.Key.DateStart,
                RepeatType = (Repeat) x.Key.RepetingTypeId,
                ConsumerNotificationSettingIds = x.Select(y => y.Id).ToList()
            });
            return notifications;
        }

        public async Task<IEnumerable<ConsumerNotificationRecipientModel>> GetNotificationRecipientsAsync(IEnumerable<int> ids) =>
            await _userManagement.GetNotificationRecipientsAsync(ids);

        public async Task UpdateAsync(IEnumerable<ConsumerNotificationSetting> consumerNotificationSetting) =>
            await _consumerNotificationsManagement.UpdateAsync(consumerNotificationSetting);

    }
}