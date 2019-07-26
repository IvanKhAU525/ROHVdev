using ROHV.Core.Consumer;
using ROHV.Core.Database;
using System.Collections.Generic;

namespace ROHV.Core.Models
{
    public class MonthlyProgressReportModel : PdfReportModel
    {
        public override IEnumerable<object> CreateDataSet(ConsumerPrintDocument document, bool isEmpty, ConsumerPrintDocumentsManagement consumerManagement)
        {
            return consumerManagement.GenerateDataSetForMonthlyProgressSummary(DocumentType,document, isEmpty);
        }
    }
}
