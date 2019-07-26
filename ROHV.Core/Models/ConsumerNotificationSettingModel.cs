using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Models
{
    public class ConsumerNotificationSettingModel
    {
        public int Id { get; set; }
        public int ConsumerId { get; set; }
        public int RepetingTypeId { get; set; }
        public int StatusId { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public System.DateTime DateStart { get; set; }
        public int AddedById { get; set; }
        public int? UpdatedById { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime? DateUpdated { get; set; }
        public IEnumerable<ConsumerNotificationRecipientModel> ConsumerNotificationRecipients { get; set; }
        public ConsumerModel Consumer { get; set; }
        public NotificationStatusModel NotificationStatus { get; set; }
        public NotificationTypeModel NotificationType { get; set; }
        public UserModel SystemUser { get; set; }
        public UserModel SystemUser1 { get; set; }
    }
}