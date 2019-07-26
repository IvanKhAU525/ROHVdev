using ITCraftFrame;
using ROHV.Core.Consumer;
using ROHV.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ROHV.Core.Models.HabPlanReports
{
    [Serializable]
    public class Test
    {
        public string T1 { set; get; }
    }
    public class ComHabPlanReportModel : HabPlanReportModel
    {        
        public ComHabPlanReportModel() { }

        public override Dictionary<string, object> GetDataSets(ConsumerHabPlan habPlanEntity, ConsumerHabPlansManagement consumerHabPlanManagement)
        {
            Dictionary<string, object> dataSets = new Dictionary<string, object>();
            if (habPlanEntity == null)
            {
                return dataSets;
            }

            List<ReportHabPlanOutcomeValue> valueOutcomeList = consumerHabPlanManagement.GetReportHabPlanOutcomeValueList(habPlanEntity);
            IEnumerable<object> timeObjs = new[]
            {
                    new
                    {
                    PatientName = consumerHabPlanManagement.GetName(habPlanEntity.Consumer),
                    PatientDOB = consumerHabPlanManagement.GetDate(habPlanEntity.Consumer.DateOfBirth),
                    EnrollmentDate = consumerHabPlanManagement.GetDate(habPlanEntity.EnrolmentDate),
                    SignatureDate = consumerHabPlanManagement.GetDate(habPlanEntity.SignatureDate),
                    CHCoordinator = habPlanEntity.Contact != null ? consumerHabPlanManagement.GetName(habPlanEntity.Contact) : "",
                    Frequency = habPlanEntity.ConsumerHabPlanFrequency.Name,
                    Duration = habPlanEntity.ConsumerHabPlanDuration.Name,
                    Medicaid = consumerHabPlanManagement.GetMedicaidNumberByDate(habPlanEntity.Consumer, habPlanEntity.DatePlan),
                    HabService = habPlanEntity.ServicesList.ServiceDescription,
                    DateOfPlan = consumerHabPlanManagement.GetDate(habPlanEntity.DatePlan),
                    EffectiveDate = consumerHabPlanManagement.GetDate(habPlanEntity.EffectivePlan),
                    MSC = habPlanEntity.Consumer.ServiceCoordinatorContact != null ? consumerHabPlanManagement.GetScheduledMSCName(habPlanEntity.Consumer,habPlanEntity.DatePlan) : "",
                    CCO = habPlanEntity.Consumer.ServiceCoordinatorContact?.CCO ?? "",
                    Outcome1 = valueOutcomeList.GetSafeDataByIndex(0) ?? new ReportHabPlanOutcomeValue(),
                    Outcome2 = valueOutcomeList.GetSafeDataByIndex(1) ?? new ReportHabPlanOutcomeValue(),
                    Outcome3 = valueOutcomeList.GetSafeDataByIndex(2)?? new ReportHabPlanOutcomeValue(),
                    Outcome4 = valueOutcomeList.GetSafeDataByIndex(3) ?? new ReportHabPlanOutcomeValue(),
                    
                    TypeDocument = consumerHabPlanManagement.GetServiceName(habPlanEntity),
                    Safeguards = consumerHabPlanManagement.GetSafeguards(habPlanEntity),                   
                    ShowReviewedBy = !habPlanEntity.ServicesList.ServiceType.Contains("Community"),
                    CHSignature = consumerHabPlanManagement.GetSignature(habPlanEntity),
                    CHSignatureMimeType = consumerHabPlanManagement.GetSignatureType(habPlanEntity),
                    CoordinatorLabel = consumerHabPlanManagement.GetCoordinatorLabel(habPlanEntity)
                    }
                };

            dataSets.Add("DataSet1", timeObjs);
            dataSets.Add("DataSetSafeGuards", consumerHabPlanManagement.GetSafeguardsWithActions(habPlanEntity));
            return dataSets;
        }       
    }
}
