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
    
    public partial class ConsumerNotificationRecipient
    {
        public int Id { get; set; }
        public int NotificationId { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string Name { get; set; }
        public int AddedById { get; set; }
        public Nullable<int> UpdatedById { get; set; }
        public System.DateTime DateCreated { get; set; }
        public Nullable<System.DateTime> DateUpdated { get; set; }
    
        public virtual SystemUser SystemUser { get; set; }
        public virtual ConsumerNotificationSetting ConsumerNotificationSetting { get; set; }
        public virtual SystemUser SystemUser1 { get; set; }
    }
}
