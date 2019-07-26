using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Models
{
    public class UserModel
    {
        public Int32 UserId { get; set; }
        public String AspNetUserId { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Email { get; set; }            
        public Boolean IsDeleted { get; set; }
        public Boolean CanManageServices { get; set; }
        public String EmailPassword { get; set; }
    }
}
