using ROHV.Core.Employees;
using ROHV.Core.User;
using ROHV.WebApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ROHV.Controllers
{
    public class EmployeesController : BaseController
    {
        // GET: Consumers
        public async Task<ActionResult> Index()
        {
            EmployeeManagment manager = new EmployeeManagment(_context);
            var lists = new Dictionary<String, List<ItemListViewModel>>();
            var states = await manager.GetStates();
            var types = await manager.GetTypes();
            var depts = await manager.GetDepts();
            var categories = await manager.GetCategories();
            


            lists.Add("States", ItemListViewModel.GetStateList(states));
            lists.Add("Types", ItemListViewModel.GetTypesList(types));
            lists.Add("Depts", ItemListViewModel.GetDeptsList(depts));
            lists.Add("Categories", ItemListViewModel.GetCategories(categories));
            ViewData["Lists"] = lists;
            return View();
        }
    }
}