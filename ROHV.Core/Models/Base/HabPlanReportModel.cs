using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ROHV.Core.Consumer;
using ROHV.Core.Database;
using ROHV.Core.Enums;
using ROHV.Core.Models.Base;

namespace ROHV.Core.Models
{
    public abstract class HabPlanReportModel : BaseReportModel, IPDFhabPlanReportModel
    {
        protected override   String DefaultReportPath => "~/Views/PDFViews/ComHabPlan.rdl";
        public override Enums.ReportType ReportTypeName => Enums.ReportType.ComHabPlan;
        public ServiceTypeIdEnum ServiceTypeId { get; set; }
        public string TitleDocument { get; set; }
        public Contact CoordinatorContact { get; set; }
        public string CoordinatorLabel { get; set; }
        public bool ShowReviewedBy { get; set; }
        public string Name { get; set; }
        public byte[] Signature { get; set; }
        public string SignatureType { get; set; }
        public string MSC { get; set; }
        public List<string> OutcomeList { get; set; }
        public List<string> ActionsList { get; set; }

        public virtual Dictionary<string, object> GetDataSets(ConsumerHabPlan habPlanEntity, ConsumerHabPlansManagement consumerHabPlanManagement)
        {
            return new Dictionary<string, object>();
        }
       

    }
}
