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
    
    public partial class ConsumerEmployee
    {
        public int ConsumerEmployeeId { get; set; }
        public int ConsumerId { get; set; }
        public int ContactId { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<int> MaxHoursPerWeek { get; set; }
        public Nullable<int> MaxHoursPerYear { get; set; }
        public string RateNote { get; set; }
    
        public virtual Consumer Consumer { get; set; }
        public virtual Contact Contact { get; set; }
        public virtual ServicesList ServicesList { get; set; }
    }
}