using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Models
{
    public class NotificationTypeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ActionNo { get; set; }

        public IEnumerable<ConsumerNotificationSettingModel> ConsumerNotificationSettings { get; set; }
    }
}
