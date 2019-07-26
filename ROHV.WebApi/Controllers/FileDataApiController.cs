
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
using ITCraftFrame;
using ROHV.WebApi.Managers;

namespace ROHV.WebApi.Controllers
{
    [Authorize]
    public class FileDataApiController : BaseController
    {
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

        [Authorize]
        [HttpDelete]
        public ActionResult DeleteFile(int? Id)
        {
            if (Id == null) return null;

            bool result = FileDataService.DeleteFileData(_context, Id.Value);



            if (!result)
            {
                return Json(new { status = "error", message = "You can't delete this file. Please contact to support team." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddOrUpdateConsumerFile()
        {
            FileMetaDataModelView inputModel = RequestManager.GetModelFromJsonRequest<FileMetaDataModelView>(HttpContext.Request);
            if (inputModel == null) return null;
            inputModel.ParentEntityTypeId = (int)EntityTypes.Consumer;
            FileDataService.UpdateFileMetaData(inputModel, CurrentUser.UserId);
            var result = FileDataService.SaveFileDataWithFile(_context, inputModel, inputModel.FileData);
            var returnData = CustomMapper.MapEntity<FileMetaDataModelView>(result);
            return Json(new { status = "ok", model = returnData }, JsonRequestBehavior.AllowGet);
        }
    }
}