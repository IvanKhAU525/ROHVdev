using ROHV.Core.Database;
using System.Data.Entity;
using System.Threading.Tasks;
using System;
using ROHV.Core.Services;
using System.Collections.Generic;
using System.Linq;

namespace ROHV.Core.Consumer
{
    public class ConsumerNotificationsManagement : BaseModel
    {
        public ConsumerNotificationsManagement(RayimContext context) : base(context) { }

        public async Task<List<ConsumerNotificationSetting>> GetNotifications(Int32 consumerId)
        {
            var result = await _context.ConsumerNotificationSettings.Where(x => x.ConsumerId == consumerId).OrderByDescending(x => x.DateStart).ToListAsync();
            return result;
        }
        public async Task<Int32> Save(ConsumerNotificationSetting dbModel, List<ConsumerNotificationRecipient> recipients)
        {
            if (dbModel.Id == 0)
            {
                _context.ConsumerNotificationSettings.Add(dbModel);
            }
            else
            {
                var model = await _context.ConsumerNotificationSettings.SingleOrDefaultAsync(x => x.Id == dbModel.Id);
                if (model != null)
                {
                    model.RepetingTypeId = dbModel.RepetingTypeId;
                    model.StatusId = dbModel.StatusId;
                    model.Name = dbModel.Name;
                    model.Note = dbModel.Note;
                    model.DateStart = dbModel.DateStart;
                    model.AddedById = dbModel.AddedById;
                    model.UpdatedById = dbModel.UpdatedById;
                    model.DateCreated = dbModel.DateCreated;
                    model.DateUpdated = dbModel.DateUpdated;
                    var old = _context.ConsumerNotificationRecipients.Where(x => x.NotificationId == dbModel.Id);
                    _context.ConsumerNotificationRecipients.RemoveRange(old);
                }
            }
            await _context.SaveChangesAsync();
            if (recipients.Count > 0)
            {
                foreach (var recipient in recipients)
                {
                    recipient.NotificationId = dbModel.Id;
                    _context.ConsumerNotificationRecipients.Add(recipient);
                }
                await _context.SaveChangesAsync();
            }
            return dbModel.Id;
        }

        public async Task<Boolean> Delete(Int32 id)
        {
            var model = await _context.ConsumerNotificationSettings.SingleOrDefaultAsync(x => x.Id == id);
            if (model != null)
            {
                _context.ConsumerNotificationSettings.Remove(model);
                var old = _context.ConsumerNotificationRecipients.Where(x => x.NotificationId == id);
                if (old != null)
                {
                    _context.ConsumerNotificationRecipients.RemoveRange(old);
                }
                await _context.SaveChangesAsync();
                return true;
            }
            return false;

        }

        public async Task DeleteAll(Int32 consumerId)
        {
            var models = _context.ConsumerNotificationSettings.Where(x => x.ConsumerId == consumerId);
            if (models.Count() > 0)
            {
                var recipients = _context.ConsumerNotificationRecipients.Where(x => (models.Select(z => z.Id).Contains(x.NotificationId)));
                if (recipients.Count() > 0)
                {
                    _context.ConsumerNotificationRecipients.RemoveRange(recipients);
                }

                _context.ConsumerNotificationSettings.RemoveRange(models);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<NotificationType>> GetTypes()
        {
            var result = await _context.NotificationTypes.ToListAsync();
            return result;
        }

        public async Task<List<NotificationStatus>> GetStatuses()
        {
            var result = await _context.NotificationStatuses.ToListAsync();
            return result;
        }

        public async Task UpdateAsync(IEnumerable<ConsumerNotificationSetting> consumerNotificationSettings) {
            var dBConsumerNotificationSettings = new List<ConsumerNotificationSetting>();
            foreach (var cns in consumerNotificationSettings) {
                 var consumerNotificationSetting = await _context.ConsumerNotificationSettings.FirstOrDefaultAsync(x => x.Id == cns.Id);
                 dBConsumerNotificationSettings.Add(consumerNotificationSetting);
            }
            dBConsumerNotificationSettings.ForEach(x => x.StatusId = 3);
            await _context.SaveChangesAsync();
        }
    }
}
