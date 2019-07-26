using ROHV.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ROHV.WebApi.ViewModels
{
    public class ConsumerPrintDocumentVOServeActionViewModel
    {

        public Int32 Id { get; set; }
        public String ServeAndAction { get; set; }

        public Int32 ValuedOutcomeId { get; set; }
        public ConsumerPrintDocumentVOServeActionViewModel() { }
        public ConsumerPrintDocumentVOServeActionViewModel(ConsumerPrintDocumentVOServeAction model)
        {
            this.Id = model.Id;
            this.ServeAndAction = model.ServeAndAction;
            this.ValuedOutcomeId = model.PrintDocumentValuedOutcomeId;
        }
        static public List<ConsumerPrintDocumentVOServeActionViewModel> GetList(List<ConsumerPrintDocumentVOServeAction> models)
        {
            List<ConsumerPrintDocumentVOServeActionViewModel> result = new List<ConsumerPrintDocumentVOServeActionViewModel>();
            foreach (var item in models)
            {
                result.Add(new ConsumerPrintDocumentVOServeActionViewModel(item));
            }
            return result;

        }
    }
}