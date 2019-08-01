using System;
using System.Collections.Generic;

namespace NotificationProcessor.Quartz.Trigger.TriggerModels
{
    public class SimpleTriggerModel : ITriggerModel
    {
        public DateTime DateStart { get; set; }
        public Repeat RepeatType { get; set; }
        public IList<int> ConsumerNotificationSettingIds { get; set; }
    }
}