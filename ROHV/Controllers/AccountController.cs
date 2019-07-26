using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ROHV.Models;
using ROHV.Core.User;

namespace ROHV.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        public AccountController()
        {
        }      
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                var address = Url.RouteUrl("AllPages", new { @Controller = "Home", @Action = "Index" }, this.Request.Url.Scheme);
                return Redirect(address);
            }
         //   ViewBag.pass = this.UserManager.PasswordHasher.HashPassword("Qwerty1!");
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                //var user1 = new ApplicationUser { UserName = model.Email, Email = model.Email };
                //var result1 = await UserManager.CreateAsync(user1, model.Password);

                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return Json(UserErrors.GetErrorObj(UserErrors.ERROR_USER_PASSWOD_INCORECT));
                }                

                var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        var address = Url.RouteUrl("AllPages", new { @Controller = "Home", @Action = "Index" }, this.Request.Url.Scheme);
                        return Json(new { result = "ok", returnUrl = address });
                    case SignInStatus.LockedOut:
                        return Json(UserErrors.GetErrorObj(UserErrors.ERROR_WRONG_DATA));
                    case SignInStatus.RequiresVerification:
                        return Json(UserErrors.GetErrorObj(UserErrors.ERROR_WRONG_DATA));
                    case SignInStatus.Failure:
                    default:
                        return Json(UserErrors.GetErrorObj(UserErrors.ERROR_USER_PASSWOD_INCORECT));
                }
            }
            return Json(UserErrors.GetErrorObj(UserErrors.ERROR_USER_PASSWOD_INCORECT));
        }
        //
        // POST: /Account/LogOff        
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            var address = Url.RouteUrl("Account", new { @Controller = "Account", @Action = "Login" }, this.Request.Url.Scheme);
            return Redirect(address);
        }



     
    }
}