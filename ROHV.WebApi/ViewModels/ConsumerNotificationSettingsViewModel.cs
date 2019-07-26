using ROHV.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ROHV.WebApi.ViewModels
{
    public class ConsumerNotificationSettingsViewModel
    {

        public Int32? Id { get; set; }
        public String RepetingTypeName { get; set; }
        public Int32 RepetingTypeId { get; set; }
        public String StatusName { get; set; }
        public Int32 StatusId { get; set; }
        public String Name { get; set; }
        public String Note { get; set; }
        public DateTime DateStart { get; set; }
        public String AddedByName { get; set; }
        public String UpdatedByName { get; set; }
        public Int32 AddedById { get; set; }
        public Int32? UpdatedById { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public Int32? ConsumerId { get; set; }

        public String RecipientsString { get; set; }
        public List<ConsumerNotificationRecipientViewModel> Recipients { get; set; }

        public ConsumerNotificationSettingsViewModel() {
            this.Recipients = new List<ConsumerNotificationRecipientViewModel>();
        }
        public ConsumerNotificationSettingsViewModel(ConsumerNotificationSetting model)
        {
            this.Id = model.Id;

            this.RepetingTypeId = model.RepetingTypeId;
            this.RepetingTypeName = model.NotificationType.Name;
            this.StatusId = model.StatusId;
            this.StatusName = model.NotificationStatus.Name;
            this.Name = model.Name;
            this.Note = model.Note;
            this.DateStart = model.DateStart;
            UpdatedById = model.UpdatedById;
            if (this.UpdatedById.HasValue)
            {
                this.UpdatedByName = model.SystemUser1.LastName + ", " + model.SystemUser1.FirstName;
            }

            AddedById = model.AddedById;           
            this.AddedByName = model.SystemUser.LastName + ", " + model.SystemUser.FirstName;                    
            DateCreated = model.DateCreated;
            DateUpdated = model.DateUpdated;
            ConsumerId = model.ConsumerId;

            if(model.ConsumerNotificationRecipients.Count!=0)
            {
                this.Recipients = ConsumerNotificationRecipientViewModel.GetList(model.ConsumerNotificationRecipients.ToList());
            }else
            {
                this.Recipients = new List<ViewModels.ConsumerNotificationRecipientViewModel>();
            }
        }
        static public List<ConsumerNotificationSettingsViewModel> GetList(List<ConsumerNotificationSetting> models)
        {
            List<ConsumerNotificationSettingsViewModel> result = new List<ConsumerNotificationSettingsViewModel>();
            foreach (var item in models)
            {
                result.Add(new ConsumerNotificationSettingsViewModel(item));
            }
            return result;

        }
        public ConsumerNotificationSetting GetModel()
        {
            ConsumerNotificationSetting model = new ConsumerNotificationSetting();
            if (this.Id.HasValue)
            {
                model.Id = this.Id.Value;
            }
            else
            {
                model.Id = 0;
            }
            model.ConsumerId = this.ConsumerId.Value;
            model.RepetingTypeId = this.RepetingTypeId;            
            model.StatusId = this.StatusId;            
            model.Name = this.Name;
            model.Note = this.Note;
            model.DateStart = this.DateStart;

            model.AddedById = this.AddedById;
            model.UpdatedById = this.UpdatedById;
            model.DateCreated = this.DateCreated;
            model.DateUpdated = this.DateUpdated;


            return model;
        }
        public List<ConsumerNotificationRecipient> GetRecipients()
        {
            List<ConsumerNotificationRecipient> result = new List<ConsumerNotificationRecipient>();
            foreach (var recipient in this.Recipients)
            {                
                result.Add(recipient.GetModel());
            }
            return result;
        }
    }
}