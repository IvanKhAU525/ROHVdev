using ROHV.Core.Database;
using ROHV.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Consumer
{
    public static class ReportManager
    {
        public static List<ReportTemplateModel> GetAvailableReportTemplates(Enums.ReportType reportType, RayimContext _context)
        {
            var templates = _context.ReportTemplates
                    .Where(x => x.ReportType.Name == reportType.ToString())
                    .Select(x => new ReportTemplateModel { StartDate = x.StartDate, EndDate = x.EndDate, Description = x.Description }).ToList();

            templates.Insert(0, new ReportTemplateModel { Description = "Default" });

            return templates;
        }
        public static  Boolean IsReportTemplateApplied(DateTime? reportDate, ReportTemplateModel template)
        {
            return reportDate != null && ((reportDate >= template.StartDate && template.EndDate == null) || reportDate >= template.StartDate && reportDate <= template.EndDate);
        }
    }
}
