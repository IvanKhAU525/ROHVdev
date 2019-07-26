using ROHV.Core.Enums;
using System;

namespace ROHV.Core.Models
{
    public class SelfHiredComHabReportModel : DocumentationRecordReportModel
    {
        public SelfHiredComHabReportModel()
        {
            ReportPath = "~/Views/PDFViews/SelfHiredComHabDocumentationRecord.rdl";
            ServiceTypeId = ServiceTypeIdEnum.SelfHired;
            DocumentType = DocumentPrintTypeEnum.SelfHiredComHubDocumentationRecord;
            PartName = "-SelfHiredComHabDocumentationRecord";
        }
    }
}
