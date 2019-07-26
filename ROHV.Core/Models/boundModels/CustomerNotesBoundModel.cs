using ROHV.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Models
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
