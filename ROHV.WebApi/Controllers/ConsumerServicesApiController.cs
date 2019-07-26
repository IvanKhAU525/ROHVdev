
using System;
using ROHV.Controllers;
using System.Web.Mvc;
using ROHV.Core.Consumer;
using System.Threading.Tasks;
using ROHV.WebApi.ViewModels;
using System.Collections.Generic;
using ROHV.Core.Services;
using System.Linq;
using ROHV.Core.User;
using ITCraftFrame;
using ROHV.Core.Database;
using ROHV.Core.Models;

namespace ROHV.WebApi.Controllers
{
    [Authorize]
    public class ConsumerServicesApiController : BaseController
    {

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Save(ConsumerServiceViewModel model)
        {
            if (User == null) return null;
            ConsumerServicesManagement manage = new ConsumerServicesManagement(_context);

            if (model.ConsumerServiceId == null)
            {
                model.CreatedByUserId = CurrentUser?.UserId;
            }
            else
            {

                model.EditedByUserId = CurrentUser?.UserId;
            }
            var dbData = await manage.Save(model.GetModel());

            ConsumerServiceViewModel savedData = CustomMapper.MapEntity<ConsumerServiceViewModel>(dbData);

            if (!string.IsNullOrEmpty(model.FileData))
            {
                var savedFileInfo = manage.SaveConsumerEmployeeFile(CurrentUser, dbData.ConsumerServiceId, model.FileData, model.FileName, model.FileId);
                model.FileId = savedFileInfo.fileId;
                model.FileName = savedFileInfo.fileName;
            }

            var returnObj = new { status = "ok", id = savedData.ConsumerServiceId, savedData.AddedByView, savedData.EditedByView, fileId = model.FileId, fileName = model.FileName };
            return Json(returnObj, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> GetTotalHours(ConsumerServiceViewModel model)
        {
            if (User == null) return null;
            ConsumerServicesManagement manage = new ConsumerServicesManagement(_context);


            model.UsedHours = await manage.GetTotalHours(model.GetModel());

            var returnObj = new { status = "ok", model};
            return Json(returnObj, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        [HttpDelete]
        public async Task<ActionResult> Delete(Int32 serviceId)
        {
            if (User == null) return null;

            ConsumerServicesManagement manage = new ConsumerServicesManagement(_context);
            if (!await manage.Delete(serviceId))
            {
                return Json(new { status = "error", message = "You can't delete this record. Please contact to support team." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> SendEmail(int serviceId, String email, string emailBody, String contactName)
        {
            if (User == null) return null;
            ConsumerServicesManagement manage = new ConsumerServicesManagement(_context);
            var service = await manage.GetService(serviceId,true);
            var mappedData = CustomMapper.MapEntity<ConsumerServiceModel, ApprovedServiceBoundModel>(service);
            mappedData.ConsumerEmployeeList = CustomMapper.MapList<ConsumerEmployeeModel, ConsumerEmployeeModel>(service.ConsumerEmployeeList);
            mappedData.InnerEmailBody = emailBody;
            List<Object> emailInputData = new List<object>() { mappedData };
            await EmailService.SendBoundEmail(email, contactName, "Approver Service Email", "apporved-service-email", emailInputData, User?.Identity?.Name);

            return Json(new { status = "ok" });
        }
    }
}