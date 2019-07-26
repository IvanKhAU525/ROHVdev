using ROHV.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ROHV.WebApi.ViewModels
{
    public class ConsumerCallLogsViewModel
    { 
        public Int32? ConsumerContactCallId { get; set; }
        public String ContactName { get; set; }
        public Int32? ContactId { get; set; }
        public DateTime? CalledOn { get; set; }
        public String Notes { get; set; }
        public String AddedByName { get; set; }
        public String UpdatedByName { get; set; }
        public Int32? AddedById { get; set; }
        public Int32? UpdatedById { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public Int32? ConsumerId { get; set; }

        public ConsumerCallLogsViewModel() { }
        public ConsumerCallLogsViewModel(ConsumerContactCall model)
        {
            ConsumerContactCallId = model.ConsumerContactCallId;
            ContactId = model.ContactId;
            if (this.ContactId.HasValue)
            {
                this.ContactName = model.Contact.LastName + ", " + model.Contact.FirstName;
            }
            CalledOn = model.CalledOn;
            Notes = model.Notes;
            UpdatedById = model.UpdatedById;
            if (this.UpdatedById.HasValue)
            {
                this.UpdatedByName = model.SystemUser.LastName + ", " + model.SystemUser.FirstName;
            }else
            {
                if(!String.IsNullOrEmpty(model.UpdatedBy))
                {
                    this.UpdatedByName = model.UpdatedBy;
                }
            }

            AddedById = model.AddedById;
            if (this.AddedById.HasValue)
            {
                this.AddedByName = model.SystemUser1.LastName + ", " + model.SystemUser1.FirstName;
            }
            else
            {
                if (!String.IsNullOrEmpty(model.AddedBy))
                {
                    this.AddedByName = model.AddedBy;
                }
            }          
         
            DateCreated = model.DateCreated;
            DateUpdated = model.DateUpdated;
            ConsumerId = model.ConsumerId;
        }
        static public List<ConsumerCallLogsViewModel> GetList(List<ConsumerContactCall> models)
        {
            List<ConsumerCallLogsViewModel> result = new List<ConsumerCallLogsViewModel>();
            foreach (var item in models)
            {
                result.Add(new ConsumerCallLogsViewModel(item));
            }
            return result;

        }
        public ConsumerContactCall GetModel()
        {
            ConsumerContactCall model = new ConsumerContactCall();
            if (this.ConsumerContactCallId.HasValue)
            {
                model.ConsumerContactCallId = this.ConsumerContactCallId.Value;
            }
            else
            {
                model.ConsumerContactCallId = 0;
            }
            model.ConsumerId = this.ConsumerId;
            model.ContactId = this.ContactId;
            model.CalledOn = this.CalledOn;
            model.Notes = this.Notes;

            model.AddedById = this.AddedById;
            model.UpdatedById = this.UpdatedById;
            model.DateCreated = this.DateCreated;
            model.DateUpdated = this.DateUpdated;


            return model;
        }
    }
}