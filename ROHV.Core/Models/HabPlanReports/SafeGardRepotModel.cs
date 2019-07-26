using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Models.HabPlanReports
{
    [Serializable]
    public class SafeGardRepotModel
    {
        public String Item { get; set; }
        public string Action { get; set; }
        public bool IsIPOP { set; get; }
    }
}
