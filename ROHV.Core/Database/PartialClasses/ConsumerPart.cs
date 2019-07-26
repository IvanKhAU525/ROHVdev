using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Database
{
    public partial class Consumer
    {
        //CHCoordinatorId
        public virtual Contact CHCoordinator { get => Contact; set => Contact = value; }
        //DHCoordinatorId
        public virtual Contact DHCoordinator { get => Contact1; set => Contact1 = value; }
        //MSCId
        public virtual Contact MSCoordinator { get => Contact2; set => Contact2 = value; }
        //ServiceCoordinatorId
        public virtual Contact ServiceCoordinatorContact { get => Contact3; set => Contact3 = value; }
        //AdvocateId
        public virtual Contact Advocate { get => Contact4; set => Contact4 = value; }
        //AdvocatePaperId
        public virtual Contact AdvocatePaper { get => Contact11; set => Contact11 = value; }
    }
}
