using ROHV.Core.Enums;

namespace ROHV.Core.Models
{
    public class GroupDayHabReportModel : DocumentationRecordReportModel
    {        
        public GroupDayHabReportModel()
        {
            ReportPath = "~/Views/PDFViews/DayGroupDocumentationRecord.rdl";
            ServiceTypeId = ServiceTypeIdEnum.GroupDayHabilitation;
            DocumentType = DocumentPrintTypeEnum.GroupDayHabDocumentationRecord;
            PartName = "-DayGroupDocumentationRecord";
        }

    }
}
