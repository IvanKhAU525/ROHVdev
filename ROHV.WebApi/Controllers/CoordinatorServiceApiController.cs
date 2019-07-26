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
    public class CoordinatorServiceApiController : BaseController
    {
        [Authorize]
        [HttpPost]
        public ActionResult Save(ConsumerServiceCoordinatorViewModel viewModel)
        {
            if (User == null) return null;
            object result = null;
            var inputModel = CustomMapper.MapEntity<BaseConsumerServiceCoordinatorModel>(viewModel);
            if (ConsumerServiceCoordinatorManagement.Validate(_context, inputModel))
            {
                var entity = ConsumerServiceCoordinatorManagement.CreateOrUpdate(_context, inputModel);
                result = new { status = "ok", model = CustomMapper.MapEntity<ConsumerServiceCoordinatorViewModel>(entity), New = viewModel.Id == 0 };
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
            ConsumerServiceCoordinatorManagement.Delete(_context, Id);

            return Json(new { status = "ok", Id = Id }, JsonRequestBehavior.AllowGet);
        }
    }
}