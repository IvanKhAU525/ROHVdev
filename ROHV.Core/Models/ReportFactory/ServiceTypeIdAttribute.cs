
using ITCraftFrame.BoundData;
using ROHV.Core.Enums;
using System;

namespace ROHV.Core
{
    class ServiceTypeIdAttribute : Attribute, INameAtribute
    {
        public string Name { get; set; }

        public ServiceTypeIdAttribute(ServiceTypeIdEnum typeId)
        {
            this.Name = typeId.ToString();
        }
    }
}
