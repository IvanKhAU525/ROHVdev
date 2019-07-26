using ROHV.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ROHV.WebApi.ViewModels
{
    public class ConsumerSearchViewModel
    {
        public Int32 ConsumerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }        
        public string City { get; set; }
        public string State { get; set; }
        public string MedicaidNo { get; set; }

        public ConsumerSearchViewModel(Consumer model)
        {
            this.ConsumerId = model.ConsumerId;
            this.City = model.City;
            this.State = model.State;
            this.FirstName = model.FirstName;
            this.LastName = model.LastName;
            this.MedicaidNo = model.MedicaidNo;            
        }

        static public List<ConsumerSearchViewModel> GetList(List<Consumer> models)
        {
            List<ConsumerSearchViewModel> result = new List<ConsumerSearchViewModel>();
            foreach (var item in models)
            {
                result.Add(new ConsumerSearchViewModel(item));
            }
            return result;

        }
    }
}