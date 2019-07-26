using ROHV.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ROHV.WebApi.ViewModels
{
    public class EmployeeSearchViewModel
    {
        public Int32 ContactId { get; set; }
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Name {
            get { return LastName + ", " + FirstName; }
        }
        public string Email { get; set; }
        public bool IsHaveSignature { get; set; }        

        public string Position { get; set; }

        public EmployeeSearchViewModel()
        {
        }
        public EmployeeSearchViewModel( Contact model)
        {
            this.ContactId = model.ContactId;
            if (model.AgencyNameId.HasValue)
            {
                this.CompanyName = model.Agency.NameCompany;
            }else
            {
                this.CompanyName = model.CompanyName;
            }
            this.FirstName = model.FirstName;
            this.LastName = model.LastName;
            this.Email = model.EmailAddress;
            this.Position = model.JobTitle;
            if (!string.IsNullOrEmpty(model.Signature))
            {
                this.IsHaveSignature = true;                
            }
        }

        

        static public List<EmployeeSearchViewModel> GetList(List<Contact> models)
        {
            List<EmployeeSearchViewModel> result = new List<EmployeeSearchViewModel>();
            foreach (var item in models)
            {
                result.Add(new EmployeeSearchViewModel(item));
            }
            return result;
        }     
    }
}