using ROHV.Core.Consumer;
using ROHV.Core.Database;
using ROHV.Core.Enums;
using ROHV.Core.Models.Base;
using System;
using System.Collections.Generic;

namespace ROHV.Core.Models
{
    public abstract class PdfReportModel : BaseReportModel, IPDFreportModel
    {    
        public ServiceTypeIdEnum ServiceTypeId { get; set; }
        public DocumentPrintTypeEnum DocumentType { get; set; }
        public string PartName { get; set; }

        public string GetName(string additinalNamePart)
        {
            return String.Concat(additinalNamePart, this.PartName);
        }

        public virtual IEnumerable<object> CreateDataSet(ConsumerPrintDocument document, bool isEmpty, ConsumerPrintDocumentsManagement consumerManagement)
        {
            return new List<object>();
        }

    }
}
