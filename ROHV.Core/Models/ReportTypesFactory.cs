using ROHV.Core.Enums;
using ROHV.Core.Models;
using ITCraftFrame.Factories;
using System.Linq;

namespace ROHV.Core
{
    public static class ReportTypesFactory
    {
        public static IPDFreportModel GetAppropriatePDFreportType(ServiceTypeIdEnum serviceTypeId, DocumentPrintTypeEnum docType)
        {
            return ClassTypeFactory.GetInstances<IPDFreportModel>()
                .Where(x => x.DocumentType == docType && x.ServiceTypeId == serviceTypeId)
                .FirstOrDefault();            
        }

        public static IPDFhabReviewReportModel GetAppropriateHabReviewReportType(ServiceTypeIdEnum serviceTypeId)
        {
            return ClassTypeFactory.GetInstances<IPDFhabReviewReportModel>()
                .Where(x => x.ServiceTypeId == serviceTypeId)
                .FirstOrDefault();
        }

    }
}
