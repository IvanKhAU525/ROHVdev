using ROHV.Core.Database;
using ROHV.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ROHV.WebApi.ViewModels
{
    public class UserViewModel: UserModel
    {
        public String Password { get; set; }       
        public Boolean IsUpdate { get; set; }
        public String Role { get; set; }
        //public String CompanyName { get; set; }
        //public String Phone { get; set; }
        //public String Address1 { get; set; }
        //public String Address2 { get; set; }
        //public String State { get; set; }
        //public String Zip { get; set; }
        //public String NPI { get; set; }
        //public String OtherID { get; set; }
        //public String City { get; set; } 

        public UserViewModel(){}
        public UserViewModel(SystemUser model)
        {
            ITCraftFrame.CustomMapper.MapEntity<UserViewModel>(model,this);            
            var role = model.AspNetUser.AspNetRoles.FirstOrDefault();
            if (role != null)
            {
                this.Role = role.Name;     
            }
        }
        static public List<UserViewModel> GetList(List<SystemUser> models)
        {
            List<UserViewModel> result = new List<UserViewModel>();
            foreach (var item in models)
            {
                result.Add(new UserViewModel(item));
            }
            return result;

        }

        public SystemUser GetModel()
        {
            SystemUser model = ITCraftFrame.CustomMapper.MapEntity<SystemUser>(this);           
            
            return model;
        }
    }
}