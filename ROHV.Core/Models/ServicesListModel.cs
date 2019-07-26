using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Models
{
    public class ServicesListModel
    {
        public int ServiceId { get; set; }
        public string ServiceDescription { get; set; }
        public string ProcedureCode { get; set; }
        public int? MinutesInUnit { get; set; }
        public string ServiceType { get; set; }
    }
}
