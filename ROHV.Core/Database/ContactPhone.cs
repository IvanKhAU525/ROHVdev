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
    
    public partial class ContactPhone
    {
        public int ContactPhoneId { get; set; }
        public Nullable<int> ContactId { get; set; }
        public Nullable<int> PhoneTypeId { get; set; }
        public string Phone { get; set; }
        public string Extension { get; set; }
        public string Note { get; set; }
    
        public virtual Contact Contact { get; set; }
        public virtual List List { get; set; }
    }
}
