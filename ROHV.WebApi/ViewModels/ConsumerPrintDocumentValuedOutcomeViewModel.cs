using ROHV.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ROHV.WebApi.ViewModels
{
    public class ConsumerPrintDocumentValuedOutcomeViewModel
    {

        public Int32 Id { get; set; }
        public String ValuedOutcome { get; set; }
        

        public List<ConsumerPrintDocumentVOServeActionViewModel> ServeActions { get; set; }

        public ConsumerPrintDocumentValuedOutcomeViewModel() {

            ServeActions = new List<ConsumerPrintDocumentVOServeActionViewModel>();
        }
        public ConsumerPrintDocumentValuedOutcomeViewModel(ConsumerPrintDocumentValuedOutcome model)
        {

            this.Id = model.Id;
            this.ValuedOutcome = model.ValuedOutcome;
            if (model.ConsumerPrintDocumentVOServeActions != null)
            {
                this.ServeActions = ConsumerPrintDocumentVOServeActionViewModel.GetList(model.ConsumerPrintDocumentVOServeActions.ToList());
            }else
            {
                this.ServeActions = new List<ConsumerPrintDocumentVOServeActionViewModel>();
            }            

        }
        static public List<ConsumerPrintDocumentValuedOutcomeViewModel> GetList(List<ConsumerPrintDocumentValuedOutcome> models)
        {
            List<ConsumerPrintDocumentValuedOutcomeViewModel> result = new List<ConsumerPrintDocumentValuedOutcomeViewModel>();
            foreach (var item in models)
            {
                result.Add(new ConsumerPrintDocumentValuedOutcomeViewModel(item));
            }
            return result;

        }
    }
}