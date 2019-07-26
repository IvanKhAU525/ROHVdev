using ROHV.Core.Enums;

namespace ROHV.Core.Models.Reports
{
    public class SupplementalDayGroupMonthlyProgressSummaryModel : MonthlyProgressReportModel
    {
        public SupplementalDayGroupMonthlyProgressSummaryModel()
        {
            ReportPath = "~/Views/PDFViews/SupplementalDayGroupMonthlyProgressSummary.rdl";
            ServiceTypeId = ServiceTypeIdEnum.SupplementalGroupDayHabilitation;
            DocumentType = DocumentPrintTypeEnum.SupplementalGroupDayMonthlyProgresSummary;
            PartName = "-DayGroupMonthlyProgressSummary";
        }
    }
}
