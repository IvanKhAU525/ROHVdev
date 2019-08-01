using System;
using System.Collections.Generic;

namespace NotificationProcessor.Quartz.Trigger.TriggerModels
{
    public interface ITriggerModel 
    {
        DateTime DateStart { get; set; }
        Repeat RepeatType { get; set; }
        IList<int> ConsumerNotificationSettingIds { get; set; }
    }


}