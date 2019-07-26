using ROHV.Core.Database;
using ROHV.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Models
{
    public class DayGroup6MonthReviewReportModel : HabReviewReportModel
    {
        public DayGroup6MonthReviewReportModel()
        {
            ReportPath = DefaultReportPath;
            ServiceTypeId = ServiceTypeIdEnum.GroupDayHabilitation;
            TitleDocument = "Group Day Habilitation 6 Month Review";
            CoordinatorLabel = "DH";
            ShowReviewedBy = true;
            Name = "DayGroup6MonthReview";
        }

        public override Contact GetCoordinatorContact(ConsumerHabReview document)
        {
            return CoordinatorContact = document.ContactDHCoordinator ?? null;
        }

    }
}
