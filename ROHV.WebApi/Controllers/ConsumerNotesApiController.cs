
using System;
using ROHV.Controllers;
using System.Web.Mvc;
using ROHV.Core.Consumer;
using System.Threading.Tasks;
using ROHV.WebApi.ViewModels;
using System.Collections.Generic;
using ROHV.Core.Services;
using System.Linq;
using ITCraftFrame;
using ROHV.Core.Database;
using ROHV.Core.Models;
using ROHV.EmailServiceCore;
using ROHV.EmailServiceCore.boundModels;

namespace ROHV.WebApi.Controllers
{
    [Authorize]
    public class ConsumerNotesApiController : BaseController
    {

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Save(ConsumerNotesViewModel model)
        {
            if (User == null) return null;
            ConsumerNotesManagement manage = new ConsumerNotesManagement(_context);
            Int32 id = 0;
            if (model.ConsumerNoteId == null)
            {
                model.DateCreated = DateTime.Now;               
            }            
            id = await manage.Save(model.GetModel());
            return Json(new { status = "ok", id = id }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> SaveSimple(ConsumerNotesViewModel model)
        {
            if (User == null) return null;
            ConsumerNotesManagement manage = new ConsumerNotesManagement(_context);
            Int32 id = 0;
            model.DateCreated = DateTime.Now;            
            id = await manage.Save(model.GetModel());
            return Json(new { status = "ok", id = id }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpDelete]
        public async Task<ActionResult> Delete(Int32 id)
        {
            if (User == null) return null;

            ConsumerNotesManagement manage = new ConsumerNotesManagement(_context);
            if (!await manage.Delete(id))
            {
                return Json(new { status = "error", message = "You can't delete this record. Please contact to support team." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> SendEmail(int noteId, String email, string emailBody, String contactName)
        {
            if (User == null) return null;
            ConsumerNotesManagement manage = new ConsumerNotesManagement(_context);
            var note = await manage.GetNote(noteId);
            var mappedData = CustomMapper.MapEntity<ConsumerNote, CustomerNotesBoundModel>(note);
            mappedData.InnerEmailBody = emailBody;
            List<Object> emailInputData = new List<object>() { mappedData };
            await EmailService.SendBoundEmail(email, contactName, "Notes Email", "note-email", emailInputData, User?.Identity?.Name);

            return Json(new { status = "ok" });
        }

    }
}