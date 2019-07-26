using ROHV.Core.Enums;
using System;

namespace ROHV.Core.Models
{
    public class SelfHiredComHabMonthlyProgressSummaryModel : MonthlyProgressReportModel
    {
        public SelfHiredComHabMonthlyProgressSummaryModel()
        {
            ReportPath = "~/Views/PDFViews/SelfHiredComHabMonthlyProgressSummary.rdl";
            ServiceTypeId = ServiceTypeIdEnum.SelfHired;
            DocumentType = DocumentPrintTypeEnum.SelfHiredComHubMonthlyProgressSummary;
            PartName = "-SelfHiredComHabMonthlyProgressSummary";
        }
    }
}
