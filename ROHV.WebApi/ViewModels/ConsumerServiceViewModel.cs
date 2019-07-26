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
    public sealed class ConsumerServiceViewModel : ConsumerServiceModel
    {
        public string AddedByView { get => $"{CreatedByUser?.FirstName} {CreatedByUser?.LastName}"; }
        public string EditedByView { get => $"{EditedByUser?.FirstName} {EditedByUser?.LastName}"; }

        public int? FileId { set; get; }
        public string FileData { set; get; }
        public string FileName { get; set; }

        public ConsumerServiceViewModel() { }
        public ConsumerServiceViewModel(ConsumerService model)
        {
            CustomMapper.MapEntity(model, this);
            var servicesFiles = ConsumerServicesManagement.GetFilesByConsumerServicesId(model.ConsumerServiceId).FirstOrDefault();
            FileId = servicesFiles?.Id;
            FileName = servicesFiles?.FileDisplayName;

            if (this.ServiceId.HasValue)
            {
                this.ServiceName = model.ServicesList.ServiceDescription;
            }

            if (this.UnitQuantities.HasValue)
            {
                this.UnitQuantitiesName = model.List.ListDescription;
            }
            if (TotalHours == null && model.AnnualUnits != null)
            {
                TotalHours = (decimal)model.AnnualUnits.Value / 4;
            }
            else if (AnnualUnits == null && model.TotalHours != null)
            {
                AnnualUnits = (int)model.TotalHours.Value * 4;
            }
        }

        static public List<ConsumerServiceViewModel> GetList(List<ConsumerService> models)
        {
            List<ConsumerServiceViewModel> result = new List<ConsumerServiceViewModel>();
            foreach (var item in models)
            {
                result.Add(new ConsumerServiceViewModel(item));
            }
            return result;

        }

        public ConsumerService GetModel()
        {
            ConsumerService model = new ConsumerService();
            CustomMapper.MapEntity(this, model);           
            model.ConsumerServiceId = this.ConsumerServiceId ?? 0;
           

            return model;
        }
    }
}