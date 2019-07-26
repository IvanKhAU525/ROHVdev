using ROHV.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ROHV.WebApi.ViewModels
{
    public class ConsumerEmailViewModel
    {
        public Int32 ContactId { get; set; }
        public String ContactName { get; set; }
        public String Email { get; set; }
        public String Message { get; set; }
        public String Subject { get; set; }

    }
}