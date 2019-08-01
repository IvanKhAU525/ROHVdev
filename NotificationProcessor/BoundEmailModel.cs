using System.Collections.Generic;
using ROHV.Core.Database;
using ROHV.Core.Models;

namespace NotificationProcessor
{
    public class BoundEmailModel
    {
        private const string emailTemplate = "auto-notification-email";
        public BoundEmailModel(ConsumerNotificationRecipientModel recipient) {
            this.ToEmail = recipient.Email;
            this.ReceiverName = recipient.Name;
            this.Subject = "AutoNotification";
            this.EmailTemplateName = emailTemplate;
            this.FromEmail = recipient.SystemUser.Email;
            this.EmailInputData = new List<object>() {
                new AutoNotificationServiceBoundModel() {
                    InnerEmailBody = recipient.ConsumerNotificationSetting.Note
                }
            };
        }
        public string ToEmail { get; set; }
        public string ReceiverName { get; set; }
        public string Subject { get; set; }
        public string EmailTemplateName { get; set; }
        public string FromEmail { get; set; }
        public List<object> EmailInputData { get; set; }
    }
}