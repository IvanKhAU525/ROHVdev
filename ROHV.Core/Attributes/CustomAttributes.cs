using ITCraftFrame;
using ITCraftFrame.BoundData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Attributes
{
    public class EmailBoundAttribute : Attribute, IDataBindAtribute
    {
        public String Name { get; set; }
        public BoundTypes BoundDataType { get; set; }
        public string[] Aliases { set; get; }
        public float MaxFontSize { set; get; }
        public bool AdjustFontSize { get; set; }

    }
}
