using ITCraftFrame;
using ROHV.Core.Database;
using ROHV.Core.Models;
using ROHV.Core.Models.Audits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Consumer
{
    public static class AuditManagement
    {
        public static List<AuditModel> GetAudits(RayimContext context, DateTime? auditDate = null)
        {
            var dbAudits = context.Audits.Where(x => auditDate == null || x.AuditDate <= auditDate).ToList();

            var modelAudits = dbAudits.Select(x =>
           {
               var result = CustomMapper.MapEntity<AuditModel>(x);
               result.Consumers = CustomMapper.MapList<ConsumerModel, Database.Consumer>(x.Consumers.ToList());
               return result;
           }).ToList();
            return modelAudits;

        }

        public static bool AddNewAudit(RayimContext context, int numberOfAuditRecords, int serviceId)
        {
            Random random = new Random(DateTime.Now.Millisecond);
            var cunsumersIds = context.ConsumerServices.
                Where(x => x.ServiceId == serviceId).
                Select(x => x.ConsumerId.Value).ToList().Distinct().
                OrderBy(x => random.Next()).
                Take(numberOfAuditRecords).ToList();
            if (cunsumersIds.Any())
            {
                var newAudit = new Audit() { AuditDate = DateTime.Now.Date, ServiceId = serviceId };
                context.Audits.Add(newAudit);
                var consumers = context.Consumers.Where(x => cunsumersIds.Contains(x.ConsumerId)).ToList();
                consumers.ForEach(x =>
                {
                    newAudit.Consumers.Add(x);
                });
                context.SaveChanges();
            }

            return cunsumersIds.Any();
        }

        public static bool DeleteAudit(RayimContext context, int id)
        {            
            var audit = context.Audits.FirstOrDefault(x => x.Id == id);
            if (audit != null)
            {
                context.Audits.Remove(audit);
                context.SaveChanges();
            }
            return audit != null;
        }
    }
}
