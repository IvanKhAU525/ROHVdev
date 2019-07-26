using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Models
{
    public class ServiceCoordinatorSearchModel
    {
        public Int32 ConsumerId { get; set; }
        public String ConsumerFirstName { get; set; }
        public String ConsumerLastName { get; set; }
        public String ServiceCoordinatorFirstName { get; set; }
        public String ServiceCoordinatorLastName { get; set; }
    }
}
