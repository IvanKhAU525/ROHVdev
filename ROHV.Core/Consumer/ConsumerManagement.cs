using ROHV.Core.Database;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System;
using ROHV.Core.Models;
using ROHV.Core.Services;
using ROHV.Models.Services;

namespace ROHV.Core.Consumer
{
    public class ConsumerManagement : BaseModel
    {
        public ConsumerManagement(RayimContext context) : base(context) { }
        public async Task<List<Contact>> GetEmployees(string search, bool? skipNotAssigned)
        {
            IQueryable<Database.Contact> contacts = _getContactsCollection(skipNotAssigned);

            if (!String.IsNullOrEmpty(search))
            {

                List<String> searchArr = search.Split(',').ToList();
                String tmp1 = searchArr[0].Trim();
                contacts = contacts.Where(x => x.LastName.StartsWith(tmp1));
                if (searchArr.Count > 1)
                {

                    String tmp2 = searchArr[1].Trim();
                    if (!String.IsNullOrEmpty(tmp2))
                    {
                        contacts = contacts.Where(x => x.FirstName.StartsWith(tmp2));
                    }
                }
            }
            var contactsReturn = await contacts.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToListAsync();
            return contactsReturn;
        }

        private IQueryable<Contact> _getContactsCollection(bool? skipNotAssigned)
        {
            return skipNotAssigned.HasValue && skipNotAssigned == true ?
                from contact in _context.Contacts
                join contactWithConsumer in _context.ConsumerEmployees
                on contact.ContactId equals contactWithConsumer.ContactId
                select contact : _context.Contacts;
        }

        public async Task<List<Contact>> GetEmployeesByConsumer(string search, Int32 consumerId)
        {
            IQueryable<Database.Contact> contacts = _context.Contacts.Where(x => x.ConsumerEmployees.Any(z => z.ConsumerId == consumerId && x.ContactId == z.ContactId));
            if (!String.IsNullOrEmpty(search))
            {

                List<String> searchArr = search.Split(',').ToList();
                String tmp1 = searchArr[0].Trim();
                contacts = contacts.Where(x => x.LastName.StartsWith(tmp1));
                if (searchArr.Count > 1)
                {

                    String tmp2 = searchArr[1].Trim();
                    if (!String.IsNullOrEmpty(tmp2))
                    {
                        contacts = contacts.Where(x => x.FirstName.StartsWith(tmp2));
                    }
                }
            }
            var contactsReturn = await contacts.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToListAsync();
            return contactsReturn;
        }


        public async Task<List<Database.Consumer>> GetConsumers(string search, int? employeeId, int? consumerId)
        {
            if (consumerId.HasValue)
            {
                return await _context.Consumers.Where(x => x.ConsumerId == consumerId).OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToListAsync();
            }
            IQueryable<Database.Consumer> consumers = _context.Consumers;
            if (!String.IsNullOrEmpty(search))
            {

                List<String> searchArr = search.Split(',').ToList();
                String tmp1 = searchArr[0].Trim();
                consumers = consumers.Where(x => x.LastName.StartsWith(tmp1));
                if (searchArr.Count > 1)
                {

                    String tmp2 = searchArr[1].Trim();
                    if (!String.IsNullOrEmpty(tmp2))
                    {
                        consumers = consumers.Where(x => x.FirstName.StartsWith(tmp2));
                    }
                }
            }

            if (employeeId.HasValue)
            {
                consumers = from item in consumers
                            join contact in _context.ConsumerEmployees on item.ConsumerId equals contact.ConsumerId
                            where contact.ContactId == employeeId.Value
                            select item;
            }
            return await consumers.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToListAsync();
        }

        public async Task<List<Core.Database.List>> GetList(string listName)
        {
            return await _context.Lists.Where(x => x.ListCategory.Description == listName).OrderBy(x => x.ListId).ToListAsync();
        }
        public async Task<List<Core.Database.ServicesList>> GetServiceList()
        {
            return await _context.ServicesLists.OrderBy(x => x.ServiceDescription).ToListAsync();
        }

        public async Task<List<Database.Agency>> GetAgencyNamesList()
        {
            List<Database.Agency> result = await _context.Agencies.OrderBy(x => x.NameCompany).ToListAsync();
            return result;
        }

        public async Task<List<Database.Consumer>> GetConsumersByMedicaid(string search)
        {
            var consumers = _context.Consumers.Where(x => x.MedicaidNo.StartsWith(search));
            return await consumers.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToListAsync();
        }
        public async Task<List<Database.Consumer>> GetConsumersByTabsId(string search)
        {
            var consumers = _context.Consumers.Where(x => x.TABSNo.StartsWith(search));
            return await consumers.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToListAsync();
        }
        
        public async Task<List<Contact>> GetServiceCoordinatorList(string search, Int32? agencyId)
        {
            IQueryable<Database.Contact> contacts = _context.Contacts;
            if (!String.IsNullOrEmpty(search))
            {
                List<String> searchArr = search.Split(',').ToList();
                String tmp1 = searchArr[0].Trim();
                contacts = contacts.Where(x => x.LastName.StartsWith(tmp1));
                if (searchArr.Count > 1)
                {

                    String tmp2 = searchArr[1].Trim();
                    if (!String.IsNullOrEmpty(tmp2))
                    {
                        contacts = contacts.Where(x => x.FirstName.StartsWith(tmp2));
                    }
                }
            }
            var result = await contacts.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToListAsync();
            return result;
        }

        public async Task<IEnumerable<ServiceCoordinatorSearchModel>> GetServiceCoordinators(string query)
        {
            var contacts = _context.Contacts.AsQueryable();
            var consumers = _context.Consumers.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).AsQueryable();

            FilterServiceCoordinatorsByName(ref contacts, query);

            var filteredContacts = await contacts.ToListAsync();
            var filteredConsumers = await consumers.ToListAsync();

            var result = filteredContacts.Join(filteredConsumers,
                x => x.ContactId,
                y => y.ServiceCoordinatorId,
                (contact, consumer) => new ServiceCoordinatorSearchModel
                {
                    ConsumerId = consumer.ConsumerId,
                    ConsumerFirstName = consumer.FirstName,
                    ConsumerLastName = consumer.LastName,
                    ServiceCoordinatorFirstName = contact.FirstName,
                    ServiceCoordinatorLastName = contact.LastName
                })
                .OrderBy(x => x.ServiceCoordinatorLastName)
                .ThenBy(x => x.ServiceCoordinatorFirstName).ToList();

            return result;
        }

        private void FilterServiceCoordinatorsByName(ref IQueryable<Contact> contacts, string filter)
        {
            if (!String.IsNullOrEmpty(filter))
            {
                var search = filter.Split(',');
                var lastNameSearch = search[0].Trim();
                contacts = contacts.Where(x => x.LastName.StartsWith(lastNameSearch) || x.FirstName.StartsWith(lastNameSearch));

                if (search.Length > 1)
                {
                    var firstNameSearch = search[1].Trim();
                    if (!String.IsNullOrEmpty(firstNameSearch))
                    {
                        contacts = contacts.Where(x => x.FirstName.StartsWith(firstNameSearch) || x.LastName.StartsWith(firstNameSearch));
                    }
                }
            }
        }

        public async Task<List<Contact>> GetAdvocatesList(string search)
        {
            IQueryable<Database.Contact> advocates = _context.Contacts.Where(x => x.ContactTypeId == 7);
            if (!String.IsNullOrEmpty(search))
            {
                List<String> searchArr = search.Split(',').ToList();
                String tmp1 = searchArr[0].Trim().ToLower();
                advocates = advocates.Where(x => x.LastName.ToLower().StartsWith(tmp1));
                if (searchArr.Count > 1)
                {

                    String tmp2 = searchArr[1].Trim();
                    if (!String.IsNullOrEmpty(tmp2))
                    {
                        advocates = advocates.Where(x => x.FirstName.StartsWith(tmp2));
                    }
                }
            }
            var result = await advocates.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToListAsync();
            return result;
        }

        public async Task<Core.Database.Consumer> GetConsumer(Int32 id)
        {
            var consumer = await _context.Consumers.SingleOrDefaultAsync(x => x.ConsumerId == id);
            if (consumer != null)
            {
                if (consumer.ServiceCoordinatorId.HasValue)
                {
                    var failRetrive = consumer.CHCoordinator;
                }
            }
            return consumer;
        }

        public async Task<Boolean> SaveRate(Database.Consumer model)
        {
            var data = await _context.Consumers.SingleOrDefaultAsync(x => x.ConsumerId == model.ConsumerId);
            if (data != null)
            {
                data.Rate = model.Rate;
                data.MaxHoursPerWeek = model.MaxHoursPerWeek;
                data.MaxHoursPerYear = model.MaxHoursPerYear;
                data.RateNote = model.RateNote;
                await _context.SaveChangesAsync();
                return true;

            }
            return false;
        }

        public async Task<List<string>> ValidateConsumerModel(Database.Consumer model)
        {
            List<string> errors = new List<string>();

            if (!await ValidateMedicalNumber(model))
            {
                errors.Add("Medical numer is not valid");
            }

            return errors;
        }

        private async Task<bool> ValidateMedicalNumber(Database.Consumer model)
        {
            var lowCaseMedicalNumber = model.MedicaidNo.ToLower().Trim();
            bool findAnyConsumer = await _context.Consumers.AnyAsync(x => (x.ConsumerId != model.ConsumerId) && x.MedicaidNo.ToLower() == lowCaseMedicalNumber);
            return !findAnyConsumer;
        }

        public async Task<Int32> Save(Database.Consumer dbModel)
        {
            if (dbModel.ConsumerId == 0)
            {
                _context.Consumers.Add(dbModel);
            }
            else
            {
                var model = await _context.Consumers.SingleOrDefaultAsync(x => x.ConsumerId == dbModel.ConsumerId);
                if (model != null)
                {
                    ITCraftFrame.CustomMapper.MapEntity(dbModel, model);
                }
                if (string.IsNullOrEmpty(model.LastName))
                {
                    Logger.LogError($" record with id has been set LastName to null {model.ConsumerId}");
                }
            }
            await _context.SaveChangesAsync();

            return dbModel.ConsumerId;
        }

        public async Task Delete(Int32 id)
        {
            var model = await _context.Consumers.SingleOrDefaultAsync(x => x.ConsumerId == id);
            if (model != null)
            {
                await (new ConsumerCallLogsManagement(_context)).DeleteAll(id);
                await (new ConsumerHabPlansManagement(_context)).DeleteAll(id);
                await (new ConsumerHabReviewsManagement(_context)).DeleteAll(id);
                await (new ConsumerNotesManagement(_context)).DeleteAll(id);
                await (new ConsumerNotificationsManagement(_context)).DeleteAll(id);
                await (new ConsumerPhonesManagement(_context)).DeleteAll(id);
                await (new ConsumerPrintDocumentsManagement(_context)).DeleteAll(id);
                await (new ConsumerServicesManagement(_context)).DeleteAll(id);
                await (new ConsumerEmployeeManagement(_context)).DeleteAll(id);
                _context.Consumers.Remove(model);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsValidSignaturePassword(int contactId, string password)
        {
            var model = await _context.Contacts.SingleOrDefaultAsync(x => x.ContactId == contactId);
            if (model != null)
            {
                if ((model.SignaturePassword ?? "") == password)
                {
                    return true;
                }
            }
            return false;
        }

        public   ContactModel GetContactByEmail(string email)
        {
            Contact dbModel =  _context.Contacts.FirstOrDefault(x => x.EmailAddress == email);
            var result = ITCraftFrame.CustomMapper.MapEntity<ContactModel>(dbModel);
            if (dbModel != null)
            {
                result.NoteFromTypeId = dbModel.Department != null ?
                    _context.ConsumerNoteFromTypes.Where(x => x.Name == dbModel.Department.Name).Select(x => x.Id).FirstOrDefault() : 1;
            }
            return result;
        }
    }
}
