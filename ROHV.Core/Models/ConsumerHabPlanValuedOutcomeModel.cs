using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Models
{
    public class ConsumerHabPlanValuedOutcomeModel
    {
        public Int32 Id { get; set; }
        public String ValuedOutcome { get; set; }
        public String CQLPOM { get; set; }
        public String MyGoal { get; set; }
        public Boolean IsIPOP { set; get; }

    }
}
