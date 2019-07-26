
using System;
using ROHV.Controllers;
using System.Web.Mvc;
using ROHV.Core.Consumer;
using System.Threading.Tasks;
using ROHV.WebApi.ViewModels;
using System.Collections.Generic;
using ROHV.Core.Services;
using System.Linq;
using ROHV.Core.Models;
using ITCraftFrame;

namespace ROHV.WebApi.Controllers
{
    [Authorize]
    public class ConsumerHabReviewsApiController : BaseController
    {

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Save(ConsumerHabReviewViewModel model)
        {
            if (User == null) return null;
            ConsumerHabReviewsManagement manage = new ConsumerHabReviewsManagement(_context);
            Int32 id = 0;
            if (model.ConsumerHabReviewId == null)
            {
                model.DateCreated = DateTime.Now;
                id = await manage.Save(model.GetModel());
            }
            else
            {
                id = model.ConsumerHabReviewId.Value;
                await manage.Save(model.GetModel());
            }
            return Json(new { status = "ok", id = id });
        }

        [Authorize]
        [HttpDelete]
        public async Task<ActionResult> Delete(Int32 id)
        {
            if (User == null) return null;

            ConsumerHabReviewsManagement manage = new ConsumerHabReviewsManagement(_context);
            if (!await manage.Delete(id))
            {
                return Json(new { status = "error", message = "You can't delete this record. Please contact to support team." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> GetDefaultData(Int32 consumerId)
        {
            if (User == null) return null;
            ConsumerHabReviewsManagement manage = new ConsumerHabReviewsManagement(_context);
            ConsumerHabReviewViewModel model = new ConsumerHabReviewViewModel(await manage.GetDefaultModel(consumerId));

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Authorize]
        public ActionResult GetPdfReport(Int32 habReviewId, DateTime? templateDate)
        {

            if (User == null) return null;
            ConsumerHabReviewsManagement manage = new ConsumerHabReviewsManagement(_context);

            foreach (String key in Session.Keys.Cast<String>().Where(x => x.StartsWith("DocumentPDF_")).ToArray())
            {
                HttpContext.Session.Remove(key);
            }
            String name = "";
            Byte[] bytes = manage.GetPDF(habReviewId, this, out name, templateDate);
            Guid guid = Guid.NewGuid();
            Session["DocumentPDF_" + guid] = bytes;
            Session["DocumentName_" + guid] = name;
            String rootUrl = new Uri(Request.Url, Url.Content("~")).ToString();

            String url = rootUrl + "api/consumerhabreviewsapi/getpdfhandler/" + guid;
            return Json(new { status = "ok", url = url });
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
            Response.AddHeader("Content-Disposition", "inline; filename=" + name + ".pdf");

            return File(streamBytes, "application/pdf");
        }

        [HttpPost]
        [Authorize]
        public ActionResult GetAvailableReportTemplates(Int32 reportId)
        {
            if (User == null) return null;

            var manage = new ConsumerHabReviewsManagement(_context);

            var reportTemplates = ReportManager.GetAvailableReportTemplates(Core.Enums.ReportType.ComHabPlanReview, _context);
            var reportDate = manage.GetHabReview(reportId).DateReview;

            var filteredTemplates = GetFilteredReportTemplates(reportTemplates, reportDate);


            return Json(new { status = "ok", reportTemplates = filteredTemplates });
        }

        private List<ReportTemplateViewModel> GetFilteredReportTemplates(List<ReportTemplateModel> templates, DateTime? reportDate)
        {
            var mappedList = CustomMapper.MapList<ReportTemplateViewModel, ReportTemplateModel>(templates);

            mappedList.ForEach(template =>
            {
                template.IsSelected = ReportManager.IsReportTemplateApplied(reportDate, template); ;
            });

            return mappedList;
        }

    }
}