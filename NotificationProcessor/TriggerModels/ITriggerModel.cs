using System;
using System.Collections.Generic;
using Quartz;

namespace NotificationProcessor.TriggerModels
{
    public interface ITriggerModel 
    {
        DateTime DateStart { get; set; }
        Repeat RepeatType { get; set; }
        IList<int> ConsumerNotificationSettingIds { get; set; }
    }


}