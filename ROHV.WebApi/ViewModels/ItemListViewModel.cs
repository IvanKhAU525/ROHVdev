using System;
using System.Collections.Generic;
using System.Linq;
using ITCraftFrame;
using ROHV.Core.Database;

namespace ROHV.WebApi.ViewModels
{
    public class ItemListViewModel
    {
        public String Name { get; set; }
        public String Value { get; set; }
        public String AdditionalValue { get; set; }

        public ItemListViewModel(String name, String value)
        {
            this.Name = name;
            this.Value = value;
        }
        public ItemListViewModel(String name, String value, String additional)
        {
            this.Name = name;
            this.Value = value;
            this.AdditionalValue = additional;
        }

        static public List<ItemListViewModel> GetList(List<Core.Database.List> models)
        {
            List<ItemListViewModel> result = new List<ItemListViewModel>();
            result = models.Select(x => new ItemListViewModel(x.ListDescription, x.ListId.ToString())).ToList();
            return result;
        }

        static public List<ItemListViewModel> GetAgencyNameList(List<Core.Database.Agency> list)
        {
            List<ItemListViewModel> result = new List<ItemListViewModel>();
            result = list.Select(x => new ItemListViewModel(x.NameCompany, x.Id.ToString())).ToList();
            return result;
        }

        static public List<ItemListViewModel> GetList(List<Core.Database.ServicesList> models)
        {
            List<ItemListViewModel> result = new List<ItemListViewModel>();
            result = models.Select(x => new ItemListViewModel(x.ServiceDescription, x.ServiceId.ToString())).ToList();
            return result;
        }

        public static List<ItemListViewModel> GetList(List<EmployeeDocumentType> models)
        {
            List<ItemListViewModel> result = new List<ItemListViewModel>();
            result = models.Select(x => new ItemListViewModel(x.Name, x.EmployeeDocumentTypeId.ToString())).ToList();
            return result;
        }

        public static List<ItemListViewModel> GetList(List<ConsumerHabPlanFrequency> models)
        {
            List<ItemListViewModel> result = new List<ItemListViewModel>();
            result = models.Select(x => new ItemListViewModel(x.Name, x.Id.ToString())).ToList();
            return result;
        }

        public static List<ItemListViewModel> GetList(List<DocumentPrintType> models)
        {
            List<ItemListViewModel> result = new List<ItemListViewModel>();
            result = models.Select(x => new ItemListViewModel(x.Name, x.Id.ToString(),x.ServiceTypeId.ToString())).ToList();
            return result;
        }

        public static List<ItemListViewModel> GetList(List<ServiceType> models)
        {
            List<ItemListViewModel> result = new List<ItemListViewModel>();
            result = models.Select(x => new ItemListViewModel(x.Name, x.Id.ToString())).ToList();
            return result;
        }

        public static List<ItemListViewModel> GetExList(List<ServiceType> models)
        {
            List<ItemListViewModel> result = new List<ItemListViewModel>();
            result = models.Select(x => new ItemListViewModel(x.Name, x.Id.ToString())).ToList();
            var modelRespite = result.SingleOrDefault(x => x.Name == "Respite");

            if (modelRespite != null)
            {
                modelRespite.AdditionalValue = "In home Respite";
                result.Add(new ItemListViewModel(modelRespite.Name,modelRespite.Value, "Site based Respite"));
                result.Add(new ItemListViewModel(modelRespite.Name, modelRespite.Value, "Overnight Respite"));
                result.Add(new ItemListViewModel(modelRespite.Name, modelRespite.Value, "Recreational Respite"));
                //result.Add(new ItemListViewModel(modelRespite.Name, modelRespite.Value, "CONTRACTED Community Habilitation"));
                //result.Add(new ItemListViewModel(modelRespite.Name, modelRespite.Value, "CONTRACTED In home Respite"));
                //result.Add(new ItemListViewModel(modelRespite.Name, modelRespite.Value, "CONTRACTED Site Based Respite"));
                //result.Add(new ItemListViewModel(modelRespite.Name, modelRespite.Value, "CONTRACTED Recreational Respite"));
            }

            return result;
        }

        public static List<ItemListViewModel> GetList(List<ConsumerNoteType> models)
        {
            List<ItemListViewModel> result = new List<ItemListViewModel>();
            result = models.Select(x => new ItemListViewModel(x.Name, x.Id.ToString())).ToList();
            return result;
        }

        public static List<ItemListViewModel> GetList(List<ConsumerNoteFromType> models)
        {
            List<ItemListViewModel> result = new List<ItemListViewModel>();
            result = models.Select(x => new ItemListViewModel(x.Name, x.Id.ToString())).ToList();
            return result;
        }


        public static List<ItemListViewModel> GetList(List<NotificationStatus> models)
        {
            List<ItemListViewModel> result = new List<ItemListViewModel>();
            result = models.Select(x => new ItemListViewModel(x.Name, x.Id.ToString())).ToList();
            return result;
        }


        public static List<ItemListViewModel> GetList(List<NotificationType> models)
        {
            List<ItemListViewModel> result = new List<ItemListViewModel>();
            result = models.Select(x => new ItemListViewModel(x.Name, x.Id.ToString())).ToList();
            return result;
        }
        public static List<ItemListViewModel> GetList(List<ConsumerHabPlanDuration> models)
        {
            List<ItemListViewModel> result = new List<ItemListViewModel>();
            result = models.Select(x => new ItemListViewModel(x.Name, x.Id.ToString())).ToList();
            return result;
        }

        public static List<ItemListViewModel> GetList(List<ConsumerHabPlanStatus> models)
        {
            List<ItemListViewModel> result = new List<ItemListViewModel>();
            result = models.Select(x => new ItemListViewModel(x.Name, x.Id.ToString())).ToList();
            return result;
        }

        public static List<ItemListViewModel> GetList(List<EmployeeDocumentStatus> models)
        {
            List<ItemListViewModel> result = new List<ItemListViewModel>();
            result = models.Select(x => new ItemListViewModel(x.Name, x.DocumentEmployeeStatusId.ToString())).ToList();
            return result;
        }

        static public List<ItemListViewModel> GetStateList(List<Core.Database.State> models)
        {
            List<ItemListViewModel> result = new List<ItemListViewModel>();
            result = models.Select(x => new ItemListViewModel(x.Name, x.Code)).ToList();
            return result;
        }
        public static List<ItemListViewModel> GetDeptsList(List<Department> models)
        {
            List<ItemListViewModel> result = new List<ItemListViewModel>();
            result = models.Select(x => new ItemListViewModel(x.Name, x.Id.ToString())).ToList();
            return result;
        }

        public static List<ItemListViewModel> GetTypesList(List<ContactType> models)
        {
            List<ItemListViewModel> result = new List<ItemListViewModel>();
            result = models.Select(x => new ItemListViewModel(x.Name, x.Id.ToString())).ToList();
            return result;
        }


        public static List<ItemListViewModel> GetCategories(List<List> models)
        {
            List<ItemListViewModel> result = new List<ItemListViewModel>();
            result = models.Where(x=>!String.IsNullOrEmpty(x.ListDescription)).Select(x => new ItemListViewModel(x.ListDescription, x.ListId.ToString())).ToList();
            return result;
        }

        static public List<TDestionation> GetClientList<TSource,TDestionation>(List<TSource> models) where TDestionation : class, new()
        {
            List<TDestionation> result = new List<TDestionation>();
            result = models.Select(x => CustomMapper.MapEntity< TSource, TDestionation>(x)).ToList();
            return result;
        }
    }
}