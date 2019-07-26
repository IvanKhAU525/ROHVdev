using ROHV.Core.Consumer;
using ROHV.Core.Database;
using ROHV.WebApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ROHV.Controllers
{
  
    public class ConsumerAuditController : BaseController
    {
        public async Task<ActionResult> Index()
        {
            ConsumerManagement cm = new ConsumerManagement(_context);
            var fullServicesData = await cm.GetServiceList();
            ViewData["FullDataServicesList"] = ItemListViewModel.GetClientList<ServicesList, ServicesListViewModel>(fullServicesData);
            return View();
        }
    }
}