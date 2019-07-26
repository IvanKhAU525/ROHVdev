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
    
    public partial class ConsumerService
    {
        public int ConsumerServiceId { get; set; }
        public Nullable<int> ConsumerId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public Nullable<System.DateTime> AgencyDate { get; set; }
        public Nullable<int> Provider1 { get; set; }
        public Nullable<int> Provider2 { get; set; }
        public Nullable<int> AnnualUnits { get; set; }
        public Nullable<int> UnitQuantities { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string PriceId { get; set; }
        public string ProviderId { get; set; }
        public string LocaterCode { get; set; }
        public Nullable<bool> Billable { get; set; }
        public Nullable<bool> Inactive { get; set; }
        public string Notes { get; set; }
        public Nullable<System.DateTime> DateInactive { get; set; }
        public Nullable<int> CreatedByUserId { get; set; }
        public Nullable<decimal> TotalHours { get; set; }
        public Nullable<int> EditedByUserId { get; set; }
        public Nullable<System.DateTime> UsedHoursStartDate { get; set; }
        public Nullable<System.DateTime> UsedHoursEndDate { get; set; }
        public Nullable<decimal> UsedHours { get; set; }
    
        public virtual ServicesList ServicesList { get; set; }
        public virtual List List { get; set; }
        public virtual SystemUser SystemUser { get; set; }
        public virtual SystemUser SystemUser1 { get; set; }
    }
}