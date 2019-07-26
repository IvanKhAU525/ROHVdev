using ROHV.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ROHV.WebApi.ViewModels
{
    public class ConsumerHabReviewIssueStatesModel
    {

        public Int32? ValuedOutcomesState { get; set; }
        public Int32? CommunityHabilitationPlanState { get; set; }
        public Int32? IndividualsSatisfactionState { get; set; }
        public Int32? AdvocatesSatisfactionState { get; set; }
        public Int32? SignificantChangesState { get; set; }        
        public Int32? SafeguardChangeState { get; set; }
        public Int32? SignificantHealthState { get; set; }

        public ConsumerHabReviewIssueStatesModel() {}
        public ConsumerHabReviewIssueStatesModel(ConsumerHabReviewIssueState model)
        {
            if (model == null) return;
            this.ValuedOutcomesState = model.ValuedOutcomesState;
            this.CommunityHabilitationPlanState = model.CommunityHabilitationPlanState;
            this.IndividualsSatisfactionState = model.IndividualsSatisfactionState;
            this.AdvocatesSatisfactionState = model.AdvocatesSatisfactionState;
            this.SignificantChangesState = model.SignificantChangesState;
            this.SafeguardChangeState = model.SafeguardChangeState;
            this.SignificantHealthState = model.SignificantHealthState;
        }

        public ConsumerHabReviewIssueState GetModel()
        {
            ConsumerHabReviewIssueState model = new ConsumerHabReviewIssueState()
            {
                Id = Guid.NewGuid(),
                AdvocatesSatisfactionState = this.AdvocatesSatisfactionState,
                CommunityHabilitationPlanState = this.CommunityHabilitationPlanState,
                ValuedOutcomesState = this.ValuedOutcomesState,
                IndividualsSatisfactionState = this.IndividualsSatisfactionState,
                SignificantChangesState = this.SignificantChangesState,
                SignificantHealthState = this.SignificantHealthState,
                SafeguardChangeState = this.SafeguardChangeState
            };
            return model;
        }
    }
}