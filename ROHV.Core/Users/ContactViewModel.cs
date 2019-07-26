using ITCraftFrame;
using ROHV.Core.Database;
using ROHV.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;

namespace ROHV.ViewModels
{
    public class ContactViewModel : ContactModel
    {
        public string ContactName { get => $"{LastName} , {FirstName}"; }      
        public String ContactType { get; set; }        
        public ContactViewModel() { }
        public ContactViewModel(Contact model)
        {
             CustomMapper.MapEntity(model, this);            
            if (model.ContactTypeId.HasValue)
            {
                this.ContactType = model.ContactType.Name;
            }
        }
        public ContactViewModel(ContactModel model)
        {
            CustomMapper.MapEntity(model, this);           
        }
        static public List<ContactViewModel> GetList(List<Contact> models)
        {
            List<ContactViewModel> result = new List<ContactViewModel>();
            foreach (var item in models)
            {
                result.Add(new ContactViewModel(item));
            }
            return result;

        }

        public Contact GetModel()
        {
            Contact model = new Contact();
            CustomMapper.MapEntity(this,model);
            return model;
        }
    }
}