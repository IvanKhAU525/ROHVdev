using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using ROHV.Core.Database;
using ROHV.Core.Services;
using System.Data.Entity;
using ROHV.Core.Models;
using ROHV.Core.Enums;


namespace ROHV.Core.User
{
    public class UserManagment : BaseModel
    {
        public UserManagment(RayimContext context) : base(context) { }
        public Task<SystemUser> GetUser(String aspNetUserId)
        {
            var systemUser = _context.SystemUsers.SingleOrDefaultAsync(x => x.AspNetUserId == aspNetUserId);
            return systemUser;
        }
        public SystemUser GetUserSync(String aspNetUserId)
        {
            var systemUser = _context.SystemUsers.SingleOrDefault(x => x.AspNetUserId == aspNetUserId);
            return systemUser;
        }
        public static UserModel GetUserByEmail(string userEmail)
        {
            using (var context = new RayimContext())
            {                

                return GetUserByEmail(userEmail, context);
            }
        }
        public static UserModel GetUserByEmail(string userEmail, RayimContext context)
        {
            var userData = context.SystemUsers.Where(x => x.Email == userEmail).FirstOrDefault();
            return ITCraftFrame.CustomMapper.MapEntity<UserModel>(userData);

        }
        public async Task<List<SystemUser>> GetAllUsers()
        {
            var systemUser = await _context.SystemUsers.Where(x => !x.IsDeleted).ToListAsync();
            return systemUser;
        }
        public async Task<SystemUser> GetUser(Int32 id)
        {
            var systemUser = await _context.SystemUsers.SingleOrDefaultAsync(x => x.UserId == id);
            return systemUser;
        }

        public async Task<Boolean> IsExistWithTheSameEmail(Int32 userId, string email)
        {
            if (userId == -1)
            {
                var result = await _context.SystemUsers.SingleOrDefaultAsync(x => x.Email.ToLower() == email.ToLower());
                if (result != null)
                {
                    return true;
                }
            }
            else
            {
                var result = await _context.SystemUsers.SingleOrDefaultAsync(x => x.Email.ToLower() == email.ToLower() && x.UserId != userId);
                if (result != null)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<Int32> SaveUser(SystemUser dbModel)
        {
            if (dbModel.UserId == 0)
            {
                _context.SystemUsers.Add(dbModel);
            }
            else
            {
                var model = await _context.SystemUsers.SingleOrDefaultAsync(x => x.UserId == dbModel.UserId);
                ITCraftFrame.CustomMapper.MapEntity(dbModel, model);
            }
            await _context.SaveChangesAsync();
            return dbModel.UserId;
        }

        public async Task<Boolean> IsActiveUser(String aspNetUserId)
        {
            var user = await _context.SystemUsers.SingleOrDefaultAsync(x => x.AspNetUserId == aspNetUserId);
            if (user == null)
            {
                return false;
            }
            if (user.IsDeleted)
            {
                return false;
            }
            return true;
        }
        public async Task<Boolean> DeleteUser(Int32 id)
        {
            var user = await _context.SystemUsers.SingleOrDefaultAsync(x => x.UserId == id);
            if (user == null)
            {
                return false;
            }
            if (user.IsRoles(RolesEnum.Admin))
            {
                return false;
            }
            user.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<RoleModel>> GetSystemRoles()
        {
            var roles = await _context.AspNetRoles.ToListAsync();

            return ITCraftFrame.CustomMapper.MapList<RoleModel, AspNetRole>(roles);
        }

        public async Task<IEnumerable<ConsumerNotificationSettingModel>> GetScheduledNotificationsAsync(DateTime dateStart) {
            var consumerNotificationSettings = await _context.ConsumerNotificationSettings
                .Include(x => x.ConsumerNotificationRecipients)
                .Where(x => x.DateStart == dateStart)
                .ToListAsync();
            return ITCraftFrame.CustomMapper.MapList<ConsumerNotificationSettingModel, ConsumerNotificationSetting>(consumerNotificationSettings);
        }

        public async Task<IEnumerable<ConsumerNotificationSettingModel>> GetScheduledNotificationsAsync() {
            var consumerNotificationSettings = await _context.ConsumerNotificationSettings
                .Where(x => x.StatusId == (int) NotificationStatuses.Working)
                .ToListAsync();
            return ITCraftFrame.CustomMapper.MapList<ConsumerNotificationSettingModel, ConsumerNotificationSetting>(consumerNotificationSettings);
        }

        public async Task<IEnumerable<DateTime>> GetScheduledNotificationsOnlyDatesAsync() {
            var notificationDates = await _context.ConsumerNotificationSettings
                .Where(x => x.StatusId == (int) NotificationStatuses.Working)
                .Select(x => x.DateStart)
                .ToListAsync();
            return notificationDates;
        }
    }
}
