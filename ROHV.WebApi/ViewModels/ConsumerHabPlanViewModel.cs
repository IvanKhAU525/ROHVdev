using ITCraftFrame;
using ROHV.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ROHV.WebApi.ViewModels
{
    public class ConsumerHabPlanViewModel
    {
        public Int32? ConsumerHabPlanId { get; set; }
        public String Name { get; set; }
        public Int32 HabServiceId { get; set; }
        public String HabServiceName { get; set; }
        
        public EmployeeSearchViewModel Coordinator { get; set; }
           
        public Int32 FrequencyId { get; set; }
        public String FrequencyName { get; set; }
        public Int32 DurationId { get; set; }
        public String DurationName { get; set; }
        public Int32 StatusId { get; set; }
        public String StatusName { get; set; }
        public String QMRP { get; set; }
        public DateTime EnrolmentDate { get; set; }
        public DateTime DatePlan { get; set; }
        public DateTime? SignatureDate { get; set; }
        public DateTime EffectivePlan { get; set; }
        public Boolean IsAproved { get; set; }
        public Boolean IsAutoSignature { get; set; }        
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public Int32 AddedById { get; set; }
        public String AddedByName { get; set; }
        public Int32? UpdatedById { get; set; }
        public String UpdatedByName { get; set; }

        public Int32 ConsumerId { get; set; }
        public List<ConsumerHabPlanValuedOutcomeViewModel> ValuedOutcomes { get; set; }
        public List<ConsumerHabPlanSafeguardViewModel> Safeguards { get; set; }


        public ConsumerHabPlanViewModel()
        {
            ValuedOutcomes = new List<ConsumerHabPlanValuedOutcomeViewModel>();
        }
        public ConsumerHabPlanViewModel(ConsumerHabPlan model)
        {
            this.ConsumerHabPlanId = model.ConsumerHabPlanId;
            this.Name = model.Name;
            this.HabServiceId = model.HabServiceId;
            this.HabServiceName = model.ServicesList.ServiceDescription;
            if (model.Contact != null)
            {
                this.Coordinator = new EmployeeSearchViewModel(model.Contact);                
            }
            
            this.FrequencyId = model.FrequencyId;
            this.FrequencyName = model.ConsumerHabPlanFrequency.Name;
            this.DurationId = model.DurationId;
            this.DurationName = model.ConsumerHabPlanDuration.Name;
            this.StatusId = model.StatusId;
            this.StatusName = model.ConsumerHabPlanStatus.Name;
            this.QMRP = model.QMRP;
            this.EnrolmentDate = model.EnrolmentDate;
            this.DatePlan = model.DatePlan;
            this.SignatureDate = model.SignatureDate;
            this.EffectivePlan = model.EffectivePlan;
            this.IsAproved = model.IsAproved;
            this.IsAutoSignature = model.IsAutoSignature;
            this.DateCreated = model.DateCreated;
            this.DateUpdated = model.DateUpdated;


            if (model.ConsumerHabPlanValuedOutcomes != null)
            {
                this.ValuedOutcomes = ConsumerHabPlanValuedOutcomeViewModel.GetList(model.ConsumerHabPlanValuedOutcomes.ToList());
            }
            else
            {
                this.ValuedOutcomes = new List<ConsumerHabPlanValuedOutcomeViewModel>();
            }
            if (model.ConsumerHabPlanSafeguards != null)
            {
                this.Safeguards = ConsumerHabPlanSafeguardViewModel.GetList(model.ConsumerHabPlanSafeguards.ToList());
            }

            this.AddedById = model.AddedById;
            this.AddedByName = model.SystemUser.LastName + ", " + model.SystemUser.FirstName;

            this.UpdatedById = model.UpdatedById;
            if (this.UpdatedById.HasValue)
            {
                this.UpdatedByName = model.SystemUser1.LastName + ", " + model.SystemUser1.FirstName;
            }
            else
            {
                this.UpdatedByName = "";
            }
        }
        static public List<ConsumerHabPlanViewModel> GetList(List<ConsumerHabPlan> models)
        {
            List<ConsumerHabPlanViewModel> result = new List<ConsumerHabPlanViewModel>();
            foreach (var item in models)
            {
                result.Add(new ConsumerHabPlanViewModel(item));
            }
            return result;

        }
        public ConsumerHabPlan GetModel()
        {
            ConsumerHabPlan model = new ConsumerHabPlan();
            if (this.ConsumerHabPlanId.HasValue)
            {
                model.ConsumerHabPlanId = this.ConsumerHabPlanId.Value;
            }
            else
            {
                model.ConsumerHabPlanId = 0;
            }
            model.ConsumerId = this.ConsumerId;
            model.HabServiceId = this.HabServiceId;
            model.CoordinatorId = this.Coordinator.ContactId;
            model.FrequencyId = this.FrequencyId;
            model.DurationId = this.DurationId;
            model.StatusId = this.StatusId;
            model.AddedById = this.AddedById;
            model.UpdatedById = this.UpdatedById;
            model.Name = this.Name;
            model.QMRP = this.QMRP;
            model.EnrolmentDate = this.EnrolmentDate;
            model.DatePlan = this.DatePlan;
            model.SignatureDate = this.SignatureDate;
            model.EffectivePlan = this.EffectivePlan;
            model.IsAproved = this.IsAproved;
            model.IsAutoSignature = this.IsAutoSignature;
            model.DateCreated = this.DateCreated;
            model.DateUpdated = this.DateUpdated;
            
            return model;
        }

        public List<ConsumerHabPlanValuedOutcome> GetOutcomesModel()
        {
            List<ConsumerHabPlanValuedOutcome> result = new List<ConsumerHabPlanValuedOutcome>();
            if (ValuedOutcomes == null) return result;
            var fakeId = -1;
            foreach (var item in ValuedOutcomes)
            {
                item.Id = fakeId;
                foreach (var action in item.ServeActions)
                {
                    action.Id = 0;
                    action.ValuedOutcomeId = fakeId;
                }
                fakeId--;
                ConsumerHabPlanValuedOutcome model = CustomMapper.MapEntity<ConsumerHabPlanValuedOutcome>(item);                
                result.Add(model);
            }
            return result;

        }
        public List<ConsumerHabPlanVOServeAction> GetActionsModel()
        {
            List<ConsumerHabPlanVOServeAction> result = new List<ConsumerHabPlanVOServeAction>();
            if (ValuedOutcomes == null) return result;
            foreach (var item in ValuedOutcomes)
            {
                foreach (var action in item.ServeActions)
                {
                    ConsumerHabPlanVOServeAction model = new ConsumerHabPlanVOServeAction()
                    {
                        ServeAndAction = action.ServeAndAction,
                        Id = action.Id,
                        HabPlanValuedOutcomeId = action.ValuedOutcomeId
                    };
                    result.Add(model);
                }
            }
            return result;
        }
        public List<ConsumerHabPlanSafeguard> GetSafeguardsModel()
        {
            List<ConsumerHabPlanSafeguard> result = new List<ConsumerHabPlanSafeguard>();
            if (Safeguards == null) return result;
            foreach (var item in Safeguards)
            {
                ConsumerHabPlanSafeguard model = CustomMapper.MapEntity<ConsumerHabPlanSafeguard>(item);
                result.Add(model);

            }
            return result;
        }    
    }
}