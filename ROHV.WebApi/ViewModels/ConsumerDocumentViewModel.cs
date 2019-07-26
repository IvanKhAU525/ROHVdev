using ROHV.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ROHV.WebApi.ViewModels
{
    public class ConsumerDocumentViewModel
    {
        public Int32? EmployeeDocumentId { get; set; }
        public Int32 DocumentTypeId { get; set; }
        public String DocumentTypeName { get; set; }
        public String DocumentTypeColor { get; set; }
        public Int32 DocumentStatusId { get; set; }
        public String DocumentStatusName { get; set; }
        public Int32 EmployeeId { get; set; }
        public String EmployeeName { get; set; }
        public String EmployeeCompanyName { get; set; }
        public Int32 AddedById { get; set; }
        public String AddedByName { get; set; }
        public Int32? UpdatedById { get; set; }
        public String UpdatedByName { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public DateTime? DateDocument { get; set; }
        public String DocumentPath { get; set; }
        public Int32 NumberNotes { get; set; }
        public Int32 ConsumerId { get; set; }

        public String FileData { get; set; }
        public List<ConsumerDocumentNoteViewModel> Notes { get; set; }

        public ConsumerDocumentViewModel()
        {
            Notes = new List<ConsumerDocumentNoteViewModel>();
        }
        public ConsumerDocumentViewModel(EmployeeDocument model)
        {
            this.EmployeeDocumentId = model.EmployeeDocumentId;
            this.DocumentTypeId = model.DocumentTypeId;
            this.DocumentTypeName = model.EmployeeDocumentType.Name;
            this.DocumentTypeColor = model.EmployeeDocumentType.Color;
            this.DocumentStatusId = model.DocumentStatusId;
            this.DocumentStatusName = model.EmployeeDocumentStatus.Name;
            this.EmployeeId = model.EmployeeId;
            this.EmployeeName = model.Contact.LastName + ", " + model.Contact.FirstName;
            if (model.Contact.AgencyNameId.HasValue)
            {
                this.EmployeeCompanyName = model.Contact.Agency.NameCompany;
            }
            this.AddedById = model.AddedById;
            this.AddedByName = model.SystemUser.LastName + ", " + model.SystemUser.FirstName;
            this.UpdatedById = model.UpdatedById;
            if (model.UpdatedById.HasValue)
            {
                this.UpdatedByName = model.SystemUser.LastName + ", " + model.SystemUser.FirstName;
            }
            this.DateCreated = model.DateCreated;
            this.DateUpdated = model.DateUpdated;
            this.DocumentPath = model.DocumentPath;
            this.ConsumerId = model.ConsumerId;
            if (model.EmployeeDocumentNotes != null)
            {
                this.Notes = ConsumerDocumentNoteViewModel.GetList(model.EmployeeDocumentNotes.ToList());
                this.NumberNotes = this.Notes.Count;
            }
            this.DateDocument = model.DateDocument;

        }
        static public List<ConsumerDocumentViewModel> GetList(List<EmployeeDocument> models)
        {
            List<ConsumerDocumentViewModel> result = new List<ConsumerDocumentViewModel>();
            foreach (var item in models)
            {
                result.Add(new ConsumerDocumentViewModel(item));
            }
            return result;

        }
        public List<EmployeeDocumentNote> GetNotesModel()
        {
            List<EmployeeDocumentNote> result = new List<EmployeeDocumentNote>();
            foreach (var note in this.Notes)
            {
                EmployeeDocumentNote model = new EmployeeDocumentNote()
                {
                    EmployeeDocumentNoteId = 0,
                    Note = note.Note,
                    AddedById = note.AddedById,
                    DateCreated = DateTime.Now
                };
                result.Add(model);
            }
            return result;
        }
        public EmployeeDocument GetModel()
        {
            EmployeeDocument model = new EmployeeDocument();
            if (this.EmployeeDocumentId.HasValue)
            {
                model.EmployeeDocumentId = this.EmployeeDocumentId.Value;
            }
            else
            {
                model.EmployeeDocumentId = 0;
            }
            model.ConsumerId = this.ConsumerId;
            model.DocumentTypeId = this.DocumentTypeId;
            model.DocumentStatusId = this.DocumentStatusId;
            model.EmployeeId = this.EmployeeId;
            model.DocumentPath = this.DocumentPath;

            model.AddedById = this.AddedById;
            model.UpdatedById = this.UpdatedById;
            model.DateCreated = this.DateCreated;
            model.DateUpdated = this.DateUpdated;
            model.DateDocument = this.DateDocument;

            return model;
        }
    }
}