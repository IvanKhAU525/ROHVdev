using ITCraftFrame;
using ROHV.Core.Consumer;
using ROHV.Core.Database;
using ROHV.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ROHV.WebApi.ViewModels
{
    public class ConsumerEmployeeViewModel : ConsumerEmployeeModel
    {
        public int? FileId { set; get; }
        public string FileData { set; get; }
        public string FileName { get; set; }

        public ConsumerEmployeeViewModel() { }

        public ConsumerEmployeeViewModel(ConsumerEmployee model)
        {
            CustomMapper.MapEntity(model, this);
            var employeeFiles = ConsumerEmployeeManagement.GetFilesByConsumerEmployeeId(model.ConsumerEmployeeId).FirstOrDefault();
            FileId = employeeFiles?.Id;
            FileName = employeeFiles?.FileDisplayName;

            this.ContactName = model.Contact.FirstName + " " + model.Contact.LastName;
            if (this.ServiceId.HasValue)
            {
                this.ServiceName = model.ServicesList.ServiceDescription;
            }
            
            if (model.Contact.AgencyNameId.HasValue)
            {
                this.CompanyName = model.Contact.Agency.NameCompany;
            }
            
        }
        static public List<ConsumerEmployeeViewModel> GetList(List<ConsumerEmployee> models)
        {
            List<ConsumerEmployeeViewModel> result = new List<ConsumerEmployeeViewModel>();
            foreach (var item in models)
            {
                result.Add(new ConsumerEmployeeViewModel(item));
            }
            return result;

        }

        public ConsumerEmployee GetModel()
        {
            ConsumerEmployee model = new ConsumerEmployee();

            CustomMapper.MapEntity(this, model);

            if (this.ConsumerEmployeeId.HasValue)
            {
                model.ConsumerEmployeeId = this.ConsumerEmployeeId.Value;
            }
            else
            {
                model.ConsumerEmployeeId = 0;
            }

            return model;
        }
    }
}