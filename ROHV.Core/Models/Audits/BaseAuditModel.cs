using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Models.Audits
{
    public class BaseAuditModel
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public DateTime? AuditDate { get; set; }
    }
}
