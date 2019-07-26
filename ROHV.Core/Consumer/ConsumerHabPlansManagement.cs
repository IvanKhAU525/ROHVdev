using ROHV.Core.Database;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System;
using ROHV.Core.Services;
using ROHV.Controllers;
using Microsoft.Reporting.WebForms;
using System.IO;
using System.Globalization;
using System.Net.Http.Headers;
using ROHV.Core.Models.HabPlanReports;
using ROHV.Core.Models;


namespace ROHV.Core.Consumer
{
  
    public class ConsumerHabPlansManagement : BaseModel
    {
        private const int MAX_SAFE_GUARDS = 4;
        public ConsumerHabPlansManagement(RayimContext context) : base(context)
        {
        }

        public async Task<List<ConsumerHabPlan>> GetHabPlans(Int32 consumerId)
        {
            var habPlans = await _context.ConsumerHabPlans.Where(x => x.ConsumerId == consumerId)
                .OrderByDescending(x => x.DateCreated).ToListAsync();
            return habPlans;
        }

        public async Task<List<ConsumerHabPlanStatus>> GetStatuses()
        {
            var result = await _context.ConsumerHabPlanStatuses.ToListAsync();
            return result;
        }

        public async Task<List<ConsumerHabPlanDuration>> GetDurations()
        {
            var result = await _context.ConsumerHabPlanDurations.ToListAsync();
            return result;
        }

        public async Task<List<ConsumerHabPlanFrequency>> GetFrequencies()
        {
            var result = await _context.ConsumerHabPlanFrequencies.ToListAsync();
            return result;
        }

        public async Task<Int32> Save(ConsumerHabPlan dbModel, List<ConsumerHabPlanValuedOutcome> outcomes,
            List<ConsumerHabPlanVOServeAction> actions, List<ConsumerHabPlanSafeguard> safeguards)
        {
            if (dbModel.ConsumerHabPlanId == 0)
            {
                _context.ConsumerHabPlans.Add(dbModel);
            }
            else
            {
                var model = await _context.ConsumerHabPlans.SingleOrDefaultAsync(x =>
                    x.ConsumerHabPlanId == dbModel.ConsumerHabPlanId);
                if (model != null)
                {
                    model.ConsumerId = dbModel.ConsumerId;
                    model.HabServiceId = dbModel.HabServiceId;
                    model.CoordinatorId = dbModel.CoordinatorId;
                    model.FrequencyId = dbModel.FrequencyId;
                    model.DurationId = dbModel.DurationId;
                    model.StatusId = dbModel.StatusId;
                    model.AddedById = dbModel.AddedById;
                    model.UpdatedById = dbModel.UpdatedById;
                    model.Name = dbModel.Name;
                    model.QMRP = dbModel.QMRP;
                    model.EnrolmentDate = dbModel.EnrolmentDate;
                    model.SignatureDate = dbModel.SignatureDate;
                    model.DatePlan = dbModel.DatePlan;
                    model.EffectivePlan = dbModel.EffectivePlan;
                    model.IsAproved = dbModel.IsAproved;
                    model.IsAutoSignature = dbModel.IsAutoSignature;
                    model.DateCreated = dbModel.DateCreated;
                    model.DateUpdated = dbModel.DateUpdated;
                    var outcomes_old =
                        _context.ConsumerHabPlanValuedOutcomes.Where(x => x.HabPlanId == dbModel.ConsumerHabPlanId);
                    var actions_old = _context.ConsumerHabPlanVOServeActions.Where(x =>
                        outcomes_old.Any(z => z.Id == x.HabPlanValuedOutcomeId));
                    var safeguards_old =
                        _context.ConsumerHabPlanSafeguards.Where(x => x.ConsumerHabPlanId == dbModel.ConsumerHabPlanId);
                    _context.ConsumerHabPlanVOServeActions.RemoveRange(actions_old);
                    _context.ConsumerHabPlanValuedOutcomes.RemoveRange(outcomes_old);
                    _context.ConsumerHabPlanSafeguards.RemoveRange(safeguards_old);

                }
            }
            await _context.SaveChangesAsync();
            if (outcomes.Count > 0)
            {
                foreach (var outcome in outcomes)
                {
                    outcome.HabPlanId = dbModel.ConsumerHabPlanId;
                    Int32 tempId = outcome.Id;
                    outcome.Id = 0;
                    _context.ConsumerHabPlanValuedOutcomes.Add(outcome);
                    await _context.SaveChangesAsync();
                    foreach (var action in actions)
                    {
                        if (action.HabPlanValuedOutcomeId == tempId && tempId < 0)
                        {
                            action.HabPlanValuedOutcomeId = outcome.Id;
                            _context.ConsumerHabPlanVOServeActions.Add(action);
                        }
                    }
                }
                await _context.SaveChangesAsync();
            }

            if (safeguards.Count > 0)
            {
                foreach (var safeguard in safeguards)
                {
                    safeguard.ConsumerHabPlanId = dbModel.ConsumerHabPlanId;
                    safeguard.ConsumerHabPlanSafeguardId = 0;
                    _context.ConsumerHabPlanSafeguards.Add(safeguard);
                }
                await _context.SaveChangesAsync();
            }
            return dbModel.ConsumerHabPlanId;
        }

      

        public async Task<Boolean> Delete(Int32 id)
        {
            var model = await _context.ConsumerHabPlans.SingleOrDefaultAsync(x => x.ConsumerHabPlanId == id);
            if (model != null)
            {
                var outcomes_old = _context.ConsumerHabPlanValuedOutcomes.Where(x => x.HabPlanId == id);
                var actions_old =
                    _context.ConsumerHabPlanVOServeActions.Where(x =>
                        outcomes_old.Any(z => z.Id == x.HabPlanValuedOutcomeId));
                var safeguards_old = _context.ConsumerHabPlanSafeguards.Where(x => x.ConsumerHabPlanId == id);
                _context.ConsumerHabPlanVOServeActions.RemoveRange(actions_old);
                _context.ConsumerHabPlanValuedOutcomes.RemoveRange(outcomes_old);
                _context.ConsumerHabPlanSafeguards.RemoveRange(safeguards_old);
                ;
                _context.ConsumerHabPlans.Remove(model);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;

        }

        public async Task DeleteAll(Int32 consumerId)
        {
            var models = _context.ConsumerHabPlans.Where(x => x.ConsumerId == consumerId);
            if (models.Count() > 0)
            {
                var safeguards_old = _context.ConsumerHabPlanSafeguards.Where(x =>
                    models.Any(z => z.ConsumerHabPlanId == x.ConsumerHabPlanId));
                _context.ConsumerHabPlanSafeguards.RemoveRange(safeguards_old);
                var vos = _context.ConsumerHabPlanValuedOutcomes.Where(x =>
                    models.Any(z => z.ConsumerHabPlanId == x.HabPlanId));
                if (vos.Count() > 0)
                {
                    var actions =
                        _context.ConsumerHabPlanVOServeActions.Where(
                            x => vos.Any(z => z.Id == x.HabPlanValuedOutcomeId));
                    if (actions.Count() > 0)
                    {
                        _context.ConsumerHabPlanVOServeActions.RemoveRange(actions);
                    }
                    _context.ConsumerHabPlanValuedOutcomes.RemoveRange(vos);
                }
                _context.ConsumerHabPlans.RemoveRange(models);
                await _context.SaveChangesAsync();
            }
        }

        public ConsumerHabPlan GetHabPLan(Int32 id)
        {
            var model = _context.ConsumerHabPlans.SingleOrDefault(x => x.ConsumerHabPlanId == id);
            return model;
        }
        private Byte[] RenderPDF(ReportViewer reportViewer)
        {
            String mimeType,
                encoding,
                extension;

            return reportViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension,
                out string[] streamids, out Warning[] warnings);
        }
        public String ConvertInvalidFilePathChars(String value)
        {
            var invalidChars = new List<Char>(Path.GetInvalidFileNameChars());
            invalidChars.Add('№');

            foreach (var symbol in invalidChars)
            {
                value = value.Replace(symbol.ToString(), " ");
            }

            return value;
        }
        public String GetName(Core.Database.Consumer consumer)
        {
            String result = "";
            if (!String.IsNullOrEmpty(consumer.MI))
            {
                result += consumer.FirstName + " " + consumer.MI + " " + consumer.LastName;
            }
            else
            {
                result += consumer.FirstName + " " + consumer.LastName;
            }
            return result;
        }

        public String GetMedicaidNumberByDate(Database.Consumer consumer, DateTime dateOfPlan)
        {
            var result =
                ConsumerMedicaidNumberManagement.GetMedicaidNumberByDate(_context, dateOfPlan, consumer.ConsumerId);

            return result?.MedicaidNo ?? consumer.MedicaidNo;
        }

        public String GetScheduledMSCName(Core.Database.Consumer consumer, DateTime dateOfPlan)
        {
            var scheduledMSC = ConsumerServiceCoordinatorManagement.GetScheduledContactByDate(_context, dateOfPlan, consumer.ConsumerId);

            return GetName(scheduledMSC ?? consumer.ServiceCoordinatorContact);
        }
        public String GetName(Core.Database.Contact contact)
        {

            String result = "";
            if (!String.IsNullOrEmpty(contact.Salutation))
            {
                result += contact.Salutation + " ";
            }
            if (!String.IsNullOrEmpty(contact.MiddleName))
            {
                result += contact.FirstName + " " + contact.MiddleName + " " + contact.LastName;
            }
            else
            {
                result += contact.FirstName + " " + contact.LastName;
            }
            return result;
        }

        public String GetDate(DateTime? date)
        {
            return date.HasValue ? date.Value.ToString(@"MM\/dd\/yyyy", CultureInfo.InvariantCulture) : "";
        }

        public byte[] GetPDF(Int32 habPlanId, BaseController claimsApiController, DateTime? templateDate, out String name)
        {
            var habPlan = this.GetHabPLan(habPlanId);

            ComHabPlanReportModel model = new ComHabPlanReportModel();
            model.CalculateTemplatePath(templateDate, _context);

            var reportViewer = new ReportViewer();

            reportViewer.Reset();
            reportViewer.LocalReport.EnableExternalImages = true;
            reportViewer.ProcessingMode = ProcessingMode.Local;

            reportViewer.LocalReport.ReportPath = claimsApiController.Server.MapPath(model.ReportPath);

            name = this.ConvertInvalidFilePathChars(habPlan.Name) + "-ComHabPlan";

            var dataSets = model.GetDataSets(habPlan, this);
            foreach(var dataSet in dataSets) {
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource(dataSet.Key, dataSet.Value));
            }
            return RenderPDF(reportViewer);
        }
       
        public List<SafeGardRepotModel> GetSafeguardsWithActions(ConsumerHabPlan habPlan)
        {
            var result = ITCraftFrame.CustomMapper.MapList< SafeGardRepotModel, ConsumerHabPlanSafeguard>(habPlan.ConsumerHabPlanSafeguards.ToList());          
            return result;           
        }
        public String GetSafeguards(ConsumerHabPlan habPlan)
        {
            String result = "";
            if (habPlan.ConsumerHabPlanSafeguards != null)
            {
                if (habPlan.ConsumerHabPlanSafeguards.Count > 0)
                {
                    result += "<ul>";
                    foreach (var item in habPlan.ConsumerHabPlanSafeguards)
                    {
                        result += "<li>" + item.Item + "</li>";                      
                    }
                    result += "</ul>";
                }
            }
            return result;
        }

        public String GetServiceName(ConsumerHabPlan habPlan)
        {
            string serviceType = habPlan.ServicesList.ServiceType;
            if (serviceType.Contains("Community"))
            {
                return "Community";
            }
            if (habPlan.ServicesList.ServiceDescription.Contains("Supplemental"))
            {
                return "Supplemental Group Day";
            }
            if (serviceType.Contains("Day Hab"))
            {
                return "Group Day";
            }
            if (serviceType.Contains("Respite"))
            {
                return "Respite";
            }

            return "Undefined";
        }

        public string GetCoordinatorLabel(ConsumerHabPlan habPlan)
        {
            string serviceType = habPlan.ServicesList.ServiceType;
            if (serviceType.Contains("Community"))
            {
                return "CH";
            }
            if (serviceType.Contains("Day Hab"))
            {
                return "DH";
            }
            if (serviceType.Contains("Respite"))
            {
                return "RH";
            }
            return "";
        }

        public byte[] GetSignature(ConsumerHabPlan habPlan)
        {
            byte[] chSignature = null;
            if (habPlan.IsAutoSignature)
            {
                chSignature = (habPlan.Contact?.Signature != null
                    ? Convert.FromBase64String(habPlan.Contact.Signature.Split(',')[1])
                    : null);
            }
            return chSignature;
        }

        public String GetSignatureType(ConsumerHabPlan habPlan)
        {
            String chSignatureType = string.Empty;
            if (habPlan.IsAutoSignature)
            {
                chSignatureType = (habPlan.Contact != null
                    ? habPlan.Contact.Signature?.Split(';')[0].Split(':')[1]
                    : "");
            }
            return chSignatureType;
        }

        public List<String> GetHabPlanOutcomeList(ConsumerHabPlan habPlan)
        {
            List<String> valueOutcomeList = new List<string>();
            foreach (var valueOutcome in habPlan.ConsumerHabPlanValuedOutcomes)
            {
                valueOutcomeList.Add(valueOutcome.ValuedOutcome);
            }
            for (int i = valueOutcomeList.Count - 1; i < 4; i++)
            {
                valueOutcomeList.Add(String.Empty);
            }
            return valueOutcomeList;
        }

        public List<ReportHabPlanOutcomeValue> GetReportHabPlanOutcomeValueList(ConsumerHabPlan habPlan)
        {
            var result = habPlan.ConsumerHabPlanValuedOutcomes.Select(x =>
                    {
                        ReportHabPlanOutcomeValue item = ITCraftFrame.CustomMapper.MapEntity<ReportHabPlanOutcomeValue>(x);
                        item.ViewActions = GetHabPlanValuedOutcomeActions(x);
                        return item;
                    }).ToList();

            return result;
        }

        public List<String> GetHabPlanMyGoals(ConsumerHabPlan habPlan) => habPlan.ConsumerHabPlanValuedOutcomes.Select(x => x.MyGoal).ToList();

        public List<String> GetHabPlanActionsList(ConsumerHabPlan habPlan)
        {
            List<String> actionsList = new List<string>();
            foreach (var valueOutcome in habPlan.ConsumerHabPlanValuedOutcomes)
            {
                actionsList.Add(GetHabPlanValuedOutcomeActions(valueOutcome));
            }
            return actionsList;
        }

        public string GetHabPlanValuedOutcomeActions(ConsumerHabPlanValuedOutcome outcomeValue)
        {
            List<String> tmpActions = outcomeValue.ConsumerHabPlanVOServeActions.Select((x, i) => $"{i + 1}. {x.ServeAndAction}").ToList();

            return String.Join("\r\n", tmpActions);
        }
    }
}