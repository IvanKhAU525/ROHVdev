using ROHV.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ROHV.WebApi.ViewModels
{
    public class FileMetaDataModelView: FileMetaDataModel
    {
        public string AddedByView { get => $"{AddedByUser?.FirstName} {AddedByUser?.LastName}"; }
        public string UpdatedByView { get => $"{UpdatedByUser?.FirstName} {UpdatedByUser?.LastName}"; }
        public string DownloadId { get => string.IsNullOrEmpty(FilePath) ? "" : Id.ToString(); }
        public string FileData { set; get; }
    }
}