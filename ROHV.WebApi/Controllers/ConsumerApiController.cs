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
using ROHV.Core.Models;

namespace ROHV.WebApi.Controllers
{
    [Authorize]
    public class ConsumerApiController : BaseController
    {

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> SearchingEmployee(String q, bool? skipNotAssigned)
        {
            if (User == null) return null;

            ConsumerManagement manage = new ConsumerManagement(_context);
            var employees = await manage.GetEmployees(q, skipNotAssigned);
            List<EmployeeSearchViewModel> searchModel = EmployeeSearchViewModel.GetList(employees);

            return Json(new { data = searchModel }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> SearchingEmployeeByConsumer(String q, Int32 consumerId)
        {
            if (User == null) return null;

            ConsumerManagement manage = new ConsumerManagement(_context);
            var employees = await manage.GetEmployeesByConsumer(q, consumerId);
            List<EmployeeSearchViewModel> searchModel = EmployeeSearchViewModel.GetList(employees);

            return Json(new { data = searchModel }, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        [HttpPost]
        public async Task<ActionResult> SearchingConsumers(String q, Int32? EmployeeId, Int32? ConsumerId)
        {
            if (User == null) return null;

            ConsumerManagement manage = new ConsumerManagement(_context);
            var consumers = await manage.GetConsumers(q, EmployeeId, ConsumerId);
            List<ConsumerSearchViewModel> searchModel = ConsumerSearchViewModel.GetList(consumers);

            return Json(new { data = searchModel }, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        [HttpPost]
        public async Task<ActionResult> SearchingMedicaid(String q)
        {
            if (User == null) return null;

            ConsumerManagement manage = new ConsumerManagement(_context);
            var consumers = await manage.GetConsumersByMedicaid(q);
            List<ConsumerSearchViewModel> searchModel = ConsumerSearchViewModel.GetList(consumers);
            return Json(new { data = searchModel }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> SearchingTabsId(String q)
        {
            if (User == null) return null;

            ConsumerManagement manage = new ConsumerManagement(_context);
            var consumers = await manage.GetConsumersByTabsId(q);
            List<ConsumerSearchViewModel> searchModel = ConsumerSearchViewModel.GetList(consumers);
            return Json(new { data = searchModel }, JsonRequestBehavior.AllowGet);
        }

        


        [Authorize]
        [HttpPost]
        public async Task<ActionResult> ServiceCoordinatorslist(String q, Int32? agencyId)
        {
            if (User == null) return null;

            ConsumerManagement manage = new ConsumerManagement(_context);
            var contacts = await manage.GetServiceCoordinatorList(q, agencyId);
            List<EmployeeSearchViewModel> searchModel = EmployeeSearchViewModel.GetList(contacts);
            return Json(new { data = searchModel }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> SearchServiceCoordinators(String q)
        {
            if (User == null) return null;

            ConsumerManagement manage = new ConsumerManagement(_context);
            var searchResult = await manage.GetServiceCoordinators(q);
            var searchModel = ServiceCoordinatorSearchViewModel.GetList(searchResult);
            return Json(new { data = searchModel }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Advocateslist(String q)
        {
            if (User == null) return null;

            ConsumerManagement manage = new ConsumerManagement(_context);
            var advocates = await manage.GetAdvocatesList(q);
            List<AdvocateSearchViewModel> searchModel = AdvocateSearchViewModel.GetList(advocates);
            return Json(new { data = searchModel }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Get(String id)
        {
            if (User == null) return null;

            if (String.IsNullOrEmpty(id)) return null;
            Int32 consumerId = Int32.Parse(id);
            ConsumerManagement manage = new ConsumerManagement(_context);
            ConsumerPhonesManagement managePhones = new ConsumerPhonesManagement(_context);
            ConsumerEmployeeManagement manageEmployees = new ConsumerEmployeeManagement(_context);
            ConsumerServicesManagement manageServices = new ConsumerServicesManagement(_context);
            ConsumerEmployeeDocumentsManagement manageDocuments = new ConsumerEmployeeDocumentsManagement(_context);
            ConsumerHabPlansManagement manageHabPlans = new ConsumerHabPlansManagement(_context);
            ConsumerHabReviewsManagement manageHabReviews = new ConsumerHabReviewsManagement(_context);
            ConsumerPrintDocumentsManagement managePrintDocuments = new ConsumerPrintDocumentsManagement(_context);
            ConsumerCallLogsManagement manageCallLogs = new ConsumerCallLogsManagement(_context);
            ConsumerNotesManagement manageNotes = new ConsumerNotesManagement(_context);
            ConsumerNotificationsManagement manageNotifications = new ConsumerNotificationsManagement(_context);

            var consumer = await manage.GetConsumer(consumerId);
            if (consumer == null) return null;
            var phones = await managePhones.GetPhones(consumerId);
            var employees = await manageEmployees.GetEmployees(consumerId);
            var services = await manageServices.GetServices(consumerId);
            var documents = await manageDocuments.GetDocuments(consumerId);
            var habPlans = await manageHabPlans.GetHabPlans(consumerId);
            var habReviews = await manageHabReviews.GetHabReviews(consumerId);
            var printDocuments = await managePrintDocuments.GetPrintDocuments(consumerId);
            var callLogs = await manageCallLogs.GetCallLogs(consumerId);
            var notes = await manageNotes.GetNotes(consumerId);
            var notifications = await manageNotifications.GetNotifications(consumerId);
            var uploadFiles = FileDataService.GetConsumerFiles(_context, consumerId);
            ConsumerFullViewModel model = new ConsumerFullViewModel(consumer);
            model.SetPhones(phones);
            model.SetEmployees(employees);
            model.SetApprovedServices(services);
            model.SetDocuments(documents);
            model.SetHabPlans(habPlans);
            model.SetHabReviews(habReviews);
            model.SetPrintDocuments(printDocuments);
            model.SetCallLogs(callLogs);
            model.SetNotes(notes);
            model.SetNotifications(notifications);
            model.SetUploadFiles(uploadFiles);
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> SaveRate(ConsumerFullViewModel model)
        {
            if (User == null) return null;

            ConsumerManagement manage = new ConsumerManagement(_context);
            if (!await manage.SaveRate(model.GetRateData()))
            {
                return Json(new { status = "error", message = "Something is going wrong. Please contact to support team." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Save(ConsumerFullViewModel model)
        {
            if (User == null) return null;
            ConsumerManagement manage = new ConsumerManagement(_context);
            ConsumerPhonesManagement managePhones = new ConsumerPhonesManagement(_context);
            Int32 id = 0;
            var dbModel = model.GetModel();
            List<string> errors = await manage.ValidateConsumerModel(dbModel);
            if (!errors.Any())
            {
                id = await manage.Save(dbModel);                
                await managePhones.Save(model.GetPhonesModel(), id);
                return Json(new { status = "ok", id = id }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = "error", message = String.Join(",", errors) }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<ActionResult> Delete(Int32 id)
        {
            if (User == null) return null;
            ConsumerManagement manage = new ConsumerManagement(_context);
            await manage.Delete(id);
            return Json(new { status = "ok" });
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CheckPassword(Int32 contactId, String password)
        {
            ConsumerManagement manage = new ConsumerManagement(_context);
            bool isValid = await manage.IsValidSignaturePassword(contactId, password);
            if (isValid)
            {
                return Json(new { status = "ok" });
            }
            return Json(new { status = "error" });
        }
    }
}