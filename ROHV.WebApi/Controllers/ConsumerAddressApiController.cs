using ITCraftFrame;
using ROHV.Controllers;
using ROHV.Core.Consumer;
using ROHV.Core.Models;
using ROHV.WebApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ROHV.WebApi.Controllers
{
    public class ConsumerAddressApiController : BaseController
    {
        [Authorize]
        [HttpPost]
        public ActionResult Save(ConsumerAddressModelView viewModel)
        {
            if (User == null) return null;
            object result = null;
            var inputModel = CustomMapper.MapEntity<BaseConsumerAddressModel>(viewModel);
            if (ConsumerAddressManagement.Validate(_context, inputModel))
            {
                var entity = ConsumerAddressManagement.CreateOrUpdate(_context, inputModel);
                result = new { status = "ok", model = CustomMapper.MapEntity<ConsumerAddressModelView>(entity), New = viewModel.Id == 0 };
            }
            else
            {
                result = new { status = "error", errorMessage = "not valid date range for MSC"};               
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Delete(int Id)
        {
            ConsumerAddressManagement.Delete(_context, Id);

            return Json(new { status = "ok", Id = Id }, JsonRequestBehavior.AllowGet);
        }
    }
}