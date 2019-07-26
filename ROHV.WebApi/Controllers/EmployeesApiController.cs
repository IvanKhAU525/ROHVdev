using System;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Linq;
using ROHV.Controllers;
using ROHV.Core.User;
using ROHV.WebApi.ViewModels;
using ROHV.Models;
using ROHV.Core.Employees;
using ROHV.Core.DatatableUtils;
using ROHV.ViewModels;

namespace ROHV.WebApi.Controllers
{
    [Authorize]
    public class EmployeesApiController : BaseController
    {  

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> GetEmployees(DTParameters param)
        {
            if (User == null) return null;
            EmployeeManagment manage = new EmployeeManagment(_context);
            var result = await manage.GetList(param);            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Get(Int32 id)
        {
            if (User == null) return null;

            EmployeeManagment manage = new EmployeeManagment(_context);
            var contact = await manage.GetContact(id);
            var result = new ContactViewModel(contact);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> SaveEmployee(ContactViewModel model)
        {
            if (User == null) return null;

            EmployeeManagment manage = new EmployeeManagment(_context);
            if (await manage.IsExistWithTheSameEmail(model.ContactId, model.EmailAddress))
            {
                return Json(new { status = "error", message = "A contact already exists for this email address. The account hasn't created." }, JsonRequestBehavior.AllowGet);
            }
            if (!model.IsUpdate)
            {                
                model.ContactId = 0;
                var dbModel = model.GetModel();
                model.ContactId = await manage.SaveContact(dbModel);                
            }
            else
            {                
                var dbModel = model.GetModel();
                model.ContactId = await manage.SaveContact(dbModel);
            }

            return Json(new { status = "ok", id = model.ContactId}, JsonRequestBehavior.AllowGet);
        }
       
        [Authorize]
        [HttpDelete]
        public async Task<ActionResult> DeleteEmployee(Int32 id)
        {
            if (User == null) return null;            
            
            EmployeeManagment manage = new EmployeeManagment(_context);
            if (!await manage.DeleteContact(id))
            {
                return Json(new { status = "error", message = "You can't delete this contact. Please contact to support team." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        [Authorize]
        public ActionResult GetPdfEmployeeLabel(Int32 contactId)
        {

            if (User == null) return null;
            EmployeeManagment manage = new EmployeeManagment(_context);

            foreach (String key in Session.Keys.Cast<String>().Where(x => x.StartsWith("DocumentLabelPDF_")).ToArray())
            {
                HttpContext.Session.Remove(key);
            }
            String name = "";
            Byte[] bytes = manage.GetPDF(contactId, this, out name);
            Guid guid = Guid.NewGuid();
            Session["DocumentLabelPDF_" + guid] = bytes;
            Session["DocumentLabelName_" + guid] = name;
            String rootUrl = new Uri(Request.Url, Url.Content("~")).ToString();
            String url = rootUrl + "api/employeesapi/getpdfhandler/" + guid;
            return Json(new { status = "ok", url = url });
        }
        [HttpGet]
        [Authorize]
        public FileResult GetPdfHandler(String id)
        {
            String key = "DocumentLabelPDF_" + id;
            String keyName = "DocumentLabelName_" + id;
            Byte[] streamBytes = (Byte[])Session[key];
            String name = (String)Session[keyName];
            if (streamBytes == null) return null;
            HttpContext.Session.Remove(key);
            Response.AddHeader("Content-Disposition", "inline; filename=" + name + ".pdf");

            return File(streamBytes, "application/pdf");
        }
    }
}