using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ReportingServices.DataProcessing;
using System.Globalization;
using System.Security.Principal;

namespace ROHV.Core
{
    public static class Extentions
    {

        public static string ToDateString(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToString(@"MM\/dd\/yyyy", CultureInfo.InvariantCulture);
            }
            return string.Empty;
        }
        public static string ToDateString2(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToString(@"MMMM yyyy", CultureInfo.InvariantCulture);
            }
            return string.Empty;
        }

        public static T GetSafeDataByIndex<T>(this List<T> inputCollection, int index)
        {
            T result = default(T);

            if (index >= 0 && inputCollection.Count > index)
            {
                result = inputCollection[index];
            }
            
            return result;
        }
    }
}
