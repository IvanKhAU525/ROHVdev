using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITCraftFrame;
using ROHV.Controllers;
using ROHV.Core.Consumer;
using ROHV.Core.Models;
using ROHV.WebApi.ViewModels;

namespace ROHV.WebApi.Controllers
{
    public class ConsumerMedicaidNumberApiController : BaseController
    {
        [Authorize]
        [HttpPost]
        public ActionResult Save(ConsumerMedicaidNumberViewModel model)
        {
            if (User == null) return null;
            object result = null;

            var inputModel = CustomMapper.MapEntity<ConsumerMedicaidNumberModel>(model);

            var validationErrors = ConsumerMedicaidNumberManagement.Validate(_context, inputModel);

            if (!validationErrors.Any())
            {
                var entity = ConsumerMedicaidNumberManagement.CreateOrUpdate(_context, inputModel);
                result = new { status = "ok", model = CustomMapper.MapEntity<ConsumerMedicaidNumberViewModel>(entity), New = model.Id == 0 };
            }
            else
            {
                result = validationErrors.Select(error => new { status = "error", errorMessage = error });
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            ConsumerMedicaidNumberManagement.Delete(_context, id);

            return Json(new { status = "ok", Id = id }, JsonRequestBehavior.AllowGet);
        }
    }
}