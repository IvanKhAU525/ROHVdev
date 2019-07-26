using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Models
{
    public class ConsumerNotesModel
    {
        public virtual Int32? ConsumerNoteId { get; set; }
        public virtual Int32 ConsumerId { get; set; }
        public virtual Int32? ContactId { get; set; }

        public virtual String ContactName { get; set; }
        public virtual Int32? TypeId { get; set; }
        public virtual String TypeName { get; set; }
        public virtual Int32? TypeFromId { get; set; }
        public virtual String TypeFromName { get; set; }
        public virtual DateTime? Date { get; set; }
        public virtual String Notes { get; set; }

        public virtual String AditionalInformation { get; set; }

        public virtual String AddedByName { get; set; }
        public virtual String UpdatedByName { get; set; }

        public virtual Int32 AddedById { get; set; }
        public virtual Int32? UpdatedById { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime? DateUpdated { get; set; }

        public ContactModel Contact { set; get; }
        public ConsumerNoteTypeModel ConsumerNoteType { set; get; }
        public ConsumerNoteFromTypeModel ConsumerNoteFromType { set; get; }

        public UserModel SystemUser { set; get; }
    }
}
