using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Models
{
    public class ConsumerModel
    {
        public Int32? ConsumerId { get; set; }
        public string FirstName { get; set; }
        public string MI { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string ParentName { get; set; }
        public Int32? AdvocateId { get; set; }
        public String AdvocateName { get; set; }

        public Int32? AdvocatePaperId { get; set; }
        public String AdvocatePaperName { get; set; }

        public Int32? Status { get; set; }
        public String StatusName { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string MedicaidNo { get; set; }
        public string SocialSecurityNo { get; set; }
        public Int32? PrimaryDiagnosis { get; set; }
        public Int32? SecondaryDiagnosis { get; set; }
        public string TABSNo { get; set; }
        public Boolean HasServiceCoordinator { get; set; }
        public string AgencyName { get; set; }

        public Int32? DayProgramId { get; set; }
        public string DayProgramName { get; set; }

        public Decimal? Rate { get; set; }
        public Int32? MaxHoursPerWeek { get; set; }
        public Int32? MaxHoursPerYear { get; set; }
        public string RateNote { get; set; }
        public string Cc2 { get; set; }
    }
}
