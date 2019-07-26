using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ROHV.Core.Models;

namespace ROHV.WebApi.ViewModels
{
    public class ServiceCoordinatorSearchViewModel
    {
        public Int32 ConsumerId { get; set; }
        public String ConsumerFirstName { get; set; }
        public String ConsumerLastName { get; set; }
        public String ServiceCoordinatorFirstName { get; set; }
        public String ServiceCoordinatorLastName { get; set; }

        public ServiceCoordinatorSearchViewModel(ServiceCoordinatorSearchModel model)
        {
            ConsumerId = model.ConsumerId;
            ConsumerFirstName = model.ConsumerFirstName;
            ConsumerLastName = model.ConsumerLastName;
            ServiceCoordinatorFirstName = model.ServiceCoordinatorFirstName;
            ServiceCoordinatorLastName = model.ServiceCoordinatorLastName;
        }

        public static IEnumerable<ServiceCoordinatorSearchViewModel> GetList(
            IEnumerable<ServiceCoordinatorSearchModel> models)
        {
            return models.Select(x => new ServiceCoordinatorSearchViewModel(x));
        }
    }
}