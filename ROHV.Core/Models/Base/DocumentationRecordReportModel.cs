using ROHV.Core.Consumer;
using ROHV.Core.Database;
using System.Collections.Generic;

namespace ROHV.Core.Models
{
    public class DocumentationRecordReportModel : PdfReportModel
    {
        public override IEnumerable<object> CreateDataSet(ConsumerPrintDocument document, bool isEmpty, ConsumerPrintDocumentsManagement consumerManagement)
        {
            return consumerManagement.GenerateDataSetForDocumentationRecord(DocumentType ,document, isEmpty);
        }
    }
}
