using ROHV.Core.Enums;

namespace ROHV.Core.Models
{
    public class SupplementalGroupDayHabReportModel : DocumentationRecordReportModel
    {        
        public SupplementalGroupDayHabReportModel()
        {
            ReportPath = "~/Views/PDFViews/SupplementalDayGroupDocumentationRecord.rdl";
            ServiceTypeId = ServiceTypeIdEnum.SupplementalGroupDayHabilitation;
            DocumentType = DocumentPrintTypeEnum.SupplementalGroupDayHabDocumentationRecord;
            PartName = "-DayGroupDocumentationRecord";
        }

    }
}
