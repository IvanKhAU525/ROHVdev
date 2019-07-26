using ROHV.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ROHV.WebApi.ViewModels
{
    public class ConsumerHabPlanVOServeActionViewModel
    {

        public Int32 Id { get; set; }
        public String ServeAndAction { get; set; }

        public Int32 ValuedOutcomeId { get; set; }
        public ConsumerHabPlanVOServeActionViewModel() { }
        public ConsumerHabPlanVOServeActionViewModel(ConsumerHabPlanVOServeAction model)
        {
            this.Id = model.Id;
            this.ServeAndAction = model.ServeAndAction;
            this.ValuedOutcomeId = model.HabPlanValuedOutcomeId;
        }
        static public List<ConsumerHabPlanVOServeActionViewModel> GetList(List<ConsumerHabPlanVOServeAction> models)
        {
            List<ConsumerHabPlanVOServeActionViewModel> result = new List<ConsumerHabPlanVOServeActionViewModel>();
            foreach (var item in models)
            {
                result.Add(new ConsumerHabPlanVOServeActionViewModel(item));
            }
            return result;

        }
    }
}