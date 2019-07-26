using ROHV.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Models
{
    public class ApprovedServiceBoundModel : ConsumerServiceModel
    {

        [EmailBound(Name = "[ServiceName]")]
        public string ViewServiceName { get => ServicesList?.ServiceDescription; }
        [EmailBound(Name = "[EffectiveDate]")]
        public string ViewEffectiveDate { get => EffectiveDate.ToDateString(); }

        [EmailBound(Name = "[CreatedBy]")]
        public string ViewCreatedBy { get => String.Format("{0} {1}", CreatedByUser?.FirstName, CreatedByUser?.LastName); }

        [EmailBound(Name = "[AnnualUnits]")]
        public string ViewAnnualUnits { get => AnnualUnits?.ToString(); }

        [EmailBound(Name = "[TotalHours]")]
        public string ViewTotalHours { get => TotalHours?.ToString(); }

        [EmailBound(Name = "[DateInactive]")]
        public string ViewDateInactive { get => DateInactive.ToDateString(); }

        [EmailBound(Name = "[Inactive]")]
        public string ViewInactive { get => Inactive.ToString(); }

        [EmailBound(Name = "[Notes]")]
        public string ViewNotes { get => Notes; }

        [EmailBound(Name = "[InnerEmailBody]")]
        public string InnerEmailBody { set; get; }

        [EmailBound(Name = "[Employees]")]
        public string ViewEmployeesContacts { get => String.Join(", ", ConsumerEmployeeList.Select(x => String.Format("{0} {1}", x.Contact?.FirstName, x.Contact?.LastName))); }




    }
}
