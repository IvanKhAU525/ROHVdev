using ROHV.Core.Enums;

namespace ROHV.Core.Models
{
    public class ContractedComHabMonthlyProgressSummaryModel : MonthlyProgressReportModel
    {
        public ContractedComHabMonthlyProgressSummaryModel()
        {
            ReportPath = "~/Views/PDFViews/ComHabMonthlyProgressSummary.rdl";
            ServiceTypeId = ServiceTypeIdEnum.ContractedCommunityHabilitation;
            DocumentType = DocumentPrintTypeEnum.ContractedComHubMonthlyProgressSummary;
            PartName = "-ContractedComHabMonthlyProgressSummary";
        }

    }
}
