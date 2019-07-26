using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ROHV.Core.Database;
using ROHV.Core.Enums;

namespace ROHV.Core.Models
{
    public class ComHab6MonthReviewReportModel :  HabReviewReportModel
    {     
        public ComHab6MonthReviewReportModel()
        {
            ReportPath = DefaultReportPath;
            ServiceTypeId = ServiceTypeIdEnum.CommunityHabilitation;
            TitleDocument = "Community Habilitation 6 Month Review";
            CoordinatorLabel = "CH";
            ShowReviewedBy = false;
            Name = "ComHab6MonthReview";
        }

        public override Contact GetCoordinatorContact(ConsumerHabReview document)
        {
            return CoordinatorContact= document.ContactCHCoordinator ?? null;
        }
    }
}
