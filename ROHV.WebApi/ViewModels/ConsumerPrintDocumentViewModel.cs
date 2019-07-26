using ROHV.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ROHV.WebApi.ViewModels
{
    public class ConsumerPrintDocumentViewModel
    {

        public Int32? ConsumerPrintDocumentId { get; set; }
        public Int32 ContactId { get; set; }
        public String ContactName { get; set; }
        public String ContactEmail { get; set; }
        public Int32 StatusId { get; set; }
        public String StatusName { get; set; }
        public Int32 ConsumerId { get; set; }
        public Int32 AddedById { get; set; }
        public String AddedByName { get; set; }
        public Int32? UpdatedById { get; set; }
        public String UpdatedByName { get; set; }
        public String ServiceAction1 { get; set; }
        public String ServiceAction2 { get; set; }
        public String ServiceAction3 { get; set; }
        public String ServiceAction4 { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public String Notes { get; set; }
        public DateTime? EffectiveDate { get; set; }

        public Int32? ServiceTypeId { get; set; }
        public String ServiceTypeName { get; set; }
        public String ServiceTypeTitle { get; set; }

        public List<ConsumerPrintDocumentValuedOutcomeViewModel> ValuedOutcomes { get; set; }

        public ConsumerPrintDocumentViewModel(){ }
        public ConsumerPrintDocumentViewModel(ConsumerPrintDocument model)
        {
            this.ConsumerPrintDocumentId = model.Id;
            this.ContactId = model.ContactId;
            this.ContactName = model.Contact.LastName + ", " + model.Contact.FirstName;
            this.ContactEmail = model.Contact.EmailAddress;
            this.StatusId = model.StatusId;
            this.StatusName = model.DocumentPrintStatus.Name;
            this.ConsumerId = model.ConsumerId;
            this.AddedById = model.AddedById;
            this.AddedByName = model.SystemUser.LastName + ", " + model.SystemUser.FirstName;
            this.UpdatedById = model.UpdatedById;
            if (this.UpdatedById.HasValue)
            {
                this.UpdatedByName = model.SystemUser1.LastName + ", " + model.SystemUser1.FirstName;
            }
            this.ServiceAction1 = model.ServiceAction1;
            this.ServiceAction2 = model.ServiceAction2;
            this.ServiceAction3 = model.ServiceAction3;
            this.ServiceAction4 = model.ServiceAction4;
            this.DateCreated = model.DateCreated;
            this.DateUpdated = model.DateUpdated;
            this.Notes = model.Notes;
            this.EffectiveDate = model.EffectiveDate;
            this.ServiceTypeId = model.ServiceTypeId;
            this.ServiceTypeTitle = model.ServiceTypeTitle;
            if (model.ServiceTypeId.HasValue)
            {
                if(!string.IsNullOrEmpty(this.ServiceTypeTitle))
                {
                    this.ServiceTypeName = this.ServiceTypeTitle;
                }
                else
                {
                    this.ServiceTypeName = model.ServiceType.Name;
                }                
            }
            
            

            if (model.ConsumerPrintDocumentValuedOutcomes != null)
            {
                this.ValuedOutcomes = ConsumerPrintDocumentValuedOutcomeViewModel.GetList(model.ConsumerPrintDocumentValuedOutcomes.ToList());
            }
            else
            {
                this.ValuedOutcomes = new List<ConsumerPrintDocumentValuedOutcomeViewModel>();
            }
        }
        static public List<ConsumerPrintDocumentViewModel> GetList(List<ConsumerPrintDocument> models)
        {
            List<ConsumerPrintDocumentViewModel> result = new List<ConsumerPrintDocumentViewModel>();
            foreach (var item in models)
            {
                result.Add(new ConsumerPrintDocumentViewModel(item));
            }
            return result;

        }

        public ConsumerPrintDocument GetModel()
        {
            ConsumerPrintDocument model = new ConsumerPrintDocument();
            if (this.ConsumerPrintDocumentId.HasValue)
            {
                model.Id = this.ConsumerPrintDocumentId.Value;
            }
            else
            {
                model.Id = 0;
            }
            model.ContactId = this.ContactId;
            model.StatusId = this.StatusId;
            model.ConsumerId = this.ConsumerId;
            model.AddedById = this.AddedById;
            model.UpdatedById = this.UpdatedById;
            model.ServiceAction1 = this.ServiceAction1;
            model.ServiceAction2 = this.ServiceAction2;
            model.ServiceAction3 = this.ServiceAction3;
            model.ServiceAction4 = this.ServiceAction4;
            model.DateCreated = this.DateCreated;
            model.DateUpdated = this.DateUpdated;
            model.Notes = this.Notes;
            model.EffectiveDate = this.EffectiveDate;
            model.ServiceTypeId = this.ServiceTypeId;
            model.ServiceTypeTitle = this.ServiceTypeTitle;
            return model;
        }

        public List<ConsumerPrintDocumentValuedOutcome> GetOutcomesModel()
        {
            List<ConsumerPrintDocumentValuedOutcome> result = new List<ConsumerPrintDocumentValuedOutcome>();
            if (ValuedOutcomes == null) return result;
            var fakeId = -1;
            foreach (var item in ValuedOutcomes)
            {
                item.Id = fakeId;
                foreach (var action in item.ServeActions)
                {
                    action.Id = 0;
                    action.ValuedOutcomeId = fakeId;
                }
                fakeId--;
                ConsumerPrintDocumentValuedOutcome model = new ConsumerPrintDocumentValuedOutcome()
                {
                    Id = item.Id,
                    ValuedOutcome = item.ValuedOutcome
                };
                result.Add(model);
            }
            return result;

        }
        public List<ConsumerPrintDocumentVOServeAction> GetActionsModel()
        {
            List<ConsumerPrintDocumentVOServeAction> result = new List<ConsumerPrintDocumentVOServeAction>();
            if (ValuedOutcomes == null) return result;
            foreach (var item in ValuedOutcomes)
            {
                foreach (var action in item.ServeActions)
                {
                    ConsumerPrintDocumentVOServeAction model = new ConsumerPrintDocumentVOServeAction()
                    {
                        ServeAndAction = action.ServeAndAction,
                        Id = action.Id,
                        PrintDocumentValuedOutcomeId = action.ValuedOutcomeId                        
                    };
                    result.Add(model);
                }
            }
            return result;
        }
    }
}