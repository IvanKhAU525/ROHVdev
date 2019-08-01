using System;
using ITCraftFrame;
using ITCraftFrame.BoundData;

namespace ROHV.EmailServiceCore.Attributes
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