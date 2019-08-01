using ROHV.Core;
using ROHV.Core.Models;
using ROHV.EmailServiceCore.Attributes;

namespace ROHV.EmailServiceCore.boundModels
{ 
    public class CustomerNotesBoundModel : ConsumerNotesModel
    {
        [EmailBound(Name = "[From]")]
        public string ViewContactName { get => Contact?.LastName + ", " + Contact?.FirstName; }        
      
        [EmailBound(Name = "[From Dept.]")]
        public  string ViewTypeFromName { get => base.ConsumerNoteFromType?.Name; }

        [EmailBound(Name = "[Date]")]
        public string ViewDate { get => base.Date.ToDateString(); }       

        [EmailBound(Name = "[InnerEmailBody]")]
        public string InnerEmailBody { set; get; }
        [EmailBound(Name = "[Notes]")]
        public override string Notes { get => base.Notes; set => base.Notes = value; }

    }
}
