using ROHV.Core.Enums;

namespace ROHV.Core.Models
{   
    public class BrokerReportModel : DocumentationRecordReportModel
    {
        public BrokerReportModel()
        {
            ReportPath = "~/Views/PDFViews/BrokerDocumentationRecord.rdl";
            ServiceTypeId = ServiceTypeIdEnum.Broker;
            DocumentType = DocumentPrintTypeEnum.BrokerDocumentationRecord;
            PartName = "-BrokerDocumentationRecord";
        }
    }
}
