using ROHV.Core.Database;
using ROHV.Core.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core
{
    public static class DateTimeExtensions
    {
        public static string ToStrDate(this DateTime? inputDate, string format = "d MMMM yyyy")
        {
            return inputDate != null ? inputDate.Value.ToString(format, CultureInfo.CreateSpecificCulture("en-US")) : String.Empty;
        }
    }

    public static class SystemUserExtentions
    {
        public static bool IsRoles(this SystemUser user,RolesEnum role)
        {
            bool result = user != null && user.AspNetUser != null && user.AspNetUser.AspNetRoles.Any(x => x.Name == role.ToString());
            return result;
        }
    }
}
