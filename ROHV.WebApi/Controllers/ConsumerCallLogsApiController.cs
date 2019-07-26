
using System;
using ROHV.Controllers;
using System.Web.Mvc;
using ROHV.Core.Consumer;
using System.Threading.Tasks;
using ROHV.WebApi.ViewModels;
using System.Collections.Generic;
using ROHV.Core.Services;
using System.Linq;

namespace ROHV.WebApi.Controllers
{
    [Authorize]
    public class ConsumerCallLogsApiController : BaseController
    {

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Save(ConsumerCallLogsViewModel model)
        {
            if (User == null) return null;
            ConsumerCallLogsManagement manage = new ConsumerCallLogsManagement(_context);
            Int32 id = 0;
            if (model.ConsumerContactCallId == null)
            {                
                model.DateCreated = DateTime.Now;             
                id = await manage.Save(model.GetModel());
            }
            else
            {
                id = model.ConsumerContactCallId.Value;
                await manage.Save(model.GetModel());                
            
            }            
            return Json(new { status = "ok", id = id }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpDelete]
        public async Task<ActionResult> Delete(Int32 id)
        {
            if (User == null) return null;

            ConsumerCallLogsManagement manage = new ConsumerCallLogsManagement(_context);
            if (!await manage.Delete(id))
            {
                return Json(new { status = "error", message = "You can't delete this record. Please contact to support team." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }

    }
}