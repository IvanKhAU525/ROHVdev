using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Models.HabPlanReports
{
    [Serializable]
    public class ReportHabPlanOutcomeValue
    {
        public string ValuedOutcome { get; set; }
        public string CQLPOM { get; set; }
        public string MyGoal { get; set; }
        public string ViewActions { get; set; }
        public bool IsIPOP { get; set; }

    }
}
