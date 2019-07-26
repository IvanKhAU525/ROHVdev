
using System;
using ROHV.Controllers;
using System.Web.Mvc;
using ROHV.Core.Consumer;
using System.Threading.Tasks;
using ROHV.WebApi.ViewModels;
using System.Collections.Generic;
using ROHV.Core.Services;
using System.Linq;
using System.Web.Script.Serialization;
using ROHV.Core.Enums;

namespace ROHV.WebApi.Controllers
{
    [Authorize]
    public class ConsumerEmployeeApiController : BaseController
    {

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Save()
        {
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            string data = new System.IO.StreamReader(Request.InputStream).ReadToEnd();
            ConsumerEmployeeViewModel model = serializer.Deserialize<ConsumerEmployeeViewModel>(data);

            if (User == null) return null;
            ConsumerEmployeeManagement manage = new ConsumerEmployeeManagement(_context);
            Int32 id = 0;
            if (!model.ConsumerEmployeeId.HasValue)
            {                                
                id = await manage.Save(model.GetModel());
            }
            else
            {
                id = model.ConsumerEmployeeId.Value;
                await manage.Save(model.GetModel());                
            
            }  
            if (!string.IsNullOrEmpty(model.FileData))
            {
                var savedFileInfo = manage.SaveConsumerEmployeeFile(CurrentUser, id, model.FileData, model.FileName, model.FileId);
                model.FileId = savedFileInfo.fileId;
                model.FileName = savedFileInfo.fileName;
            }
            return Json(new { status = "ok", id = id, fileId = model.FileId, fileName = model.FileName }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpDelete]
        public async Task<ActionResult> Delete(Int32 serviceId)
        {
            if (User == null) return null;

            ConsumerEmployeeManagement manage = new ConsumerEmployeeManagement(_context);
            if (!await manage.Delete(serviceId))
            {
                return Json(new { status = "error", message = "You can't delete this record. Please contact to support team." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> SendEmail(ConsumerEmailViewModel model)
        {
            try
            {
                String message = this.RenderRazorViewToString("~/Views/Consumers/EmailTemplates/EmailToEmployee.cshtml", model);
                var email = model.Email;                
                await EmailService.Send(email, model.ContactName, model.Subject, message, User?.Identity?.Name);
            }
            catch(Exception)
            {
                return Json(new { status = "error" });
            }

            return Json(new { status = "ok" });
        }

        [HttpGet]
        [Authorize]
        public FileResult GetFileHandler(Int32 id)
        {
            ConsumerEmployeeManagement manage = new ConsumerEmployeeManagement(_context);
            var fileData = FileDataService.GetFileMetadata(_context, id);
            
            if (fileData != null)
            {
                var filePath = fileData.FilePath;
                if (System.IO.File.Exists(filePath))
                {                  
                    
                    Response.AddHeader("Content-Disposition", "inline; filename=" + fileData.FileDisplayName);

                    return File(filePath, fileData.FileContentType);
                }
            }
            return null;


        }
    }
}