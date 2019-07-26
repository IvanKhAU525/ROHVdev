using ROHV.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ROHV.WebApi.ViewModels
{
    public class ConsumerDocumentNoteViewModel
    {

        public Int32 EmployeeDocumentNoteId { get; set; }
        public Int32 AddedById { get; set; }
        public String AddedByName { get; set; }
        public String Note { get; set; }
        public DateTime DateCreated { get; set; }

        public ConsumerDocumentNoteViewModel() { }
        public ConsumerDocumentNoteViewModel(EmployeeDocumentNote model)
        {
            this.EmployeeDocumentNoteId = model.EmployeeDocumentNoteId;
            this.AddedById = model.AddedById;
            this.AddedByName = model.SystemUser.LastName + ", " + model.SystemUser.FirstName;
            this.Note = model.Note;
            this.DateCreated = model.DateCreated;

        }
        static public List<ConsumerDocumentNoteViewModel> GetList(List<EmployeeDocumentNote> models)
        {
            List<ConsumerDocumentNoteViewModel> result = new List<ConsumerDocumentNoteViewModel>();
            foreach (var item in models)
            {
                result.Add(new ConsumerDocumentNoteViewModel(item));
            }
            return result;

        }
    }
}