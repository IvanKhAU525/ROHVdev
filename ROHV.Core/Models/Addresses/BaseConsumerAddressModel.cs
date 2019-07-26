using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Models
{
    public class BaseConsumerAddressModel
    {
        public int Id { get; set; }
        public int ConsumerId { get; set; }
        public string Address1 { set; get; }
        public string Address2 { set; get; }
        public string City { set; get; }
        public string State { set; get; }
        public string Zip { set; get; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
