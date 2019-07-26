using ITCraftFrame;
using ROHV.Core.Database;
using ROHV.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ROHV.WebApi.ViewModels
{
    public class ConsumerHabPlanValuedOutcomeViewModel: ConsumerHabPlanValuedOutcomeModel
    {    

        public List<ConsumerHabPlanVOServeActionViewModel> ServeActions { get; set; }

        public ConsumerHabPlanValuedOutcomeViewModel() {

            ServeActions = new List<ConsumerHabPlanVOServeActionViewModel>();
        }
        public ConsumerHabPlanValuedOutcomeViewModel(ConsumerHabPlanValuedOutcome model)
        {          
            CustomMapper.MapEntity(model, this);
            if (model.ConsumerHabPlanVOServeActions != null)
            {
                this.ServeActions = ConsumerHabPlanVOServeActionViewModel.GetList(model.ConsumerHabPlanVOServeActions.ToList());
            }else
            {
                this.ServeActions = new List<ConsumerHabPlanVOServeActionViewModel>();
            }            

        }
        static public List<ConsumerHabPlanValuedOutcomeViewModel> GetList(List<ConsumerHabPlanValuedOutcome> models)
        {
            List<ConsumerHabPlanValuedOutcomeViewModel> result = new List<ConsumerHabPlanValuedOutcomeViewModel>();
            foreach (var item in models)
            {
                result.Add(new ConsumerHabPlanValuedOutcomeViewModel(item));
            }
            return result;

        }
    }
}