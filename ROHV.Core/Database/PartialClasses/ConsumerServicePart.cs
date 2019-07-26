using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Database
{
    using System;
    using System.Collections.Generic;

    public partial class ConsumerService
    {
        public virtual SystemUser CreatedByUser { get => SystemUser; set => SystemUser = value; }
        public virtual SystemUser EditedByUser { get => SystemUser1; set => SystemUser1 = value; }
    }
}
