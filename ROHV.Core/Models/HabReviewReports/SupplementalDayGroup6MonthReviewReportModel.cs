using ROHV.Core.Database;
using ROHV.Core.Enums;

namespace ROHV.Core.Models
{
    public class SupplementalDayGroup6MonthReviewReportModel : HabReviewReportModel
    {
        public SupplementalDayGroup6MonthReviewReportModel()
        {
            ReportPath = DefaultReportPath;
            ServiceTypeId = ServiceTypeIdEnum.SupplementalGroupDayHabilitation;
            TitleDocument = "Supplemental Group Day Habilitation 6 Month Review";
            CoordinatorLabel = "DH";
            ShowReviewedBy = true;
            Name = "SupplementalDayGroup6MonthReview";
        }

        public override Contact GetCoordinatorContact(ConsumerHabReview document)
        {
            return CoordinatorContact = document.ContactDHCoordinator ?? null;
        }

    }
}
