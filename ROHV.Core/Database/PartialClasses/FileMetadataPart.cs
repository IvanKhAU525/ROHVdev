using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Database
{
    using System;
    using System.Collections.Generic;

    public partial class FileMetaData
    {
        public virtual SystemUser AddedByUser { get => SystemUser; set => SystemUser = value; }
        public virtual SystemUser UpdatedByUser { get => SystemUser1; set => SystemUser1 = value; }

    }
}
