//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ROHV.Core.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class ConsumerNote
    {
        public int ConsumerNoteId { get; set; }
        public int ConsumerId { get; set; }
        public Nullable<int> ContactId { get; set; }
        public Nullable<int> TypeId { get; set; }
        public Nullable<int> TypeFromId { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Notes { get; set; }
        public int AddedById { get; set; }
        public Nullable<int> UpdatedById { get; set; }
        public System.DateTime DateCreated { get; set; }
        public Nullable<System.DateTime> DateUpdated { get; set; }
        public string AditionalInformation { get; set; }
    
        public virtual ConsumerNoteFromType ConsumerNoteFromType { get; set; }
        public virtual ConsumerNoteType ConsumerNoteType { get; set; }
        public virtual Consumer Consumer { get; set; }
        public virtual Contact Contact { get; set; }
        public virtual SystemUser SystemUser { get; set; }
        public virtual SystemUser SystemUser1 { get; set; }
    }
}
