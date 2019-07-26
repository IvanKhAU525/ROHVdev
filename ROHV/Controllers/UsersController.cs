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
    public class UsersController : BaseController
    {
        // GET: Consumers
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult> Index()
        {
            UserManagment manager = new UserManagment(_context);
            var roles = await manager.GetSystemRoles();
            ViewData["SystemRoles"] = roles;
            return View();
        }
    }
}