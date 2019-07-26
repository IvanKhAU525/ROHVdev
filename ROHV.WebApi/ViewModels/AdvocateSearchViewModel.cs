using ROHV.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ROHV.WebApi.ViewModels
{
    public class AdvocateSearchViewModel
    {
        public Int32 AdvocateId { get; set; }
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string City { get; set; }
        public string State { get; set; }

        public AdvocateSearchViewModel(Contact model)
        {
            this.AdvocateId = model.ContactId;          
            this.FirstName = model.FirstName;
            this.LastName = model.LastName;
            this.Email = String.IsNullOrEmpty(model.EmailAddress)?"": model.EmailAddress;
            this.City = String.IsNullOrEmpty(model.City) ? "" : model.City;
            this.State = String.IsNullOrEmpty(model.State) ? "" : model.State;  ;
            this.CompanyName = String.IsNullOrEmpty(model.CompanyName) ? "" : model.CompanyName;   
        }


        static public List<AdvocateSearchViewModel> GetList(List<Contact> models)
        {
            List<AdvocateSearchViewModel> result = new List<AdvocateSearchViewModel>();
            foreach (var item in models)
            {
                result.Add(new AdvocateSearchViewModel(item));
            }
            return result;
        }     
    }
}