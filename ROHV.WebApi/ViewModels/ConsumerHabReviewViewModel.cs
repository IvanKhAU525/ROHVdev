using ROHV.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ROHV.WebApi.ViewModels
{
    public class ConsumerHabReviewViewModel
    {
        public Int32? ConsumerHabReviewId { get; set; }
        public Int32 ServiceId { get; set; }
        public String ServiceName { get; set; }
        public EmployeeSearchViewModel CHCoordinator { get; set; }
        public EmployeeSearchViewModel DHCoordinator { get; set; }

        public EmployeeSearchViewModel MSC { get; set; }
        
        public String Parents { get; set; }
        public String Others { get; set; }
        public String Others2 { get; set; }
        public String Others3 { get; set; }
        public bool IsMSCParticipant { get; set; }
        public bool IsIncludeIndividialToParticipant { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public DateTime? DateReview { get; set; }

        public DateTime? SignatureDate { get; set; }

        public Int32 AddedById { get; set; }
        public String AddedByName { get; set; }
        public Int32? UpdatedById { get; set; }
        public String UpdatedByName { get; set; }

        public Int32 ConsumerId { get; set; }

        public String Notes { get; set; }

        public Boolean IsAutoSignature { get; set; }

        public ConsumerHabReviewIssueStatesModel ConsumerHabReviewIssueStates { get; set; }

        public ConsumerHabReviewViewModel()
        {
            ConsumerHabReviewIssueStates = new ConsumerHabReviewIssueStatesModel();
        }
        private String GetContact(Contact contact)
        {
            if (contact != null)
            {
                return contact.LastName + ", " + contact.FirstName;
            }
            return String.Empty;
        }
        public ConsumerHabReviewViewModel(ConsumerHabReview model)
        {
            this.ConsumerHabReviewId = model.Id;
            this.ServiceId = model.ServiceId;
            if (model.ServiceType != null)
            {
                this.ServiceName = model.ServiceType.Name;
            }

            if (model.ContactCHCoordinator != null)
            {
                this.CHCoordinator = new EmployeeSearchViewModel(model.ContactCHCoordinator);
            }

            if (model.ContactDHCoordinator != null)
            {
                this.DHCoordinator = new EmployeeSearchViewModel(model.ContactDHCoordinator);
            }

            if (model.ContactMSC != null)
            {
                this.MSC = new EmployeeSearchViewModel(model.ContactMSC);
            }            

            this.Parents = model.Parents;
            this.Others = model.Others;
            this.Others2 = model.Others2;
            this.Others3 = model.Others3;

            this.IsIncludeIndividialToParticipant = model.IsIncludeIndividialToParticipant;
            this.IsAutoSignature = model.IsAutoSignature;
            this.IsMSCParticipant = model.IsMSCParticipant;
            this.Notes = model.Notes;

            this.DateCreated = model.DateCreated;
            this.DateUpdated = model.DateUpdated;

            this.AddedById = model.AddedById;
            if (model.SystemUserAddedBy != null)
            {
                this.AddedByName = model.SystemUserAddedBy.LastName + ", " + model.SystemUserAddedBy.FirstName;
            }

            this.UpdatedById = model.UpdatedById;
            this.UpdatedByName = "";
            if (model.SystemUserUpdatedBy != null)
            {
                this.UpdatedByName = model.SystemUserUpdatedBy.LastName + ", " + model.SystemUserUpdatedBy.FirstName;
            }

            this.ConsumerHabReviewIssueStates = new ConsumerHabReviewIssueStatesModel(model.ConsumerHabReviewIssueStates.FirstOrDefault());
            this.DateReview = model.DateReview;
            this.SignatureDate = model.SignatureDate;
        }

        public static List<ConsumerHabReviewViewModel> GetList(List<ConsumerHabReview> models)
        {
            List<ConsumerHabReviewViewModel> result = new List<ConsumerHabReviewViewModel>();
            foreach (var item in models)
            {
                result.Add(new ConsumerHabReviewViewModel(item));
            }
            return result;

        }
        public ConsumerHabReview GetModel()
        {
            ConsumerHabReview model = new ConsumerHabReview();
            if (this.ConsumerHabReviewId.HasValue)
            {
                model.Id = this.ConsumerHabReviewId.Value;
            }
            else
            {
                model.Id = 0;
            }
            model.ConsumerId = this.ConsumerId;
            model.ServiceId = this.ServiceId;

            model.CHCoordinatorId = CHCoordinator?.ContactId;
            model.DHCoordinatorId = DHCoordinator?.ContactId;
            model.MSCId = MSC?.ContactId;

            model.Parents = this.Parents;
            model.Others = this.Others;
            model.Others2 = this.Others2;
            model.Others3 = this.Others3;

            model.IsIncludeIndividialToParticipant = this.IsIncludeIndividialToParticipant;
            model.IsAutoSignature = this.IsAutoSignature;

            model.IsMSCParticipant = this.IsMSCParticipant;

            model.Notes = this.Notes;



            model.AddedById = this.AddedById;
            model.UpdatedById = this.UpdatedById;
            model.DateCreated = this.DateCreated;
            model.DateUpdated = this.DateUpdated;
            model.DateReview = this.DateReview;
            model.SignatureDate = this.SignatureDate;

            model.ConsumerHabReviewIssueStates = new List<ConsumerHabReviewIssueState>() {
                this.ConsumerHabReviewIssueStates.GetModel()
            };
            return model;
        }
    }
}