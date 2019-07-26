using ROHV.Core.Models.Audits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ROHV.WebApi.ViewModels
{
    public class ConsumerAuditViewModel: ConsumerAuditsModel
    {
        public string ConsumerFirstName { set; get;}
        public string ConsumerLastName { set; get; }       
    }
}