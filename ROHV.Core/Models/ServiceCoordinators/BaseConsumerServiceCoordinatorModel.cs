using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Models
{
    public class BaseConsumerServiceCoordinatorModel
    {
        public int Id { get; set; }
        public int ConsumerId { get; set; }
        public int ContactId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }      
    }
}
