using ROHV.Core.Enums;

namespace ROHV.Core.Models
{
    public class ComHabReportModel : DocumentationRecordReportModel
    {
        public ComHabReportModel()
        {
            ReportPath = "~/Views/PDFViews/ComHabDocumentationRecord.rdl";
            ServiceTypeId = ServiceTypeIdEnum.CommunityHabilitation;
            DocumentType = DocumentPrintTypeEnum.ComHabDocumentationRecord;
            PartName = "-ComHabDocumentationRecord";
        }
    }
}
