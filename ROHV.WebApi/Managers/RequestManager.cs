using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;

namespace ROHV.WebApi.Managers
{
    public static class RequestManager
    {
        public static T ConvertPostDataToModel<T>(NameValueCollection postData) where T : new()
        {
            T result = new T();
            var t = typeof(T);
            var propertiesData = t.GetProperties().Where(x => x.CanRead && x.CanWrite);
            foreach (var prop in propertiesData)
            {
                var value = postData[prop.Name];

                object safeValue = null;

                if (!String.IsNullOrEmpty(value))
                {
                    Type eachType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                    safeValue = eachType.IsEnum ? Enum.Parse(eachType, value) : Convert.ChangeType(value, eachType);
                }

                prop.SetValue(result, safeValue);
            }

            return result;
        }
        public static T GetModelFromJsonRequest<T>(HttpRequestBase request)
        {
            string result = "";
            using (Stream req = request.InputStream)
            {
                req.Seek(0, System.IO.SeekOrigin.Begin);
                result = new StreamReader(req).ReadToEnd();
            }
            return JsonConvert.DeserializeObject<T>(result);
        }

    }
}