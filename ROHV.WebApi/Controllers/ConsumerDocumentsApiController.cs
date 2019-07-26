

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

namespace ROHV.WebApi.Controllers
{
    [Authorize]
    public class ConsumerDocumentsApiController : BaseController
    {

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Save()
        {

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            string data = new System.IO.StreamReader(Request.InputStream).ReadToEnd();
            ConsumerDocumentViewModel model = serializer.Deserialize<ConsumerDocumentViewModel>(data);

            if (User == null) return null;
            ConsumerEmployeeDocumentsManagement manage = new ConsumerEmployeeDocumentsManagement(_context);
            Int32 id = 0;
            if (model.EmployeeDocumentId == null)
            {
                model.DateCreated = DateTime.Now;
                id = await manage.Save(model.GetModel(), model.GetNotesModel(), model.FileData, this);
            }
            else
            {
                id = model.EmployeeDocumentId.Value;
                await manage.Save(model.GetModel(), model.GetNotesModel(), model.FileData, this);

            }
            var document = await manage.GetDocument(id);
            return Json(new { status = "ok", id = id, filePath= document.DocumentPath }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpDelete]
        public async Task<ActionResult> Delete(Int32 id)
        {
            if (User == null) return null;

            ConsumerEmployeeDocumentsManagement manage = new ConsumerEmployeeDocumentsManagement(_context);
            if (!await manage.Delete(id,this))
            {
                return Json(new { status = "error", message = "You can't delete this record. Please contact to support team." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public async Task<FileResult> GetDocumentHandler(Int32 id)
        {
            ConsumerEmployeeDocumentsManagement manage = new ConsumerEmployeeDocumentsManagement(_context);

            var document = await manage.GetDocument(id);
            if (document != null)
            {                
                var filePath = manage.GetDocumentPath(this) + document.DocumentPath;
                if (System.IO.File.Exists(filePath))
                {
                    if(String.IsNullOrEmpty(document.DocumentContentType))
                    {
                        document.DocumentContentType = "application/binary";
                    }
                    var res = document.DocumentPath.Split('.');
                    var fileName = document.Contact.LastName + "_" + document.Contact.FirstName + " (" + document.EmployeeDocumentType.Name+")";
                    if (res.Length > 1)
                    {
                         fileName +="."+res[1];
                    }
                    Response.AddHeader("Content-Disposition", "inline; filename=" + fileName);
                    
                    return File(filePath, document.DocumentContentType);
                }
            }
            return null;


        }

    }
}