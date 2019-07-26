using ROHV.Core.Enums;

namespace ROHV.Core.Models.Reports
{
    public class DayGroupMonthlyProgressSummaryModel : MonthlyProgressReportModel
    {
        public DayGroupMonthlyProgressSummaryModel()
        {
            ReportPath = "~/Views/PDFViews/DayGroupMonthlyProgressSummary.rdl";
            ServiceTypeId = ServiceTypeIdEnum.GroupDayHabilitation;
            DocumentType = DocumentPrintTypeEnum.GroupDayMonthlyProgresSummary;
            PartName = "-DayGroupMonthlyProgressSummary";
        }
    }
}
