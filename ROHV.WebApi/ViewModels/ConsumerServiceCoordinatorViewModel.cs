using ROHV.Core;
using ROHV.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ROHV.WebApi.ViewModels
{
    public class ConsumerServiceCoordinatorViewModel: ConsumerServiceCoordinatorModel
    {
        public string ViewContactName { get => $"{ Contact?.FirstName},{Contact?.LastName}"; }
       
    }
}