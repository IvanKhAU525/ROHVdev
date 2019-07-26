using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Models
{
    public class FileMetaDataModel
    {
        public UserModel AddedByUser { set; get; }
        public UserModel UpdatedByUser { set; get; }

        public int Id { get; set; }
        public int ParentEntityId { get; set; }
        public int ParentEntityTypeId { get; set; }
        public int AddedById { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string FilePath { get; set; }
        public string FileContentType { get; set; }
        public string FileDisplayName { get; set; }
        public string Note { get; set; }

    }
}
