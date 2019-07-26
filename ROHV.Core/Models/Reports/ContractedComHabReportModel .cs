using ROHV.Core.Enums;

namespace ROHV.Core.Models
{
    public class ContractedComHabReportModel : DocumentationRecordReportModel
    {
        public ContractedComHabReportModel()
        {
            ReportPath = "~/Views/PDFViews/ComHabDocumentationRecord.rdl";
            ServiceTypeId = ServiceTypeIdEnum.ContractedCommunityHabilitation;
            DocumentType = DocumentPrintTypeEnum.ContractedComHabDocumentationRecord;
            PartName = "-ContractedComHabDocumentationRecord";
        }
    }
}
