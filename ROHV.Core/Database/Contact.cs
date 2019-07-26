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
    
    public partial class Contact
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Contact()
        {
            this.ConsumerContactCalls = new HashSet<ConsumerContactCall>();
            this.ConsumerHabPlans = new HashSet<ConsumerHabPlan>();
            this.ConsumerPrintDocuments = new HashSet<ConsumerPrintDocument>();
            this.Consumers = new HashSet<Consumer>();
            this.Consumers1 = new HashSet<Consumer>();
            this.Consumers2 = new HashSet<Consumer>();
            this.Consumers3 = new HashSet<Consumer>();
            this.ContactPhones = new HashSet<ContactPhone>();
            this.EmployeeDocuments = new HashSet<EmployeeDocument>();
            this.ConsumerEmployees = new HashSet<ConsumerEmployee>();
            this.Consumers4 = new HashSet<Consumer>();
            this.Consumers11 = new HashSet<Consumer>();
            this.ConsumerNotes = new HashSet<ConsumerNote>();
            this.ConsumerHabReviews = new HashSet<ConsumerHabReview>();
            this.ConsumerHabReviews1 = new HashSet<ConsumerHabReview>();
            this.ConsumerHabReviews2 = new HashSet<ConsumerHabReview>();
            this.ConsumerHabReviews3 = new HashSet<ConsumerHabReview>();
            this.ConsumerServiceCoordinators = new HashSet<ConsumerServiceCoordinator>();
        }
    
        public int ContactId { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public string CompanyName { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public Nullable<bool> IsServiceCoordinator { get; set; }
        public string JobTitle { get; set; }
        public string EmailAddress { get; set; }
        public string Notes { get; set; }
        public Nullable<int> AgencyNameId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> ContactTypeId { get; set; }
        public bool IsDeleted { get; set; }
        public string Phone { get; set; }
        public string MobilePhone { get; set; }
        public string Signature { get; set; }
        public string SignaturePassword { get; set; }
        public string FileNumber { get; set; }
        public string PhoneExtension { get; set; }
        public string Fax { get; set; }
        public string CCO { get; set; }
    
        public virtual Agency Agency { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConsumerContactCall> ConsumerContactCalls { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConsumerHabPlan> ConsumerHabPlans { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConsumerPrintDocument> ConsumerPrintDocuments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Consumer> Consumers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Consumer> Consumers1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Consumer> Consumers2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Consumer> Consumers3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContactPhone> ContactPhones { get; set; }
        public virtual ContactType ContactType { get; set; }
        public virtual Department Department { get; set; }
        public virtual List List { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeDocument> EmployeeDocuments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConsumerEmployee> ConsumerEmployees { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Consumer> Consumers4 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Consumer> Consumers11 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConsumerNote> ConsumerNotes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConsumerHabReview> ConsumerHabReviews { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConsumerHabReview> ConsumerHabReviews1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConsumerHabReview> ConsumerHabReviews2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConsumerHabReview> ConsumerHabReviews3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConsumerServiceCoordinator> ConsumerServiceCoordinators { get; set; }
    }
}