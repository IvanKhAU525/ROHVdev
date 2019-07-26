using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Models
{
    public class ConsumerEmployeeModel
    {
        public Int32? ConsumerEmployeeId { get; set; }
        public Int32 ContactId { get; set; }
        public String ContactName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Int32? ServiceId { get; set; }
        public String ServiceName { get; set; }
        public String Email { get; set; }
        public String CompanyName { get; set; }
        public Int32 ConsumerId { get; set; }
        public Decimal? Rate { get; set; }
        public int? MaxHoursPerWeek { get; set; }
        public int? MaxHoursPerYear { get; set; }
        public string RateNote { get; set; }        
        public virtual ContactModel Contact { get; set; }               

    }
}
