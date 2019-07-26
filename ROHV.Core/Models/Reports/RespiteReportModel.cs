using ROHV.Core.Database;
using ROHV.Core.Enums;
using System;

namespace ROHV.Core.Models
{
    class RespiteReportModel : DocumentationRecordReportModel
    {
        public RespiteReportModel()
        {
            ReportPath = "~/Views/PDFViews/RespiteDocumentationRecord.rdl";
            PartName = "-RespiteDocumentationRecord";
            ServiceTypeId = ServiceTypeIdEnum.Respite;
            DocumentType = DocumentPrintTypeEnum.RespiteDocumentationRecord;
        }
    }
}
