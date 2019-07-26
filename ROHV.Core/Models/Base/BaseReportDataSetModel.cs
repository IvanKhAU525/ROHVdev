using System;

namespace ROHV.Core.Models
{
    public class BaseReportDataSetModel
    {
        public string PatientName { get; set; }
        public string EffectiveDate { get; set; }
        public string Address { get; set; }
        public string ContactName { get; set; }
        public string JobTitle { get; set; }
        public string ServiceDescription { get; set; }
        public string WorkerAddress { get; set; }
        public string Medicaid { get; set; }
        public string Rate { get; set; }
        public string LastReviewDate { get; set; }
        public string Title { get; set; }
        public string ReportHeader { get; set; }
        public string ConsumerSityStateZip { get; set; }
        public string ContactSityStateZip { get; set; }
        public string ServiceAction1 { get; set; }
        public string ServiceAction2 { get; set; }
        public string ServiceAction3 { get; set; }
        public string ServiceAction4 { get; set; }
        public string ValuedOutcome1 { get; set; }
        public string ValuedOutcome2 { get; set; }
        public string ValuedOutcome3 { get; set; }
        public string ValuedOutcome4 { get; set; }
        public string ValuedOutcomeFormated { get; set; }

        public BaseReportDataSetModel()
        {
            PatientName = String.Empty;
            EffectiveDate = String.Empty;
            ContactName = String.Empty;
            JobTitle = String.Empty;
            ServiceDescription = String.Empty;
            WorkerAddress = String.Empty;
            Rate = String.Empty;
            Medicaid = String.Empty;
            LastReviewDate = String.Empty;
            Address = String.Empty;
            Title = String.Empty;
            ConsumerSityStateZip = String.Empty;
            ContactSityStateZip = String.Empty;
            ServiceAction1 = String.Empty;
            ServiceAction2 = String.Empty;
            ServiceAction3 = String.Empty;
            ServiceAction4 = String.Empty;
            ValuedOutcome1 = String.Empty;
            ValuedOutcome2 = String.Empty;
            ValuedOutcome3 = String.Empty;
            ValuedOutcome4 = String.Empty;
            ReportHeader = String.Empty;
        }
    }
}
