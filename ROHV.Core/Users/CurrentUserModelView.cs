using ROHV.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ROHV.Core.User
{
    public class CurrentUserModelView
    {
        public Int32 UserId { get; set; }
        public String Name { get; set; }
        public Boolean CanManageServices { get; set; }

        public CurrentUserModelView(SystemUser model)
        {
            if (model != null)
            {
                this.Name = model.LastName + ", " + model.FirstName;
                this.UserId = model.UserId;
                this.CanManageServices = model.CanManageServices;
            }

        }
    }
}