using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ROHV.Core.Database;
using ROHV.Core.Enums;
using ROHV.Core.Models.Base;

namespace ROHV.Core.Models
{
    public abstract class HabReviewReportModel : BaseReportModel, IPDFhabReviewReportModel
    {
        public override Enums.ReportType ReportTypeName => Enums.ReportType.ComHabPlanReview;
        protected override string DefaultReportPath => "~/Views/PDFViews/6MonthReview.rdl";
        public ServiceTypeIdEnum ServiceTypeId { get; set; }
        public string TitleDocument { get; set; }
        public Contact CoordinatorContact { get; set; }
        public string CoordinatorLabel { get; set; }
        public bool ShowReviewedBy { get; set; }
        public string Name { get; set; }

        public virtual IEnumerable<object> CreateDataSet(ConsumerHabReview document)
        {
            return new List<object>();
        }

        public virtual Contact GetCoordinatorContact(ConsumerHabReview document)
        {
            return new Contact();
        }
        

    }
}
