using System;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Linq;
using ROHV.Controllers;
using ROHV.Core.User;
using ROHV.WebApi.ViewModels;
using ROHV.Models;

namespace ROHV.WebApi.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersApiController : BaseController
    {

        [HttpGet]
        public async Task<ActionResult> GetUsers()
        {
            if (User == null) return null;
            UserManagment manage = new UserManagment(_context);
            var users = await manage.GetAllUsers();
            var result = UserViewModel.GetList(users);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public async Task<ActionResult> Get(Int32 id)
        {
            if (User == null) return null;

            UserManagment manage = new UserManagment(_context);
            var user = await manage.GetUser(id);
            var result = new UserViewModel(user);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public async Task<ActionResult> SaveUser(UserViewModel model)
        {
            if (User == null) return null;

            UserManagment manage = new UserManagment(_context);
            if (await manage.IsExistWithTheSameEmail(model.UserId, model.Email))
            {
                return Json(new { status = "error", message = "An account already exists for this email address. The account hasn't created." }, JsonRequestBehavior.AllowGet);
            }

            var error = !model.IsUpdate ? await AddNewAspNetUser(model) : await UpdateAspNetUser(model);


            if (!string.IsNullOrEmpty(error))
            {
                return Json(new { status = "error", message = error }, JsonRequestBehavior.AllowGet);
            }

            var dbModel = model.GetModel();
            model.UserId = await manage.SaveUser(dbModel);
            return Json(new { status = "ok", id = model.UserId, aspnetid = model.AspNetUserId }, JsonRequestBehavior.AllowGet);
        }

        private async Task<string> UpdateAspNetUser(UserViewModel model)
        {
            var error = String.Empty;
            String userId = model.AspNetUserId;
            var user = await UserManager.FindByIdAsync(userId);
            if (user.Email != model.Email)
            {
                // change username and email
                user.UserName = model.Email;
                user.Email = model.Email;
                await UserManager.UpdateAsync(user);
            }
            if (!user.IsSuperAdmin())
            {
                await reAssignRoles(userId, model.Role);
            }
            else
            {
                var oldUserRoles = await UserManager.GetRolesAsync(user.Id);
                if (!oldUserRoles.Contains(model.Role))
                {
                    error = "This account role can't be reassigned";
                }
            }
            return error;
        }

        private async Task reAssignRoles(string userId, string newRole)
        {
            var hasRole = false;
            var oldUserRoles = await UserManager.GetRolesAsync(userId);
            if (!String.IsNullOrEmpty(newRole) && oldUserRoles.Contains(newRole))
            {
                hasRole = oldUserRoles.Remove(newRole);
            }
            if (oldUserRoles.Any())
            {
                await UserManager.RemoveFromRolesAsync(userId, oldUserRoles.ToArray());
            }
            if (!hasRole && !String.IsNullOrEmpty(newRole))
            {
                await UserManager.AddToRoleAsync(userId, newRole);
            }
        }

        private async Task<string> AddNewAspNetUser(UserViewModel model)
        {
            var error = String.Empty;
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (!String.IsNullOrEmpty(model.Role))
                {
                    await UserManager.AddToRoleAsync(user.Id, model.Role);
                }
                model.AspNetUserId = user.Id;
                model.UserId = 0;
            }
            else
            {
                error = result.Errors.ToList()[0];
            }
            return error;

        }

        [HttpPost]
        public async Task<ActionResult> UpdateProfile(UserViewModel model)
        {
            if (User == null) return null;

            UserManagment manage = new UserManagment(_context);
            var dbModel = model.GetModel();
            await manage.SaveUser(dbModel);

            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }


        [HttpDelete]
        public async Task<ActionResult> DeleteUser(UserViewModel model)
        {
            if (User == null) return null;

            UserManagment manage = new UserManagment(_context);
            if (!await manage.DeleteUser(model.UserId))
            {
                return Json(new { status = "error", message = "You can't delete this user. Please contact to support team." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword(UserViewModel model)
        {
            if (User == null) return null;

            var user = await UserManager.FindByIdAsync(model.AspNetUserId);
            if (user.Email != model.Email)
            {
                // change username and email            
                user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.Password);
                await UserManager.UpdateAsync(user);
            }
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }

    }
}