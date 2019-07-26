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
    
    public partial class ConsumerHabPlanValuedOutcome
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ConsumerHabPlanValuedOutcome()
        {
            this.ConsumerHabPlanVOServeActions = new HashSet<ConsumerHabPlanVOServeAction>();
        }
    
        public int Id { get; set; }
        public Nullable<int> HabPlanId { get; set; }
        public string ValuedOutcome { get; set; }
        public string CQLPOM { get; set; }
        public string MyGoal { get; set; }
        public bool IsIPOP { get; set; }
    
        public virtual ConsumerHabPlan ConsumerHabPlan { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConsumerHabPlanVOServeAction> ConsumerHabPlanVOServeActions { get; set; }
    }
}
