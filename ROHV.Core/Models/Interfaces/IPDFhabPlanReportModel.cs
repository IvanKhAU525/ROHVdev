using ROHV.Core.Consumer;
using ROHV.Core.Database;
using ROHV.Core.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ROHV.Core.Models
{
    public interface IPDFhabPlanReportModel
    {
        string ReportPath { get; set; }

        ServiceTypeIdEnum ServiceTypeId { get; set; }

        string TitleDocument { get; set; }

        string Name { get; set; }

        Contact CoordinatorContact { get; set; }

        byte[] Signature { get; set; }

        string SignatureType { get; set; }

        string CoordinatorLabel { get; set; }

        string MSC { get; set; }

        List<String> OutcomeList { get; set; }

        List<String> ActionsList { get; set; }

        bool ShowReviewedBy { get; set; }

        Dictionary<string, object> GetDataSets(ConsumerHabPlan habPlanEntity, ConsumerHabPlansManagement consumerHabPlanManagement);
        
    }
}