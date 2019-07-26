using ROHV.Core.Database;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System;
using ROHV.Core.Services;
using ROHV.Controllers;
using Microsoft.Reporting.WebForms;
using System.Globalization;
using System.IO;
using ROHV.Core.Models;
using ROHV.Core.Enums;
using ROHV.Core.Models.Addresses;


namespace ROHV.Core.Consumer
{
    public class ConsumerPrintDocumentsManagement : BaseModel
    {
        public ConsumerPrintDocumentsManagement(RayimContext context) : base(context) { }

        public async Task<List<DocumentPrintType>> GetTypes()
        {
            var result = await _context.DocumentPrintTypes.Where(x => x.IsActive).ToListAsync();
            return result;
        }
        public async Task<List<ServiceType>> GetServiceTypes()
        {
            var result = await _context.ServiceTypes.ToListAsync();
            return result;
        }

        public async Task<List<ConsumerPrintDocument>> GetPrintDocuments(Int32 consumerId)
        {
            var printDocuments = await _context.ConsumerPrintDocuments.Where(x => x.ConsumerId == consumerId).OrderByDescending(x => x.DateCreated).ToListAsync();
            return printDocuments;
        }

        public async Task<Int32> Save(ConsumerPrintDocument dbModel, List<ConsumerPrintDocumentValuedOutcome> outcomes, List<ConsumerPrintDocumentVOServeAction> actions)
        {
            if (dbModel.Id == 0)
            {
                _context.ConsumerPrintDocuments.Add(dbModel);
            }
            else
            {
                var model = await _context.ConsumerPrintDocuments.SingleOrDefaultAsync(x => x.Id == dbModel.Id);
                if (model != null)
                {
                    model.ContactId = dbModel.ContactId;
                    model.AddedById = dbModel.AddedById;
                    model.UpdatedById = dbModel.UpdatedById;
                    model.ServiceAction1 = dbModel.ServiceAction1;
                    model.ServiceAction2 = dbModel.ServiceAction2;
                    model.ServiceAction3 = dbModel.ServiceAction3;
                    model.ServiceAction4 = dbModel.ServiceAction4;
                    model.DateCreated = dbModel.DateCreated;
                    model.DateUpdated = dbModel.DateUpdated;
                    model.Notes = dbModel.Notes;
                    model.EffectiveDate = dbModel.EffectiveDate;
                    model.ServiceTypeId = dbModel.ServiceTypeId;
                    model.ServiceTypeTitle = dbModel.ServiceTypeTitle;
                    var outcomes_old = _context.ConsumerPrintDocumentValuedOutcomes.Where(x => x.PrintDocumentId == dbModel.Id);
                    var actions_old = _context.ConsumerPrintDocumentVOServeActions.Where(x => outcomes_old.Any(z => z.Id == x.PrintDocumentValuedOutcomeId));
                    _context.ConsumerPrintDocumentVOServeActions.RemoveRange(actions_old);
                    _context.ConsumerPrintDocumentValuedOutcomes.RemoveRange(outcomes_old);

                }
            }
            await _context.SaveChangesAsync();

            if (outcomes.Count > 0)
            {
                foreach (var outcome in outcomes)
                {
                    outcome.PrintDocumentId = dbModel.Id;
                    Int32 tempId = outcome.Id;
                    outcome.Id = 0;
                    _context.ConsumerPrintDocumentValuedOutcomes.Add(outcome);
                    await _context.SaveChangesAsync();
                    foreach (var action in actions)
                    {
                        if (action.PrintDocumentValuedOutcomeId == tempId && tempId < 0)
                        {
                            action.PrintDocumentValuedOutcomeId = outcome.Id;
                            _context.ConsumerPrintDocumentVOServeActions.Add(action);
                        }
                    }
                }
                await _context.SaveChangesAsync();
            }
            return dbModel.Id;
        }
        public ConsumerPrintDocument GetDocument(Int32 id)
        {
            var model = _context.ConsumerPrintDocuments.SingleOrDefault(x => x.Id == id);
            return model;
        }

        public String GetAddress(ReportAddressModel address)
        {
            String result = address.Address1;
            if (!String.IsNullOrEmpty(address.Address2))
            {
                result += ", " + address.Address2;
            }
            if (!String.IsNullOrEmpty(address.City))
            {
                result += ", " + address.City;
            }
            if (!String.IsNullOrEmpty(address.State))
            {
                result += ", " + address.State;
            }
            if (!String.IsNullOrEmpty(address.Zip))
            {
                result += " " + address.Zip;
            }
            return result;
        }

        private ReportAddressModel getCurrentAddress(Database.Consumer consumer, DateTime? lastReviewDate)
        {
            ReportAddressModel result = null;

            if (lastReviewDate != null)
            {
                result = ConsumerAddressManagement.GetScheduledAddressByDate(_context, lastReviewDate.Value, consumer.ConsumerId);
            }

            return result ?? getAddressFromCustomer(consumer);

        }

        private ReportAddressModel getAddressFromCustomer(Database.Consumer consumer)
        {
            return ITCraftFrame.CustomMapper.MapEntity<ReportAddressModel>(consumer);
        }

        public String GetSityStateZip(ReportAddressModel address)
        {
            String result = String.Empty;
            if (!String.IsNullOrEmpty(address.City))
            {
                result += ", " + address.City;
            }
            if (!String.IsNullOrEmpty(address.State))
            {
                result += ", " + address.State;
            }
            if (!String.IsNullOrEmpty(address.Zip))
            {
                result += " " + address.Zip;
            }
            return result;
        }

        public String GetContactSityStateZip(Core.Database.Contact contact)
        {
            String result = String.Empty;
            if (!String.IsNullOrEmpty(contact.City))
            {
                result += ", " + contact.City;
            }
            if (!String.IsNullOrEmpty(contact.State))
            {
                result += ", " + contact.State;
            }
            if (!String.IsNullOrEmpty(contact.Zip))
            {
                result += " " + contact.Zip;
            }
            return result;
        }

        public String GetAddress(Core.Database.Contact contact)
        {
            String result = contact.Address1;
            if (!String.IsNullOrEmpty(contact.Address2))
            {
                result += ", " + contact.Address2;
            }
            if (!String.IsNullOrEmpty(contact.City))
            {
                result += ", " + contact.City;
            }
            if (!String.IsNullOrEmpty(contact.State))
            {
                result += ", " + contact.State;
            }
            if (!String.IsNullOrEmpty(contact.Zip))
            {
                result += " " + contact.Zip;
            }
            return result;

        }
        public String GetConsumerName(Core.Database.Consumer consumer)
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
        public String GetContactName(Core.Database.Contact contact)
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
        public String GetServices(ConsumerPrintDocument document)
        {
            String result = "";
            Int32 i = 0;
            if (document.ConsumerPrintDocumentValuedOutcomes != null)
            {
                foreach (var valued in document.ConsumerPrintDocumentValuedOutcomes)
                {
                    if (valued.ConsumerPrintDocumentVOServeActions != null)
                    {
                        foreach (var action in valued.ConsumerPrintDocumentVOServeActions)
                        {
                            String item = action.ServeAndAction;
                            if (!String.IsNullOrEmpty(item))
                            {
                                result += "<B>" + (i + 1).ToString() + ". </B><I>" + item + "</I><BR/>";
                                i++;
                            }
                        }
                    }
                }
            }
            return result;
        }
        private Byte[] RenderPDF(ReportViewer reportViewer)
        {
            Warning[] warnings;
            String[] streamids;
            String mimeType,
                   encoding,
                   extension;

            return reportViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);
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
        public Database.Consumer GetClientById(Int32 clientId)
        {
            var model = _context.Consumers.SingleOrDefault(x => x.ConsumerId == clientId);
            return model;
        }

        private List<String> GetActions(ConsumerPrintDocument document)
        {
            List<String> result = new List<string>();
            if (document.ConsumerPrintDocumentValuedOutcomes != null)
            {
                var i = 0;
                foreach (var valued in document.ConsumerPrintDocumentValuedOutcomes)
                {
                    if (valued.ConsumerPrintDocumentVOServeActions != null)
                    {
                        String actionString = "";
                        foreach (var action in valued.ConsumerPrintDocumentVOServeActions)
                        {
                            String item = action.ServeAndAction;
                            if (!String.IsNullOrEmpty(item))
                            {
                                actionString += "<B>" + (i + 1).ToString() + ". </B><I>" + item + "</I><BR/>";
                                i++;
                            }
                        }
                        if (!String.IsNullOrEmpty(actionString))
                        {
                            result.Add(actionString);
                        }
                    }
                }
            }
            int count = 5 - result.Count;
            for (int i = 0; i < count; i++)
            {
                result.Add(String.Empty);
            }
            return result;
        }


        private List<String> GetValued(ConsumerPrintDocument document)
        {
            List<String> result = new List<string>();
            if (document.ConsumerPrintDocumentValuedOutcomes != null)
            {
                foreach (var valued in document.ConsumerPrintDocumentValuedOutcomes)
                {
                    if (!String.IsNullOrEmpty(valued.ValuedOutcome))
                    {
                        result.Add(valued.ValuedOutcome);
                    }
                }
            }
            int count = 5 - result.Count;
            for (int i = 0; i < count; i++)
            {
                result.Add(String.Empty);
            }
            return result;

        }

        public byte[] GetPDF(string documentId, Int32 documentTypeId, BaseController claimsApiController, out String name, Boolean isEmpty)
        {
            Int32 serviceTypeId = _context.DocumentPrintTypes.SingleOrDefault(x => x.Id == documentTypeId).ServiceTypeId.Value;
            ConsumerPrintDocument document = null;
            if (!String.IsNullOrEmpty(documentId))
            {
                document = this.GetDocument(Int32.Parse(documentId));
            }

            String partName = "New";
            name = "";

            if (document != null && !isEmpty)
            {
                partName = this.ConvertInvalidFilePathChars(this.GetContactName(document.Contact));
            }

            var reportViewer = new ReportViewer();

            reportViewer.Reset();
            reportViewer.LocalReport.EnableExternalImages = true;
            reportViewer.ProcessingMode = ProcessingMode.Local;

            IPDFreportModel reportTypeInstance = ReportTypesFactory.GetAppropriatePDFreportType((ServiceTypeIdEnum)serviceTypeId, (DocumentPrintTypeEnum)documentTypeId);
            reportViewer.LocalReport.ReportPath = claimsApiController.Server.MapPath(reportTypeInstance.ReportPath);
            name = reportTypeInstance.GetName(partName);

            var timeObjs = reportTypeInstance.CreateDataSet(document, isEmpty, this);
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", timeObjs));
            return RenderPDF(reportViewer);
        }

        public async Task DeleteAll(Int32 consumerId)
        {
            var models = _context.ConsumerPrintDocuments.Where(x => x.ConsumerId == consumerId);
            if (models.Count() > 0)
            {
                var vos = _context.ConsumerPrintDocumentValuedOutcomes.Where(x => models.Any(z => z.Id == x.PrintDocumentId));
                if (vos.Count() > 0)
                {
                    var actions = _context.ConsumerPrintDocumentVOServeActions.Where(x => vos.Any(z => z.Id == x.PrintDocumentValuedOutcomeId));
                    if (actions.Count() > 0)
                    {
                        _context.ConsumerPrintDocumentVOServeActions.RemoveRange(actions);
                    }
                    _context.ConsumerPrintDocumentValuedOutcomes.RemoveRange(vos);
                }
                _context.ConsumerPrintDocuments.RemoveRange(models);
                await _context.SaveChangesAsync();
            }
        }
        public async Task Delete(Int32 documentId)
        {
            var model = _context.ConsumerPrintDocuments.SingleOrDefault(x => x.Id == documentId);
            if (model != null)
            {

                var vos = _context.ConsumerPrintDocumentValuedOutcomes.Where(x => x.PrintDocumentId == documentId);
                if (vos.Count() > 0)
                {
                    var actions = _context.ConsumerPrintDocumentVOServeActions.Where(x => vos.Any(z => z.Id == x.PrintDocumentValuedOutcomeId));
                    if (actions.Count() > 0)
                    {
                        _context.ConsumerPrintDocumentVOServeActions.RemoveRange(actions);
                    }
                    _context.ConsumerPrintDocumentValuedOutcomes.RemoveRange(vos);
                }
                _context.ConsumerPrintDocuments.Remove(model);
                await _context.SaveChangesAsync();
            }
        }

        public IEnumerable<object> GenerateDataSetForDocumentationRecord(DocumentPrintTypeEnum DocumentType,ConsumerPrintDocument document, bool isEmpty)
        {

            ReportDataSetModel dataSet = new ReportDataSetModel();
            
            if (document != null)
            {
                string title = GetTitle(document);
                List<String> valued = GetValued(document);
                DateTime? lastReviewDate = null;
                lastReviewDate = document.Consumer.ConsumerHabReviews
                                .Where(x => x.ServiceId == document.ServiceTypeId)
                                .OrderByDescending(x => x.DateReview)
                                .Select(x => x.DateReview).FirstOrDefault();

                ReportAddressModel currentAddress = getCurrentAddress(document.Consumer, lastReviewDate);

                dataSet.LastReviewDate = lastReviewDate.ToDateString();
                dataSet.PatientName = this.GetConsumerName(document.Consumer);
                dataSet.EffectiveDate = document.EffectiveDate.ToDateString2();
                dataSet.Address = this.GetAddress(currentAddress);
                dataSet.ServiceDescription = this.GetServices(document);
                dataSet.Medicaid = this.GetMedicaidNumberByDate(document.Consumer, document.EffectiveDate);// document.Consumer.MedicaidNo;
                dataSet.Title = title;
                dataSet.ReportHeader = ITCraftFrame.EnumUtils.ToDescriptionString(DocumentType);
                dataSet.ValuedOutcome1 = valued[0];
                dataSet.ValuedOutcome2 = valued[1];
                dataSet.ValuedOutcome3 = valued[2];
                dataSet.ValuedOutcome4 = valued[3];
                dataSet.ValuedOutcomeFormated = string.Join(" ", valued[0], valued[1], valued[2], valued[3]);

                if (!isEmpty)
                {
                    dataSet.ContactName = this.GetContactName(document.Contact);
                    dataSet.JobTitle = document.Contact.JobTitle;
                    dataSet.WorkerAddress = this.GetAddress(document.Contact);
                    dataSet.Rate = GetRate(document);
                    dataSet.ConsumerSityStateZip = GetSityStateZip(currentAddress);
                    dataSet.ContactSityStateZip = GetContactSityStateZip(document.Contact);

                }
            }
            return new [] { dataSet  };
        }

       

        public IEnumerable<object> GenerateDataSetForMonthlyProgressSummary(DocumentPrintTypeEnum  DocumentType, ConsumerPrintDocument document, bool isEmpty)
        {
            ReportDataSetModel dataSet = new ReportDataSetModel();
            
            if (document != null)
            {

                List<String> actions = GetActions(document);
                List<String> valued = GetValued(document);
                dataSet.ReportHeader = ITCraftFrame.EnumUtils.ToDescriptionString(DocumentType);
                dataSet.PatientName = this.GetConsumerName(document.Consumer);
                dataSet.Medicaid = this.GetMedicaidNumberByDate(document.Consumer, document.EffectiveDate);// document.Consumer.MedicaidNo;
                dataSet.ServiceAction1 = actions[0];
                dataSet.ServiceAction2 = actions[1];
                dataSet.ServiceAction3 = actions[2];
                dataSet.ServiceAction4 = actions[3];
                dataSet.ValuedOutcome1 = valued[0];
                dataSet.ValuedOutcome2 = valued[1];
                dataSet.ValuedOutcome3 = valued[2];
                dataSet.ValuedOutcome4 = valued[3];
                dataSet.ValuedOutcomeFormated = string.Join(" ", valued[0], valued[1], valued[2], valued[3]);

                if (!isEmpty)
                {
                    dataSet.EffectiveDate = document.EffectiveDate.HasValue ? document.EffectiveDate.Value.ToString("MMMM yyyy", CultureInfo.InvariantCulture) : "";
                    dataSet.ContactName = this.GetContactName(document.Contact);
                }
            }
            return new[] { dataSet };
        }


        private string GetTitle(ConsumerPrintDocument document)
        {
            string title = (string.IsNullOrEmpty(document.ServiceTypeTitle) ? "Respite" : document.ServiceTypeTitle); 
            return title.ToUpper();
        }

        private string GetRate(ConsumerPrintDocument document)
        {
            var employee = _context.ConsumerEmployees
                       .FirstOrDefault(x => x.ConsumerId == document.ConsumerId
                                    && x.ContactId == document.ContactId);
            String rate = String.Empty;
            if (employee != null)
            {
                if (employee.Rate.HasValue)
                {
                    rate = String.Format("{0:C2}", employee.Rate);
                }
            }
            return rate;
        }

        public String GetMedicaidNumberByDate(Database.Consumer consumer, DateTime? processingDate)
        {
            var result =
                ConsumerMedicaidNumberManagement.GetMedicaidNumberByDate(_context, processingDate, consumer.ConsumerId);

            return result?.MedicaidNo ?? consumer.MedicaidNo;
        }

    }
}
