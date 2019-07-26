using System;
using ROHV.Controllers;
using System.Web.Mvc;
using ROHV.Core.Consumer;
using System.Threading.Tasks;
using ROHV.WebApi.ViewModels;
using System.Collections.Generic;
using ROHV.Core.Services;

namespace ROHV.WebApi.Controllers
{
    public class ConsumerPhoneApiController : BaseController
    {
        [Authorize]
        [HttpPost]
        public ActionResult Save(ConsumerPhonesViewModel model)
        {
            if (User == null) return null;
            ConsumerPhonesManagement managePhones = new ConsumerPhonesManagement(_context);
            int consumerPhoneId = managePhones.Save(model.GetModel());

            return Json(new { status = "ok", consumerPhoneId = consumerPhoneId }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Delete(int phoneId)
        {
            if (User == null) return null;
            ConsumerPhonesManagement managePhones = new ConsumerPhonesManagement(_context);
            await managePhones.Delete(phoneId);

            return Json(new { status = "ok", phoneId = phoneId }, JsonRequestBehavior.AllowGet);
        }
    }
}