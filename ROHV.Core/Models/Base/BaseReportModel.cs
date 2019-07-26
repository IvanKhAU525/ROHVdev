using ROHV.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Models.Base
{
    public abstract class BaseReportModel
    {
        public string ReportPath { get; set; }
        public virtual Enums.ReportType ReportTypeName => Enums.ReportType.None;
        protected virtual String DefaultReportPath => "";
        public void CalculateTemplatePath(DateTime? templateDate, RayimContext context)
        {
            String templatePath = String.Empty;

            if (templateDate.HasValue)
            {
                templatePath = context.ReportTemplates
                        .FirstOrDefault(x => x.ReportType.Name == ReportTypeName.ToString() && x.StartDate == templateDate).ReportPath;
            }

            ReportPath = String.IsNullOrEmpty(templatePath) ? DefaultReportPath : templatePath;
        }
    }
}
