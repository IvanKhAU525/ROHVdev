using ROHV.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ROHV.WebApi.ViewModels
{
    public class ConsumerNotificationRecipientViewModel
    {
        public Int32 Id { get; set; }
        public String Email { get; set; }
        public String Position { get; set; }
        public String Name { get; set; }
        public String AddedByName { get; set; }
        public String UpdatedByName { get; set; }
        public Int32 AddedById { get; set; }
        public Int32? UpdatedById { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }



        public ConsumerNotificationRecipientViewModel() { }
        public ConsumerNotificationRecipientViewModel(ConsumerNotificationRecipient model)
        {
            this.Id = model.Id;
            this.Email = model.Email;
            this.Position = model.Position;
            this.Name = model.Name;
            UpdatedById = model.UpdatedById;
            if (this.UpdatedById.HasValue)
            {
                this.UpdatedByName = model.SystemUser1.LastName + ", " + model.SystemUser1.FirstName;
            }

            AddedById = model.AddedById;
            this.AddedByName = model.SystemUser.LastName + ", " + model.SystemUser.FirstName;
            DateCreated = model.DateCreated;
            DateUpdated = model.DateUpdated;
        }
        static public List<ConsumerNotificationRecipientViewModel> GetList(List<ConsumerNotificationRecipient> models)
        {
            List<ConsumerNotificationRecipientViewModel> result = new List<ConsumerNotificationRecipientViewModel>();
            foreach (var item in models)
            {
                result.Add(new ConsumerNotificationRecipientViewModel(item));
            }
            return result;

        }
        public ConsumerNotificationRecipient GetModel()
        {
            ConsumerNotificationRecipient model = new ConsumerNotificationRecipient();
            model.Id = 0;
            model.Email = this.Email;
            model.Position = this.Position;
            model.Name = this.Name;


            model.AddedById = this.AddedById;
            model.UpdatedById = this.UpdatedById;
            model.DateCreated = this.DateCreated;
            model.DateUpdated = this.DateUpdated;

            return model;
        }       
    }
}