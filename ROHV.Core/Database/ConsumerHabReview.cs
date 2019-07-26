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
    
    public partial class ConsumerHabReview
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ConsumerHabReview()
        {
            this.ConsumerHabReviewIssueStates = new HashSet<ConsumerHabReviewIssueState>();
        }
    
        public int Id { get; set; }
        public int ConsumerId { get; set; }
        public int ServiceId { get; set; }
        public Nullable<int> AdvocateId { get; set; }
        public Nullable<int> MSCId { get; set; }
        public Nullable<int> CHCoordinatorId { get; set; }
        public Nullable<int> DHCoordinatorId { get; set; }
        public string Parents { get; set; }
        public string Others { get; set; }
        public int AddedById { get; set; }
        public Nullable<int> UpdatedById { get; set; }
        public System.DateTime DateCreated { get; set; }
        public Nullable<System.DateTime> DateUpdated { get; set; }
        public string Notes { get; set; }
        public Nullable<System.DateTime> DateReview { get; set; }
        public bool IsMSCParticipant { get; set; }
        public string Others2 { get; set; }
        public bool IsIncludeIndividialToParticipant { get; set; }
        public string Others3 { get; set; }
        public Nullable<System.DateTime> SignatureDate { get; set; }
        public bool IsAutoSignature { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConsumerHabReviewIssueState> ConsumerHabReviewIssueStates { get; set; }
        public virtual SystemUser SystemUserAddedBy { get; set; }
        public virtual Contact ContactAdvocate { get; set; }
        public virtual Contact ContactCHCoordinator { get; set; }
        public virtual Consumer Consumer { get; set; }
        public virtual Contact ContactDHCoordinator { get; set; }
        public virtual Contact ContactMSC { get; set; }
        public virtual ServiceType ServiceType { get; set; }
        public virtual SystemUser SystemUserUpdatedBy { get; set; }
    }
}