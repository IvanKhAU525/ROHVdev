using ROHV.Core.Database;
using ROHV.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ROHV.WebApi.ViewModels
{
    public sealed class ConsumerNotesViewModel: ConsumerNotesModel
    {
       


        public ConsumerNotesViewModel() { }
        public ConsumerNotesViewModel(ConsumerNote model)
        {
            ConsumerNoteId = model.ConsumerNoteId;
            ContactId = model.ContactId;
            if (this.ContactId.HasValue)
            {
                this.ContactName = model.Contact.LastName + ", " + model.Contact.FirstName;
            }
            TypeId = model.TypeId;
            if (TypeId.HasValue)
            {
                TypeName = model.ConsumerNoteType.Name;
            }
            TypeFromId = model.TypeFromId;
            if (TypeFromId.HasValue)
            {
                TypeFromName = model.ConsumerNoteFromType.Name;
            }
            Date = model.Date;
            Notes = model.Notes;
            AditionalInformation = model.AditionalInformation;

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
        }
        static public List<ConsumerNotesViewModel> GetList(List<ConsumerNote> models)
        {
            List<ConsumerNotesViewModel> result = new List<ConsumerNotesViewModel>();
            foreach (var item in models)
            {
                result.Add(new ConsumerNotesViewModel(item));
            }
            return result;

        }
        public ConsumerNote GetModel()
        {
            ConsumerNote model = new ConsumerNote();
            if (this.ConsumerNoteId.HasValue)
            {
                model.ConsumerNoteId = this.ConsumerNoteId.Value;
            }
            else
            {
                model.ConsumerNoteId = 0;
            }
            model.ConsumerId = this.ConsumerId;
            model.ContactId = this.ContactId;
            model.Date = this.Date;
            model.Notes = this.Notes;
            model.TypeId = this.TypeId;
            model.TypeFromId = this.TypeFromId;
            model.AditionalInformation = this.AditionalInformation;

            model.AddedById = this.AddedById;
            model.UpdatedById = this.UpdatedById;
            model.DateCreated = this.DateCreated;
            model.DateUpdated = this.DateUpdated;


            return model;
        }
    }
}