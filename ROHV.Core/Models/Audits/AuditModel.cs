using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Models.Audits
{
    public class AuditModel: BaseAuditModel
    {      
        public ServicesListModel ServicesList { get; set; }
        public List<ConsumerModel> Consumers { get; set; }
    }
}
