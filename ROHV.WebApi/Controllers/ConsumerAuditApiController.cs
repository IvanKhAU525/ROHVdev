using ROHV.Controllers;
using ROHV.Core.Consumer;
using ROHV.WebApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ROHV.WebApi.Controllers
{
    public class ConsumerAuditApiController : BaseController
    {
        [Authorize]
        [HttpGet]        
        public ActionResult GetAudits(DateTime?  auditDate)
        {
            if (User == null) return null;
            var audits = AuditManagement.GetAudits(_context,auditDate);
            var viewResult = audits.Select(x => new AuditViewModel(x)).ToList();
            return Json(viewResult, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddNewAudit(AuditViewModel model)
        {
            if (User == null) return null;

            var addedCustomers = AuditManagement.AddNewAudit(_context, model.NumberOfAuditRecords, model.ServiceId);
            if (addedCustomers)
            {
                return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "error", message = "No customers found" }, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize]
        [HttpDelete]
        public ActionResult Delete(int Id)
        {
            if (User == null) return null;

            var isDeleted = AuditManagement.DeleteAudit(_context,Id);
            if (isDeleted)
            {
                return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "error", message = "Records is not Deleted" }, JsonRequestBehavior.AllowGet);
            }
        }
        

    }
}
