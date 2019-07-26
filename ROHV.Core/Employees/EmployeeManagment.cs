using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using ROHV.Core.Database;
using ROHV.Core.Services;
using System.Data.Entity;
using ROHV.Core.DatatableUtils;
using ROHV.Controllers;
using Microsoft.Reporting.WebForms;

namespace ROHV.Core.Employees
{
    public class EmployeeManagment : BaseModel
    {
        public EmployeeManagment(RayimContext context) : base(context) { }        
        public async Task<List<Contact>> GetAllContacts()
        {
            var result = await _context.Contacts.Where(x => !x.IsDeleted).ToListAsync();
            return result;
        }

        public async Task<DTResult<EmployeeModel>> GetList(DTParameters gridParrams)
        {
            DTColumn columnName = gridParrams.Columns.SingleOrDefault(x => x.Data == "Name");            

            String globaSearch = gridParrams.Search.Value;
            var result = from item in _context.Contacts
                         where !item.IsDeleted                       
                         select item;
                        
            if (!String.IsNullOrEmpty(columnName.Search.Value))
            {
                result = from item in result
                         where item.LastName.StartsWith(columnName.Search.Value) ||
                                item.FirstName.StartsWith(columnName.Search.Value)
                         select item;
            }           
            foreach (var order in gridParrams.Order)
            {
                DTColumn columnOrder = gridParrams.Columns[order.Column];
                switch (columnOrder.Data)
                {
                    case "Name":
                        {
                            if (order.Dir == DTOrderDir.ASC)
                            {
                                result = result.OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
                            }
                            else
                            {
                                result = result.OrderByDescending(x => x.LastName).ThenByDescending(x=>x.FirstName);
                            }
                            break;
                        }
                    case "CompanyName":
                        {
                            if (order.Dir == DTOrderDir.ASC)
                            {
                                result = result.OrderBy(x => x.CompanyName);
                            }
                            else
                            {
                                result = result.OrderByDescending(x => x.CompanyName);
                            }
                            break;
                        }
                    case "Address":
                        {
                            if (order.Dir == DTOrderDir.ASC)
                            {
                                result = result.OrderBy(x => x.Address1);
                            }
                            else
                            {
                                result = result.OrderByDescending(x => x.Address1);
                            }
                            break;
                        }
                    case "City":
                        {
                            if (order.Dir == DTOrderDir.ASC)
                            {
                                result = result.OrderBy(x => x.City);
                            }
                            else
                            {
                                result = result.OrderByDescending(x => x.City);
                            }
                            break;
                        }
                    case "State":
                        {
                            if (order.Dir == DTOrderDir.ASC)
                            {
                                result = result.OrderBy(x => x.State);
                            }
                            else
                            {
                                result = result.OrderByDescending(x => x.State);
                            }
                            break;
                        }
                    case "ContactType":
                        {
                            if (order.Dir == DTOrderDir.ASC)
                            {
                                result = result.OrderBy(x => (x.ContactType!=null?x.ContactType.Name:""));
                            }
                            else
                            {
                                result = result.OrderByDescending(x => (x.ContactType != null ? x.ContactType.Name : ""));
                            }
                            break;
                        }
                    case "JobTitle":
                        {
                            if (order.Dir == DTOrderDir.ASC)
                            {
                                result = result.OrderBy(x => x.JobTitle);
                            }
                            else
                            {
                                result = result.OrderByDescending(x => x.JobTitle);
                            }
                            break;
                        }
                    case "Phone":
                        {
                            if (order.Dir == DTOrderDir.ASC)
                            {
                                result = result.OrderBy(x => x.Phone);
                            }
                            else
                            {
                                result = result.OrderByDescending(x => x.Phone);
                            }
                            break;
                        }
                    case "EmailAddress":
                        {
                            if (order.Dir == DTOrderDir.ASC)
                            {
                                result = result.OrderBy(x => x.EmailAddress);
                            }
                            else
                            {
                                result = result.OrderByDescending(x => x.EmailAddress);
                            }
                            break;
                        }
                }
            }
            Int32 count = result.Count();
            var list = await result.Skip(gridParrams.Start).Take(gridParrams.Length).ToListAsync();
            List<EmployeeModel> models = new List<EmployeeModel>();
            foreach (var item in list)
            {
                EmployeeModel model = new EmployeeModel();
                model.ContactId = item.ContactId.ToString();
                model.FirstName = item.FirstName;
                model.LastName = item.LastName;                
                model.CompanyName = item.CompanyName;
                model.Address1 = item.Address1;
                model.Address2 = item.Address2;                
                model.City = item.City;
                model.State = item.State;
                model.ContactType = item.ContactTypeId.HasValue ? item.ContactType.Name : "";
                model.JobTitle = item.JobTitle;
                model.Phone = item.Phone;
                model.MobilePhone = item.MobilePhone;
                model.EmailAddress = item.EmailAddress;
                model.Action = "";
                models.Add(model);
            }

            DTResult<EmployeeModel> resultGrid = new DTResult<EmployeeModel>
            {
                draw = gridParrams.Draw,
                data = models,
                recordsFiltered = count,
                recordsTotal = count
            };
            return resultGrid;

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
        public byte[] GetPDF(int contactId, BaseController controller, out string name)
        {
            Contact contact = _context.Contacts.SingleOrDefault(x => x.ContactId == contactId);

            name = Utils.ConvertInvalidFilePathChars(this.GetName(contact));            
            if (contact == null) return null;

            var reportViewer = new ReportViewer();
            reportViewer.Reset();
            reportViewer.LocalReport.EnableExternalImages = true;
            reportViewer.ProcessingMode = ProcessingMode.Local;

            reportViewer.LocalReport.ReportPath = controller.Server.MapPath("~/Views/PDFViews/PrintLabel.rdl");            
            IEnumerable<Object> timeObjs = null;            
            timeObjs = new[]
            {
                new
                {
                    ContactName = this.GetName(contact),
                    Address1= contact.Address1 +(String.IsNullOrEmpty(contact.Address2)?"":", "+contact.Address2),
                    Address2 = contact.City+", "+contact.State+" "+contact.Zip                
                }
            };
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", timeObjs));
            return Utils.RenderPDF(reportViewer);
        }

        public async Task<Contact> GetContact(Int32 id)
        {
            var result = await _context.Contacts.SingleOrDefaultAsync(x => x.ContactId == id);
            return result;
        }

        public async  Task<List<Department>> GetDepts()
        {
            var result = await _context.Departments.ToListAsync();
            return result;
        }
        public async Task<List<List>> GetCategories()
        {
            var result = await _context.Lists.Where(x=>x.ListCategoryId==2).OrderBy(x=>x.ListDescription).ToListAsync();
            return result;
        }

        public async Task<List<ContactType>> GetTypes()
        {
            var result = await _context.ContactTypes.ToListAsync();
            return result;
        }

        public async Task<List<State>> GetStates()
        {
            var states = await _context.States.OrderBy(x => x.SortingPlace).ToListAsync();
            return states;
        }

        public async Task<Boolean> IsExistWithTheSameEmail(Int32 contactId, string email)
        {
            if(String.IsNullOrEmpty(email))
            {
                return false;
            }
            if (contactId == -1)
            {
                var result = await _context.Contacts.FirstOrDefaultAsync(x => x.EmailAddress.ToLower() == email.ToLower());
                if (result != null)
                {
                    return true;
                }
            }
            else
            {
                var result = await _context.Contacts.FirstOrDefaultAsync(x => x.EmailAddress.ToLower() == email.ToLower() && x.ContactId != contactId);
                if (result != null)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<Int32> SaveContact(Contact dbModel)
        {
            if (dbModel.ContactId== 0)
            {
                _context.Contacts.Add(dbModel);
            }
            else
            {
                var model = await _context.Contacts.SingleOrDefaultAsync(x => x.ContactId == dbModel.ContactId);
                ITCraftFrame.CustomMapper.MapEntity(dbModel, model);
            }
            await _context.SaveChangesAsync();
            return dbModel.ContactId;
        }
     
        public async Task<Boolean> DeleteContact(Int32 id)
        {
            var contact = await _context.Contacts.SingleOrDefaultAsync(x => x.ContactId == id);
            if (contact == null)
            {
                return false;
            }
            contact.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
