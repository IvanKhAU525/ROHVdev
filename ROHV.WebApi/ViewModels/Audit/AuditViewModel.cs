using ITCraftFrame;
using ROHV.Core.Models;
using ROHV.Core.Models.Audits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ROHV.WebApi.ViewModels
{
    public class AuditViewModel : BaseAuditModel
    {
        public string ServiceName { get => ServicesList.ServiceDescription; }
        public ServicesListModel ServicesList { get; set; }
        public int NumberOfAuditRecords { set; get; }
        public List<ConsumerAuditViewModel> Consumers { get; set; }
        public AuditViewModel(AuditModel audit)
        {
            CustomMapper.MapEntity(audit, this);
            Consumers = audit.Consumers.Select(x => new ConsumerAuditViewModel()
            {
                ConsumerId = x.ConsumerId.Value,
                ConsumerFirstName = x.FirstName,
                ConsumerLastName = x.LastName,
                AuditId = audit.Id

            }).ToList();
        }
        public AuditViewModel()
        {

        }

    }
}