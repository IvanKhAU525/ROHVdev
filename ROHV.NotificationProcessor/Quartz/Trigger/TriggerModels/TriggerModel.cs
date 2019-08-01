using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NotificationProcessor.Quartz.Trigger.TriggerModels
{
    public class TriggerModel : ITriggerModel, IEquatable<TriggerModel>
    {
        private static long _nextId;
        public long TriggerId { get; }
        public DateTime DateStart { get; set; }
        public Repeat RepeatType { get; set; }
        public IList<int> ConsumerNotificationSettingIds { get; set; }

        public TriggerModel() {
            try {
                this.TriggerId = Interlocked.Increment(ref _nextId);
            }
            catch (OverflowException exception) {
                this.TriggerId = Interlocked.Exchange(ref _nextId, 0);
            }
        }

        public TriggerModel(SimpleTriggerModel simpleTriggerModel) {
            this.DateStart = simpleTriggerModel.DateStart;
            this.RepeatType = simpleTriggerModel.RepeatType;
            this.ConsumerNotificationSettingIds = simpleTriggerModel.ConsumerNotificationSettingIds;
        }
        
        public bool Equals(TriggerModel other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (TriggerId != other.TriggerId) return false;
            if (DateStart != other.DateStart) return false;
            if (RepeatType != other.RepeatType) return false;
            if (ConsumerNotificationSettingIds.Except(other.ConsumerNotificationSettingIds).Any()) return false;
            return true;
        }

        public override bool Equals(object obj) {
            return Equals(obj as TriggerModel);
        }

        public override int GetHashCode() {
            {
                var hashCode = DateStart.GetHashCode();
                hashCode = (hashCode * 397) ^ (TriggerId.GetHashCode());
                hashCode = (hashCode * 397) ^ ((int)RepeatType);
                hashCode = (hashCode * 397) ^ (ConsumerNotificationSettingIds != null ? ConsumerNotificationSettingIds.Sum() + ConsumerNotificationSettingIds.Count() : 0);
                return hashCode;
            }
        }
    }
}