using ROHV.Core.Enums;

namespace ROHV.Core.Models
{
    public class ComHabMonthlyProgressSummaryModel : MonthlyProgressReportModel
    {
        public ComHabMonthlyProgressSummaryModel()
        {
            ReportPath = "~/Views/PDFViews/ComHabMonthlyProgressSummary.rdl";
            ServiceTypeId = ServiceTypeIdEnum.CommunityHabilitation;
            DocumentType = DocumentPrintTypeEnum.ComHubMonthlyProgressSummary;
            PartName = "-ComHabMonthlyProgressSummary";
        }

    }
}
