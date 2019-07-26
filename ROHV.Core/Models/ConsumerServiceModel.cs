using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Models
{
    public class ConsumerServiceModel
    {
        public virtual Int32? ConsumerServiceId { get; set; }
        public virtual Int32? ServiceId { get; set; }
        public virtual String ServiceName { get; set; }
        public virtual DateTime? DateInactive { get; set; }
        public virtual DateTime? EffectiveDate { get; set; }
        public virtual DateTime? AgencyDate { get; set; }
        public virtual Int32? AnnualUnits { get; set; }
        public virtual Int32? UnitQuantities { get; set; }
        public virtual Decimal? TotalHours { get; set; }
        public virtual String UnitQuantitiesName { get; set; }
        public virtual String DWorkers { get; set; }
        public virtual String Notes { get; set; }
        public virtual bool Inactive { get; set; }
        public virtual int? CreatedByUserId { set; get; }   
        public virtual Int32? ConsumerId { get; set; }
        public DateTime? UsedHoursStartDate { get; set; }
        public DateTime? UsedHoursEndDate { get; set; }
        public decimal? UsedHours { get; set; }

        public virtual UserModel CreatedByUser { set; get; }
        public virtual int? EditedByUserId { set; get; }
        public virtual UserModel EditedByUser { set; get; }
        public virtual ServicesListModel ServicesList { set; get; }

        public virtual List<ConsumerEmployeeModel> ConsumerEmployeeList { set; get; }
    }
}
