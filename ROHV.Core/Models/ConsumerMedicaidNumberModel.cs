using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Models
{
    public class ConsumerMedicaidNumberModel
    {
        public int Id { get; set; }
        public int ConsumerId { get; set; }
        public string MedicaidNo { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
