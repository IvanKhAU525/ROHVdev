using System;
using ROHV.Core.Database;
using ROHV.Core.Enums;

namespace ROHV.Core.Models
{
    public class SelfHiredRespiteReportModel : DocumentationRecordReportModel
    {
        public SelfHiredRespiteReportModel()
        {
            ReportPath = "~/Views/PDFViews/SelfHiredRespiteDocumentationRecord.rdl";
            PartName = "-SelfHiredRespiteDocumentationRecord";
            ServiceTypeId = ServiceTypeIdEnum.SelfHired;
            DocumentType = DocumentPrintTypeEnum.SelfHiredRespiteDocumentationRecord;
        }

    }
}
