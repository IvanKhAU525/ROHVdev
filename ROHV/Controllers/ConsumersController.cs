using Newtonsoft.Json;
using ROHV.Core.Consumer;
using ROHV.Core.Database;
using ROHV.WebApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace ROHV.Controllers
{
    public class ConsumersController : BaseController
    {
        // GET: Consumers
        public async Task<ActionResult> Index()
        {
            ConsumerManagement cm = new ConsumerManagement(_context);
            ConsumerEmployeeDocumentsManagement docs = new ConsumerEmployeeDocumentsManagement(_context);
            ConsumerHabPlansManagement habPlans = new ConsumerHabPlansManagement(_context);
            ConsumerPrintDocumentsManagement printDocuments = new ConsumerPrintDocumentsManagement(_context);
            ConsumerNotesManagement notes = new ConsumerNotesManagement(_context);
            ConsumerNotificationsManagement notifications = new ConsumerNotificationsManagement(_context);

            var lists = new Dictionary<String, List<ItemListViewModel>>();
            var dayProgram = ItemListViewModel.GetList(await cm.GetList("Day Programs"));
            var diagnosis = ItemListViewModel.GetList(await cm.GetList("Diagnosis"));
            var status = ItemListViewModel.GetList(await cm.GetList("Status"));
            var agencyName = ItemListViewModel.GetAgencyNameList(await cm.GetAgencyNamesList());
            var phoneTypes = ItemListViewModel.GetList(await cm.GetList("Phone Types"));
            var unitQuantities = ItemListViewModel.GetList(await cm.GetList("Unit Quantities"));
            var fullServicesData = await cm.GetServiceList();
            var services = ItemListViewModel.GetList(fullServicesData);

            var docStatuses = ItemListViewModel.GetList(await docs.GetDocStatuses());
            var docTypes = await docs.GetDocTypes();

            var habPlanStatuses = ItemListViewModel.GetList(await habPlans.GetStatuses());
            var habPlanDurations = ItemListViewModel.GetList(await habPlans.GetDurations());
            var habPlanFrequencies = ItemListViewModel.GetList(await habPlans.GetFrequencies());

            var printDocumentsTypes = ItemListViewModel.GetList(await printDocuments.GetTypes());

            var noteTypes = ItemListViewModel.GetList(await notes.GetTypes());
            var noteFromTypes = ItemListViewModel.GetList(await notes.GetFromTypes());

            var notificationStatuses = ItemListViewModel.GetList(await notifications.GetStatuses());
            var notificationRepeatingTypes = ItemListViewModel.GetList(await notifications.GetTypes());
            var serviceTypesList = ItemListViewModel.GetList(await printDocuments.GetServiceTypes());
            var serviceTypesExList = ItemListViewModel.GetExList(await printDocuments.GetServiceTypes());

            lists.Add("DayPrograms", dayProgram);
            lists.Add("Diagnosis", diagnosis);
            lists.Add("Status", status);
            lists.Add("AgencyName", agencyName);
            lists.Add("PhoneTypesList", phoneTypes);
            lists.Add("ServicesList", services);
            lists.Add("ServiceTypesList", serviceTypesList);
            lists.Add("ServiceTypesExList", serviceTypesExList);

            lists.Add("UnitQuantitiesList", unitQuantities);
            lists.Add("EmployeeDocumentStatuses", docStatuses);

            lists.Add("HabPlanStatuses", habPlanStatuses);
            lists.Add("HabPlanDurations", habPlanDurations);
            lists.Add("HabPlanFrequencies", habPlanFrequencies);

            lists.Add("PrintDocumentTypes", printDocumentsTypes);
            lists.Add("NoteTypes", noteTypes);
            lists.Add("NoteFromTypes", noteFromTypes);

            lists.Add("NotificationStatusesList", notificationStatuses);
            lists.Add("RepeatingTypesList", notificationRepeatingTypes);
            ViewData["Lists"] = lists;
            ViewData["EmployeeDocumentTypes"] = docTypes;          
            ViewData["QIDP"] = WebConfigurationManager.AppSettings["QIDP"];
            ViewData["FullDataServicesList"] = ItemListViewModel.GetClientList<ServicesList,ServicesListViewModel>(fullServicesData);
            return View();
        }
    }
}