using ROHV.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ROHV.WebApi.ViewModels
{
    public class ConsumerPhonesViewModel
    {
        public Int32? ConsumerPhoneId { get; set; }
        public Int32? PhoneTypeId { get; set; }
        public String Phone { get; set; }
        public String Extension { get; set; }
        public String Note { get; set; }
        public String PhoneTypeName { get; set; }        
        public Int32 ConsumerId { get; set; }
        public ConsumerPhonesViewModel()
        {

        }
        public ConsumerPhonesViewModel(ConsumerPhone model)
        {
            this.ConsumerPhoneId = model.ConsumerPhoneId;
            this.PhoneTypeId = model.PhoneTypeId;
            this.Phone = model.Phone;
            this.Extension = model.Extension;
            this.Note = model.Note;
            if (this.PhoneTypeId.HasValue)
            {
                this.PhoneTypeName = model.List.ListDescription;
            }
        }
        static public List<ConsumerPhonesViewModel> GetList(List<ConsumerPhone> models)
        {
            List<ConsumerPhonesViewModel> result = new List<ConsumerPhonesViewModel>();
            foreach (var item in models)
            {
                result.Add(new ConsumerPhonesViewModel(item));
            }
            return result;

        }
        public ConsumerPhone GetModel()
        {
            ConsumerPhone model = new ConsumerPhone();
            if (this.ConsumerPhoneId.HasValue)
            {
                model.ConsumerPhoneId = this.ConsumerPhoneId.Value;
            }
            else
            {
                model.ConsumerPhoneId = 0;
            }
            model.ConsumerId = this.ConsumerId;
            model.PhoneTypeId = this.PhoneTypeId;
            model.Phone = this.Phone;
            model.Extension = this.Extension;
            model.Note = this.Note;

            return model;
        }
    }
}