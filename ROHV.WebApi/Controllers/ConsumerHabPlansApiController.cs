using System;
using ROHV.Controllers;
using ITCraftFrame;
using System.Web.Mvc;
using ROHV.Core.Consumer;
using System.Threading.Tasks;
using ROHV.WebApi.ViewModels;
using System.Collections.Generic;
using ROHV.Core.Services;
using System.Linq;
using ROHV.Core.Models;

namespace ROHV.WebApi.Controllers
{
    [Authorize]
    public class ConsumerHabPlansApiController : BaseController
    {

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Save(ConsumerHabPlanViewModel model)
        {
            if (User == null) return null;
            ConsumerHabPlansManagement manage = new ConsumerHabPlansManagement(_context);
            Int32 id = 0;
            if (model.ConsumerHabPlanId == null)
            {
                model.DateCreated = DateTime.Now;
                id = await manage.Save(model.GetModel(), model.GetOutcomesModel(),model.GetActionsModel(), model.GetSafeguardsModel());
            }
            else
            {
                id = model.ConsumerHabPlanId.Value;
                await manage.Save(model.GetModel(), model.GetOutcomesModel(), model.GetActionsModel(), model.GetSafeguardsModel());
            }
            return Json(new { status = "ok", id = id }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpDelete]
        public async Task<ActionResult> Delete(Int32 id)
        {
            if (User == null) return null;

            ConsumerHabPlansManagement manage = new ConsumerHabPlansManagement(_context);
            if (!await manage.Delete(id))
            {
                return Json(new { status = "error", message = "You can't delete this record. Please contact to support team." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public ActionResult GetPdfReport(Int32 habPlanId, DateTime? templateDate)
        {
            if (User == null) return null;
            ConsumerHabPlansManagement manage = new ConsumerHabPlansManagement(_context);

            foreach (String key in Session.Keys.Cast<String>().Where(x => x.StartsWith("DocumentPDF_")).ToArray())
            {
                HttpContext.Session.Remove(key);
            }

            Byte[] bytes = manage.GetPDF(habPlanId, this, templateDate, out String name);
            Guid guid = Guid.NewGuid();
            Session["DocumentPDF_" + guid] = bytes;
            Session["DocumentName_" + guid] = name;
            String rootUrl = new Uri(Request.Url, Url.Content("~")).ToString();
            String url = rootUrl + "api/consumerdocumnetprintapi/getpdfhandler/" + guid;

            return Json(new { status = "ok", url });
        }

        [HttpPost]
        [Authorize]
        public ActionResult GetAvailableReportTemplates(Int32 habPlanId)
        {
            if (User == null) return null;

            var manage = new ConsumerHabPlansManagement(_context);

            var reportTemplates = ReportManager.GetAvailableReportTemplates(Core.Enums.ReportType.ComHabPlan,_context);
            var habPlanDate = manage.GetHabPLan(habPlanId).DatePlan;
            var filteredTemplates = GetFilteredReportTemplates(reportTemplates, habPlanDate);

            return Json(new { status = "ok", reportTemplates = filteredTemplates });
        }

        private List<ReportTemplateViewModel> GetFilteredReportTemplates(List<ReportTemplateModel> templates, DateTime habPlanDate)
        {
            var mappedList = CustomMapper.MapList<ReportTemplateViewModel, ReportTemplateModel>(templates);

            mappedList.ForEach(template =>
            {
                template.IsSelected = ReportManager.IsReportTemplateApplied(habPlanDate, template);
            });

            return mappedList;
        }
       

        [HttpGet]
        [Authorize]
        public FileResult GetPdfHandler(String id)
        {
            String key = "DocumentPDF_" + id;
            String keyName = "DocumentName_" + id;
            Byte[] streamBytes = (Byte[])Session[key];
            String name = (String)Session[keyName];
            if (streamBytes == null) return null;
            HttpContext.Session.Remove(key);

            return File(streamBytes, "application/pdf", name + ".pdf");
        }
    }
}