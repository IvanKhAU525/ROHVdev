using ROHV.Core.Consumer;
using ROHV.Core.Database;
using ROHV.Core.Enums;
using System.Collections.Generic;

namespace ROHV.Core.Models
{
    public interface IPDFreportModel
    {
        string ReportPath { get; set; }
        ServiceTypeIdEnum ServiceTypeId { get; set; }
        string PartName { get; set; }
        DocumentPrintTypeEnum DocumentType { get; set; }

        string GetName(string additinalNamePart);

        IEnumerable<object> CreateDataSet(ConsumerPrintDocument document, bool isEmpty, ConsumerPrintDocumentsManagement consumerManagement);
    }
}