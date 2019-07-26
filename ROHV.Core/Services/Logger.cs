using ROHV.Core.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Models.Services
{
    public class Logger
    {
        public static void LogError(Exception ex)
        {
            String  additionalErrorInfo = "";
            if (ex is DbEntityValidationException)
            {
                additionalErrorInfo += "\r\n";
                foreach (var eve in ((DbEntityValidationException)ex).EntityValidationErrors)
                {
                    additionalErrorInfo += String.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        additionalErrorInfo += String.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                    additionalErrorInfo += "\r\n";
                }
              
            }
            Log log = new Log();
            log.Date = DateTime.Now;
            if (ex.InnerException != null)
            {
                log.InnerException = ex.InnerException.ToString();
            }
            log.Message = ex.Message;
            log.Source = ex.Source;
            log.StackTrace = additionalErrorInfo+ex.StackTrace;
            using (RayimContext cont = new RayimContext())
            {
                cont.Logs.Add(log);
                cont.SaveChanges();
            }
        }
        public static void LogError(String str)
        {
            Log log = new Log();
            log.Date = DateTime.Now;
            log.InnerException = null;
            log.Message = "Looged string";
            log.Source = "Hamaspik.WebApi";
            log.StackTrace = str;
            using (RayimContext cont = new RayimContext())
            {
                cont.Logs.Add(log);
                cont.SaveChanges();
            }
        }
    }
}
