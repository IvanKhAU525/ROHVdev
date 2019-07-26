using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Models
{
    public class ContactModel
    {
        public Int32 ContactId { get; set; }
        public Int32? CategoryId { get; set; }
        public String CompanyName { get; set; }
        public String Salutation { get; set; }
        public String FirstName { get; set; }
        public String MiddleName { get; set; }
        public String LastName { get; set; }
        public String Address1 { get; set; }
        public String Address2 { get; set; }
        public String City { get; set; }
        public String State { get; set; }
        public String Zip { get; set; }
        public Boolean? IsServiceCoordinator { get; set; }
        public String JobTitle { get; set; }
        public String EmailAddress { get; set; }
        public String Notes { get; set; }
        public Int32? AgencyNameId { get; set; }
        public Int32? DepartmentId { get; set; }
        public int NoteFromTypeId { set; get; }
        public Int32? ContactTypeId { get; set; }        
        public String Phone { get; set; }
        public String CCO { get; set; }

        public String MobilePhone { get; set; }

        public Boolean IsUpdate { get; set; }
        public String Signature { get; set; }
        public string FileNumber { get; set; }
        public string PhoneExtension { get; set; }
        public string Fax { get; set; }
    }
}
