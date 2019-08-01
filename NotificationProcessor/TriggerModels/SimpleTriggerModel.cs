using System;
using System.Collections.Generic;

namespace NotificationProcessor.TriggerModels
{
    public class SimpleTriggerModel : ITriggerModel
    {
        public DateTime DateStart { get; set; }
        public Repeat RepeatType { get; set; }
        public IList<int> ConsumerNotificationSettingIds { get; set; }
    }
}