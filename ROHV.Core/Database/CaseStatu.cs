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
    
    public partial class CaseStatu
    {
        public int CaseStatusId { get; set; }
        public Nullable<int> ConsumerId { get; set; }
        public Nullable<int> WorkflowProcessId { get; set; }
        public Nullable<System.DateTime> WorkflowDate { get; set; }
        public string Note { get; set; }
        public string AddedBy { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<bool> Selected { get; set; }
    
        public virtual Consumer Consumer { get; set; }
        public virtual List List { get; set; }
    }
}
