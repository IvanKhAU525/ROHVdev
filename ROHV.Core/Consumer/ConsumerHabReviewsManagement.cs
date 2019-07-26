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
using ROHV.Core.Models;
using ROHV.Core.Enums;

namespace ROHV.Core.Consumer
{
    public class ConsumerHabReviewsManagement : BaseModel
    {
        public ConsumerHabReviewsManagement(RayimContext context) : base(context)
        {
        }

        public async Task<List<ConsumerHabReview>> GetHabReviews(Int32 consumerId)
        {
            var habReviews = await _context.ConsumerHabReviews.Where(x => x.ConsumerId == consumerId)
                .OrderByDescending(x => x.DateCreated).ToListAsync();
            return habReviews;
        }

        public async Task<ConsumerHabReview> GetDefaultModel(Int32 consumerId)
        {
            var model = await _context.Consumers.SingleOrDefaultAsync(x => x.ConsumerId == consumerId);
            ConsumerHabReview result = new ConsumerHabReview()
            {
                CHCoordinatorId = model.CHCoordinatorId,
                ContactCHCoordinator = model.CHCoordinatorId.HasValue
                    ? _context.Contacts.SingleOrDefault(x => x.ContactId == model.CHCoordinatorId.Value)
                    : null,

                DHCoordinatorId = model.DHCoordinatorId,
                ContactDHCoordinator = model.DHCoordinatorId.HasValue
                    ? _context.Contacts.SingleOrDefault(x => x.ContactId == model.DHCoordinatorId.Value)
                    : null,

                MSCId = model.MSCId,
                ContactMSC = model.MSCId.HasValue
                    ? _context.Contacts.SingleOrDefault(x => x.ContactId == model.MSCId.Value)
                    : null,

                AdvocateId = model.AdvocateId,
                ContactAdvocate = model.AdvocateId.HasValue
                    ? _context.Contacts.SingleOrDefault(x => x.ContactId == model.AdvocateId.Value)
                    : null
            };
            return result;
        }

        public async Task<Int32> Save(ConsumerHabReview dbModel)
        {
            if (dbModel.Id == 0)
            {
                _context.ConsumerHabReviews.Add(dbModel);
            }
            else
            {
                var model = await _context.ConsumerHabReviews.SingleOrDefaultAsync(x => x.Id == dbModel.Id);
                if (model != null)
                {
                    model.ConsumerId = dbModel.ConsumerId;
                    model.ServiceId = dbModel.ServiceId;

                    model.CHCoordinatorId = dbModel.CHCoordinatorId;
                    model.DHCoordinatorId = dbModel.DHCoordinatorId;
                    model.MSCId = dbModel.MSCId;
                    model.AdvocateId = dbModel.AdvocateId;

                    model.Parents = dbModel.Parents;
                    model.Others = dbModel.Others;
                    model.Others2 = dbModel.Others2;
                    model.Others3 = dbModel.Others3;
                    model.IsIncludeIndividialToParticipant = dbModel.IsIncludeIndividialToParticipant;
                    model.IsAutoSignature = dbModel.IsAutoSignature;
                    model.IsMSCParticipant = dbModel.IsMSCParticipant;

                    model.Notes = dbModel.Notes;

                    model.AddedById = dbModel.AddedById;
                    model.UpdatedById = dbModel.UpdatedById;
                    model.DateCreated = dbModel.DateCreated;
                    model.DateUpdated = dbModel.DateUpdated;
                    model.DateReview = dbModel.DateReview;
                    model.SignatureDate = dbModel.SignatureDate;
                }
            }
            await _context.SaveChangesAsync();
            var issueModel =
                await _context.ConsumerHabReviewIssueStates.SingleOrDefaultAsync(x =>
                    x.ConsumerHabReviewId == dbModel.Id);
            var issueData = dbModel.ConsumerHabReviewIssueStates.First();
            if (issueModel != null)
            {
                issueModel.AdvocatesSatisfactionState = issueData.AdvocatesSatisfactionState;
                issueModel.CommunityHabilitationPlanState = issueData.CommunityHabilitationPlanState;
                issueModel.SignificantChangesState = issueData.SignificantChangesState;
                issueModel.SafeguardChangeState = issueData.SafeguardChangeState;
                issueModel.SignificantHealthState = issueData.SignificantHealthState;
                issueModel.IndividualsSatisfactionState = issueData.IndividualsSatisfactionState;
                issueModel.ValuedOutcomesState = issueData.ValuedOutcomesState;
            }
            else
            {
                issueData.Id = Guid.NewGuid();
                issueData.ConsumerHabReviewId = dbModel.Id;
                _context.ConsumerHabReviewIssueStates.Add(issueData);
            }
            await _context.SaveChangesAsync();

            return dbModel.Id;
        }

        public async Task<Boolean> Delete(Int32 id)
        {
            var model = await _context.ConsumerHabReviews.SingleOrDefaultAsync(x => x.Id == id);
            if (model != null)
            {
                _context.ConsumerHabReviewIssueStates.Remove(
                    _context.ConsumerHabReviewIssueStates.SingleOrDefault(x => x.ConsumerHabReviewId == model.Id));
                _context.ConsumerHabReviews.Remove(model);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task DeleteAll(Int32 consumerId)
        {
            var models = _context.ConsumerHabReviews.Where(x => x.ConsumerId == consumerId);
            if (models.Any())
            {
                _context.ConsumerHabReviewIssueStates.RemoveRange(
                    _context.ConsumerHabReviewIssueStates.Where(x => models.Any(z => z.Id == x.ConsumerHabReviewId)));
                _context.ConsumerHabReviews.RemoveRange(models);
                await _context.SaveChangesAsync();
            }
        }

        public ConsumerHabReview GetHabReview(Int32 id)
        {
            var model = _context.ConsumerHabReviews.SingleOrDefault(x => x.Id == id);
            return model;
        }

        private Byte[] RenderPDF(ReportViewer reportViewer)
        {
            Warning[] warnings;
            String[] streamids;
            String mimeType,
                encoding,
                extension;

            return reportViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension,
                out streamids, out warnings);
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

        public String GetName(Core.Database.Contact contact)
        {
            if (contact == null) return "";
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

        public List<String> GetParticipants(ConsumerHabReview document, out int countRows)
        {
            List<String> result = new List<string>();

            if (document.ServiceId == 1)
            {
                String chCoordinator = document.ContactCHCoordinator != null
                    ? this.GetName(document.ContactCHCoordinator) + " - CH Coordinator"
                    : "";
                if (!String.IsNullOrEmpty(chCoordinator))
                {
                    result.Add(chCoordinator);
                }
            }
            if (document.ServiceId == 2)
            {
                String dhCoordinator = document.ContactDHCoordinator != null
                    ? this.GetName(document.ContactDHCoordinator) + " - DH Coordinator"
                    : "";
                if (!String.IsNullOrEmpty(dhCoordinator))
                {
                    result.Add(dhCoordinator);
                }
            }
            String msc = document.ContactMSC != null ? this.GetName(document.ContactMSC) + " - MSC" : "";
            if (!String.IsNullOrEmpty(msc) && document.IsMSCParticipant)
            {
                result.Add(msc);
            }

            if (!String.IsNullOrEmpty(document.Parents))
            {
                result.Add(document.Parents + " - Parent");
            }

            if (document.IsIncludeIndividialToParticipant)
            {
                result.Add(this.GetName(document.Consumer) + " - Individual");
            }

            if (!String.IsNullOrEmpty(document.Others))
            {
                result.Add(document.Others);
            }

            if (!String.IsNullOrEmpty(document.Others2))
            {
                result.Add(document.Others2);
            }

            if (!String.IsNullOrEmpty(document.Others3))
            {
                result.Add(document.Others3);
            }
            countRows = result.Count;
            var count = 8 - result.Count;
            for (Int32 i = 0; i < count; i++)
            {
                result.Add(String.Empty);
            }
            return result;
        }

        public Int32 GetIssueState(Int32? val)
        {
            if (val.HasValue) return val.Value;
            return -10;
        }

        public byte[] GetPDF(Int32 habReviewId, BaseController claimsApiController, out String name, DateTime? templateDate)
        {
            ConsumerHabReview document = _context.ConsumerHabReviews.SingleOrDefault(x => x.Id == habReviewId);
            name = "";
            if (document == null) return null;

            var reportViewer = new ReportViewer();
            reportViewer.Reset();
            reportViewer.LocalReport.EnableExternalImages = true;
            reportViewer.ProcessingMode = ProcessingMode.Local;

            IPDFhabReviewReportModel habReviewTypeInstance = ReportTypesFactory.GetAppropriateHabReviewReportType((ServiceTypeIdEnum)document.ServiceId);
            
            if (habReviewTypeInstance == null)
            {
                  throw new Exception("Wrong service type for printing the document");
            }
            habReviewTypeInstance.CalculateTemplatePath(templateDate, _context);
            reportViewer.LocalReport.ReportPath = claimsApiController.Server.MapPath(habReviewTypeInstance.ReportPath);
            Contact coordinatorContact = habReviewTypeInstance.GetCoordinatorContact(document);
            string titleDocument = habReviewTypeInstance.TitleDocument;
            bool showReviewedBy = habReviewTypeInstance.ShowReviewedBy;
            string coordinatorLabel = habReviewTypeInstance.CoordinatorLabel;
            name = habReviewTypeInstance.Name;

            var dataset = GenerateDataSetForHabReview(document, habReviewTypeInstance);

            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dataset));
            return RenderPDF(reportViewer);
        }

        public IEnumerable<object> GenerateDataSetForHabReview(ConsumerHabReview document, IPDFhabReviewReportModel habReviewTypeInstance)
        {
            IEnumerable<object> timeObjs = null;

            int participantCount;
            List<string> participants = this.GetParticipants(document, out participantCount);

            var stateIssue = document.ConsumerHabReviewIssueStates == null ? new ConsumerHabReviewIssueState() : document.ConsumerHabReviewIssueStates.First();

            if (document != null)
            {
                timeObjs = new[]
                {
                    new
                    {
                        PatientName = this.GetName(document.Consumer),
                        Medicaid = document.Consumer.MedicaidNo,
                        PatientDOB = document.Consumer.DateOfBirth.ToDateString(),
                        DateReview = document.DateReview.ToDateString(),
                        SignatureDate = document.SignatureDate.ToDateString(),
                        CHCoordinator = habReviewTypeInstance.CoordinatorContact != null ? this.GetName(habReviewTypeInstance.CoordinatorContact) : "",
                        MSC =document.ContactMSC != null ? this.GetName(document.ContactMSC) : "",
                        Participant1 = participants[0],
                        Participant2 = participants[1],
                        Participant3 = participants[2],
                        Participant4 = participants[3],
                        Participant5 = participants[4],
                        Participant6 = participants[5],
                        Participant7 = participants[6],
                        Participant8 = participants[7],
                        ValuedOutcomesState = GetIssueState(stateIssue.ValuedOutcomesState),
                        CommunityHabilitationPlanState = GetIssueState(stateIssue.CommunityHabilitationPlanState),
                        SafeguardChangeState = GetIssueState(stateIssue.SafeguardChangeState),
                        IndividualsSatisfactionState = GetIssueState(stateIssue.IndividualsSatisfactionState),
                        AdvocatesSatisfactionState = GetIssueState(stateIssue.AdvocatesSatisfactionState),
                        SignificantChangesState = GetIssueState(stateIssue.SignificantChangesState),
                        SignificantHealthState = GetIssueState(stateIssue.SignificantHealthState),
                        TitleDocument = habReviewTypeInstance.TitleDocument,
                        ShowReviewedBy = habReviewTypeInstance.ShowReviewedBy,
                        CHSignature = GetCHsignature(document, habReviewTypeInstance.CoordinatorContact),
                        CHSignatureMimeType = GetCHsignatureType(document, habReviewTypeInstance.CoordinatorContact),
                        Note = document.Notes,
                        CoordinatorLabel = habReviewTypeInstance.CoordinatorLabel,
                        ParticipantCount = participantCount
                    }
                };

            }
            return timeObjs;
        }

        private byte[] GetCHsignature(ConsumerHabReview document, Contact coordinatorContact)
        {
            byte[] chSignature = null;
            if (document.IsAutoSignature)
            {
                chSignature = (coordinatorContact?.Signature != null
                    ? Convert.FromBase64String(coordinatorContact.Signature.Split(',')[1])
                    : null);
            }
            return chSignature;
        }

        private string GetCHsignatureType(ConsumerHabReview document, Contact coordinatorContact)
        {
            string chSignatureType = string.Empty;
            if (document.IsAutoSignature)
            {

                chSignatureType = (coordinatorContact != null
                    ? coordinatorContact.Signature?.Split(';')[0].Split(':')[1]
                    : "");
            }
            return chSignatureType;
        }
    }
}