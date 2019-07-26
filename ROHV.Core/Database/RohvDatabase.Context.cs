﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class RayimContext : DbContext
    {
        public RayimContext()
            : base("name=RayimContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Agency> Agencies { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<CaseStatu> CaseStatus { get; set; }
        public virtual DbSet<ConsumerContactCall> ConsumerContactCalls { get; set; }
        public virtual DbSet<ConsumerHabPlanDuration> ConsumerHabPlanDurations { get; set; }
        public virtual DbSet<ConsumerHabPlanFrequency> ConsumerHabPlanFrequencies { get; set; }
        public virtual DbSet<ConsumerHabPlan> ConsumerHabPlans { get; set; }
        public virtual DbSet<ConsumerHabPlanStatus> ConsumerHabPlanStatuses { get; set; }
        public virtual DbSet<ConsumerHabPlanVOServeAction> ConsumerHabPlanVOServeActions { get; set; }
        public virtual DbSet<ConsumerNotificationRecipient> ConsumerNotificationRecipients { get; set; }
        public virtual DbSet<ConsumerNotificationSetting> ConsumerNotificationSettings { get; set; }
        public virtual DbSet<ConsumerPhone> ConsumerPhones { get; set; }
        public virtual DbSet<ConsumerPrintDocument> ConsumerPrintDocuments { get; set; }
        public virtual DbSet<Consumer> Consumers { get; set; }
        public virtual DbSet<ConsumerService> ConsumerServices { get; set; }
        public virtual DbSet<ConsumerTherapy> ConsumerTherapies { get; set; }
        public virtual DbSet<ContactPhone> ContactPhones { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<ContactType> ContactTypes { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<DocumentPrintStatus> DocumentPrintStatuses { get; set; }
        public virtual DbSet<DocumentPrintType> DocumentPrintTypes { get; set; }
        public virtual DbSet<EmployeeDocumentNote> EmployeeDocumentNotes { get; set; }
        public virtual DbSet<EmployeeDocument> EmployeeDocuments { get; set; }
        public virtual DbSet<EmployeeDocumentStatus> EmployeeDocumentStatuses { get; set; }
        public virtual DbSet<EmployeeDocumentType> EmployeeDocumentTypes { get; set; }
        public virtual DbSet<ListCategory> ListCategories { get; set; }
        public virtual DbSet<List> Lists { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<NotificationStatus> NotificationStatuses { get; set; }
        public virtual DbSet<NotificationType> NotificationTypes { get; set; }
        public virtual DbSet<ServicesList> ServicesLists { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<SystemUser> SystemUsers { get; set; }
        public virtual DbSet<Category_List> Category_Lists { get; set; }
        public virtual DbSet<ServiceType> ServiceTypes { get; set; }
        public virtual DbSet<ConsumerEmployee> ConsumerEmployees { get; set; }
        public virtual DbSet<ConsumerPrintDocumentValuedOutcome> ConsumerPrintDocumentValuedOutcomes { get; set; }
        public virtual DbSet<ConsumerPrintDocumentVOServeAction> ConsumerPrintDocumentVOServeActions { get; set; }
        public virtual DbSet<ConsumerNoteFromType> ConsumerNoteFromTypes { get; set; }
        public virtual DbSet<ConsumerNote> ConsumerNotes { get; set; }
        public virtual DbSet<ConsumerNoteType> ConsumerNoteTypes { get; set; }
        public virtual DbSet<ConsumerHabPlanSafeguard> ConsumerHabPlanSafeguards { get; set; }
        public virtual DbSet<ConsumerHabReviewIssueState> ConsumerHabReviewIssueStates { get; set; }
        public virtual DbSet<ConsumerHabReview> ConsumerHabReviews { get; set; }
        public virtual DbSet<ConsumerServiceCoordinator> ConsumerServiceCoordinators { get; set; }
        public virtual DbSet<ConsumerAddress> ConsumerAddresses { get; set; }
        public virtual DbSet<Audit> Audits { get; set; }
        public virtual DbSet<EntityType> EntityTypes { get; set; }
        public virtual DbSet<FileMetaData> FileMetaDatas { get; set; }
        public virtual DbSet<ConsumerMedicaidNumber> ConsumerMedicaidNumbers { get; set; }
        public virtual DbSet<ReportTemplate> ReportTemplates { get; set; }
        public virtual DbSet<ReportType> ReportTypes { get; set; }
        public virtual DbSet<ConsumerHabPlanValuedOutcome> ConsumerHabPlanValuedOutcomes { get; set; }
        public virtual DbSet<vTimeSheetData> vTimeSheetDatas { get; set; }
    }
}
