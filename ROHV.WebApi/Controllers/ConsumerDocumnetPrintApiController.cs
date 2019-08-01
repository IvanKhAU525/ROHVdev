using System;
using ROHV.Controllers;
using System.Web.Mvc;
using ROHV.Core.Consumer;
using System.Threading.Tasks;
using ROHV.WebApi.ViewModels;
using System.Collections.Generic;
using System.Linq;
using ROHV.EmailServiceCore;

namespace ROHV.WebApi.Controllers
{
    [Authorize]
    public class ConsumerDocumnetPrintApiController : BaseController
    {

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Save(ConsumerPrintDocumentViewModel model)
        {
            if (User == null) return null;
            ConsumerPrintDocumentsManagement manage = new ConsumerPrintDocumentsManagement(_context);
            Int32 id = 0;
            if (model.ConsumerPrintDocumentId == null)
            {
                model.DateCreated = DateTime.Now;
                model.StatusId = 1;
                id = await manage.Save(model.GetModel(), model.GetOutcomesModel(),model.GetActionsModel());
            }
            else
            {
                id = model.ConsumerPrintDocumentId.Value;
                await manage.Save(model.GetModel(), model.GetOutcomesModel(), model.GetActionsModel());

            }
            return Json(new { status = "ok", id = id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public ActionResult GetPdfReport(String documentId, Int32 documentTypeId, Boolean isEmpty)
        {

            if (User == null) return null;
            ConsumerPrintDocumentsManagement manage = new ConsumerPrintDocumentsManagement(_context);

            foreach (String key in Session.Keys.Cast<String>().Where(x => x.StartsWith("DocumentPDF_")).ToArray())
            {
                HttpContext.Session.Remove(key);
            }
            String name = "";
            Byte[] bytes = manage.GetPDF(documentId, documentTypeId, this, out name, isEmpty);
            Guid guid = Guid.NewGuid();
            Session["DocumentPDF_" + guid] = bytes;
            Session["DocumentName_" + guid] = name;
            String rootUrl = new Uri(Request.Url, Url.Content("~")).ToString();
            String url = rootUrl + "api/consumerdocumnetprintapi/getpdfhandler/" + guid;
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
        public async Task<ActionResult> SendEmail(String documentId, List<Int32> documentTypes, String email, String emailOther, string emailBody, String contactName)
        {
            
            if (User == null) return null;
            if (String.IsNullOrEmpty(email) && String.IsNullOrEmpty(emailOther) || documentTypes.Count == 0)
            {
                return Json(new { status = "error" });
            }
            ConsumerPrintDocumentsManagement manage = new ConsumerPrintDocumentsManagement(_context);
            List<EmailService.FileAttachment> files = new List<EmailService.FileAttachment>();
            foreach (var documentTypeId in documentTypes)
            {
                EmailService.FileAttachment file = new EmailService.FileAttachment();
                String name = "";
                Byte[] bytes = manage.GetPDF(documentId, documentTypeId, this, out name, false);
                file.Name = name+".pdf";
                file.FileBytes = bytes;
                files.Add(file);
            }
            //  email = "zionsaal@gmail.com";
            var strEmailBody = emailBody ?? "Documents";

            var aliasSender = User?.Identity?.Name;
            if (!String.IsNullOrEmpty(email))
            {
                await EmailService.SendEmailWithAttach(email, contactName, "RAYIM.ORG Print documents", strEmailBody, files, aliasSender);
            }
            if (!String.IsNullOrEmpty(emailOther))
            {
                await EmailService.SendEmailWithAttach(emailOther, contactName, "RAYIM.ORG Print documents", strEmailBody, files, aliasSender);
            }

            return Json(new { status = "ok" });
        }

        [Authorize]
        [HttpDelete]
        public async Task<ActionResult> Delete(Int32 id)
        {
            if (User == null) return null;

            ConsumerPrintDocumentsManagement manage = new ConsumerPrintDocumentsManagement(_context);
            await manage.Delete(id);
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }
    }
}