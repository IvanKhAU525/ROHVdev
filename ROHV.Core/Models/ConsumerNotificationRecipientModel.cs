using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Models
{
    public class ConsumerNotificationRecipientModel
    {
        public int Id { get; set; }
        public int NotificationId { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string Name { get; set; }
        public int AddedById { get; set; }
        public int? UpdatedById { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime? DateUpdated { get; set; }

        public UserModel SystemUser { get; set; }
        public ConsumerNotificationSettingModel ConsumerNotificationSetting { get; set; }
        public UserModel SystemUser1 { get; set; }
    }
}
