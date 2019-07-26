using ITCraftFrame;
using ROHV.Core.Database;
using ROHV.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ROHV.WebApi.ViewModels
{
    public class ConsumerFullViewModel : ConsumerModel
    {
        public class Contact
        {
            public Contact() { }
            public Contact(Int32? id, Core.Database.Contact contact)
            {
                this.Id = id;
                if (contact != null)
                {
                    this.Name = contact.LastName + ", " + contact.FirstName;
                    if (contact.AgencyNameId.HasValue)
                    {
                        this.Company = contact.Agency.NameCompany;
                    }
                }
            }
            public Int32? Id { get; set; }
            public String Company { get; set; }
            public String Name { get; set; }
        }

        public Contact ServiceCoordinator { get; set; }
        public Contact MSC { get; set; }
        public Contact CH { get; set; }
        public Contact DH { get; set; }

        public List<ConsumerPhonesViewModel> Phones { get; set; }
        public List<ConsumerEmployeeViewModel> Employees { get; set; }
        public List<ConsumerServiceViewModel> ApprovedServices { get; set; }
        public List<ConsumerDocumentViewModel> Documents { get; set; }
        public List<ConsumerHabPlanViewModel> HabPlans { get; set; }
        public List<ConsumerHabReviewViewModel> HabReviews { get; set; }
        public List<ConsumerPrintDocumentViewModel> PrintDocuments { get; set; }
        public List<ConsumerCallLogsViewModel> CallLogs { get; set; }
        public List<ConsumerNotesViewModel> Notes { get; set; }
        public List<ConsumerNotificationSettingsViewModel> Notifications { get; set; }

        public List<ConsumerServiceCoordinatorViewModel> ServiceCoordinators { set; get; }
        public List<ConsumerAddressModelView> Addresses { set; get; }
        public List<ConsumerMedicaidNumberViewModel> MedicaidNumbers { get; set; }
        public List<FileMetaDataModelView> UploadFiles { get; set; }

        public ConsumerFullViewModel()
        {

            Phones = new List<ConsumerPhonesViewModel>();
            Employees = new List<ConsumerEmployeeViewModel>();
            ApprovedServices = new List<ConsumerServiceViewModel>();
            Documents = new List<ConsumerDocumentViewModel>();
            HabPlans = new List<ConsumerHabPlanViewModel>();
            HabReviews = new List<ConsumerHabReviewViewModel>();
            PrintDocuments = new List<ConsumerPrintDocumentViewModel>();
            CallLogs = new List<ConsumerCallLogsViewModel>();
            Notes = new List<ConsumerNotesViewModel>();
            Notifications = new List<ConsumerNotificationSettingsViewModel>();
            UploadFiles = new List<FileMetaDataModelView>();
        }
        public ConsumerFullViewModel(Consumer model)
        {
            CustomMapper.MapEntity(model, this);

            if (model.Advocate != null)
            {
                this.AdvocateName = model.Advocate.LastName + ", " + model.Advocate.FirstName;
            }

            if (model.AdvocatePaper != null)
            {
                this.AdvocatePaperName = model.AdvocatePaper.LastName + ", " + model.AdvocatePaper.FirstName;
            }

            if (this.Status.HasValue)
            {
                this.StatusName = model.List.ListDescription;
            }

            this.DateOfBirth = (model.DateOfBirth != null ? DateTime.SpecifyKind(model.DateOfBirth.Value, DateTimeKind.Unspecified) : model.DateOfBirth);

            this.HasServiceCoordinator = model.HasServiceCoordinator.HasValue ? model.HasServiceCoordinator.Value : false;

            this.CH = new Contact(model.CHCoordinatorId, model.CHCoordinator);
            this.DH = new Contact(model.DHCoordinatorId, model.DHCoordinator);
            this.MSC = new Contact(model.MSCId, model.MSCoordinator);
            this.ServiceCoordinator = new Contact(model.ServiceCoordinatorId, model.ServiceCoordinatorContact);

            this.ServiceCoordinators = CustomMapper.MapList<ConsumerServiceCoordinatorViewModel, ConsumerServiceCoordinator>(model.ConsumerServiceCoordinators.ToList());
            this.Addresses = CustomMapper.MapList<ConsumerAddressModelView, ConsumerAddress>(model.ConsumerAddresses.ToList());
            this.MedicaidNumbers = CustomMapper.MapList<ConsumerMedicaidNumberViewModel, ConsumerMedicaidNumber>(model.ConsumerMedicaidNumbers.ToList());
        }

        public void SetPhones(List<ConsumerPhone> phones)
        {
            this.Phones = ConsumerPhonesViewModel.GetList(phones);
        }
        public void SetEmployees(List<ConsumerEmployee> employees)
        {
            this.Employees = ConsumerEmployeeViewModel.GetList(employees);
        }

        public void SetApprovedServices(List<ConsumerService> services)
        {
            this.ApprovedServices = ConsumerServiceViewModel.GetList(services);
            if (this.Employees.Count > 0)
            {
                List<String> dWorkers = new List<string>();
                foreach (var item in this.ApprovedServices)
                {
                    foreach (var employee in this.Employees)
                    {
                        if (item.ServiceId == employee.ServiceId)
                        {
                            dWorkers.Add(employee.ContactName);
                        }
                    }
                    item.DWorkers = String.Join(", ", dWorkers);
                    dWorkers.Clear();
                }
            }
        }

        public void SetDocuments(List<EmployeeDocument> documents)
        {
            this.Documents = ConsumerDocumentViewModel.GetList(documents);
        }

        public void SetHabPlans(List<ConsumerHabPlan> habPlans)
        {
            this.HabPlans = ConsumerHabPlanViewModel.GetList(habPlans);
        }

        public void SetHabReviews(List<ConsumerHabReview> habReviews)
        {
            this.HabReviews = ConsumerHabReviewViewModel.GetList(habReviews);
        }

        public void SetPrintDocuments(List<ConsumerPrintDocument> printDocuments)
        {
            this.PrintDocuments = ConsumerPrintDocumentViewModel.GetList(printDocuments);
        }

        public Consumer GetRateData()
        {
            Consumer model = new Consumer();
            model.ConsumerId = this.ConsumerId.Value;
            model.Rate = this.Rate;
            model.MaxHoursPerWeek = this.MaxHoursPerWeek;
            model.MaxHoursPerYear = this.MaxHoursPerYear;
            model.RateNote = this.RateNote;

            return model;

        }
        public Consumer GetDHData()
        {
            Consumer model = new Consumer();
            model.ConsumerId = this.ConsumerId.Value;
            model.CHCoordinatorId = this.CH.Id;
            model.DHCoordinatorId = this.DH.Id;
            model.MSCId = this.MSC.Id;

            return model;

        }

        public void SetUploadFiles(List<FileMetaDataModel> uploadFiles)
        {
            this.UploadFiles = CustomMapper.MapList<FileMetaDataModelView, FileMetaDataModel>(uploadFiles);
        }

        public void SetCallLogs(List<ConsumerContactCall> models)
        {
            this.CallLogs = ConsumerCallLogsViewModel.GetList(models);
        }

        public void SetNotes(List<ConsumerNote> models)
        {
            this.Notes = ConsumerNotesViewModel.GetList(models);
        }

        public void SetNotifications(List<ConsumerNotificationSetting> models)
        {
            this.Notifications = ConsumerNotificationSettingsViewModel.GetList(models);
            if (this.Notifications.Count > 0)
            {
                List<String> recipients = new List<string>();
                foreach (var item in this.Notifications)
                {
                    foreach (var recipient in item.Recipients)
                    {
                        if (!String.IsNullOrEmpty(recipient.Name))
                        {
                            recipients.Add(recipient.Name);
                        }
                        else
                        {
                            recipients.Add(recipient.Email);
                        }
                    }
                    item.RecipientsString = String.Join(", ", recipients);
                }
            }
        }

        public Consumer GetModel()
        {
            Consumer model = new Consumer();

            CustomMapper.MapEntity(this, model);

            model.MI = this.MiddleName;
            model.ConsumerId = this.ConsumerId ?? 0;

            if (this.ServiceCoordinator != null)
            {
                model.ServiceCoordinatorId = this.ServiceCoordinator.Id;
            }
            if (this.DH != null)
            {
                model.DHCoordinatorId = this.DH.Id;
            }
            if (this.CH != null)
            {
                model.CHCoordinatorId = this.CH.Id;
            }

            if (this.MSC != null)
            {
                model.MSCId = this.MSC.Id;
            }


            return model;
        }

        public List<ConsumerPhone> GetPhonesModel()
        {
            List<ConsumerPhone> phones = new List<ConsumerPhone>();
            foreach (var phone in Phones)
            {
                phones.Add(phone.GetModel());
            }
            return phones;
        }
    }
}