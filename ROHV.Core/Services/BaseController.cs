using ROHV.Core.Database;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ROHV.Models;
using ROHV.Core.User;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using ROHV.ViewModels;
using ROHV.Core.Consumer;

namespace ROHV.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public RayimContext _context;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _context = new RayimContext();
            ApplicationUser = new ApplicationUser();
            if (this.User != null)
            {
                if (this.User.Identity.IsAuthenticated)
                {
                    ViewBag.UserName = this.User.Identity.Name;
                    Task<ApplicationUser> user = UserManager.FindByEmailAsync(ViewBag.UserName);
                    user.Wait();
                    ApplicationUser = user.Result;
                    if (_context == null)
                    {
                        _context = new RayimContext();
                    }
                    UserManagment userManagment = new UserManagment(_context);
                    ConsumerManagement cm = new ConsumerManagement(_context);

                    SystemUser userModel = userManagment.GetUserSync(ApplicationUser.Id);
                    var contact = cm.GetContactByEmail(User?.Identity?.Name ?? "");
                   
                    CurrentUserModelView modelView = new CurrentUserModelView(userModel);
                    ViewData["CurrentUser"] = modelView;
                    var contactData = new ContactViewModel(contact ?? new Core.Models.ContactModel());
                    ViewData["loggedContact"] = JsonConvert.SerializeObject(contactData);
                }
            }
            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
                if (_context != null)
                    _context.Dispose();
                _context = null;
            }
            base.Dispose(disposing);
        }

        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        #region Helpers
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUser ApplicationUser { get; private set; }

        private SystemUser _currentUser;
        public SystemUser CurrentUser
        {
            get
            {
                return _currentUser ?? getCurrentUser();
            }
            private set
            {
                _currentUser = value;
            }
        }

        private SystemUser getCurrentUser()
        {
            UserManagment userManager = new UserManagment(_context);
            var currentUser = UserManager.FindByEmailAsync(User?.Identity?.Name).Result;
            var systemUser = userManager.GetUser(currentUser.Id).Result;
            return systemUser;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        #endregion


    }
}