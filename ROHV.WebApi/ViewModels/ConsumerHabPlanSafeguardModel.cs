using ITCraftFrame;
using ROHV.Core.Database;
using ROHV.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ROHV.WebApi.ViewModels
{
    public class ConsumerHabPlanSafeguardViewModel: ConsumerHabPlanSafeguardModel
    { 

        public ConsumerHabPlanSafeguardViewModel()
        {

        }
        public ConsumerHabPlanSafeguardViewModel(ConsumerHabPlanSafeguard model)
        {
            CustomMapper.MapEntity(model, this);
            this.Id = model.ConsumerHabPlanSafeguardId;
        }
        static public List<ConsumerHabPlanSafeguardViewModel> GetList(List<ConsumerHabPlanSafeguard> models)
        {
            List<ConsumerHabPlanSafeguardViewModel> result = new List<ConsumerHabPlanSafeguardViewModel>();
            foreach (var item in models)
            {
                result.Add(new ConsumerHabPlanSafeguardViewModel(item));
            }
            return result;

        }
    }
}