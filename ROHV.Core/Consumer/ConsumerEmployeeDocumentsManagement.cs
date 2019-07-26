using ROHV.Core.Database;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System;
using ROHV.Core.Services;
using ROHV.Controllers;
using System.IO;
using ROHV.Core.Models;


namespace ROHV.Core.Consumer
{
    public class ConsumerEmployeeDocumentsManagement : BaseModel
    {
        public ConsumerEmployeeDocumentsManagement(RayimContext context) : base(context) { }
        public async Task<List<EmployeeDocument>> GetDocuments(Int32 consumerId)
        {
            var documents = await _context.EmployeeDocuments.Where(x => x.ConsumerId == consumerId).OrderBy(x => x.DateUpdated).ThenBy(x => x.DateCreated).ToListAsync();
            return documents;
        }

        public async Task<List<EmployeeDocumentTypeModel>> GetDocTypes()
        {
            var result = await _context.EmployeeDocumentTypes.ToListAsync();
            return ITCraftFrame.CustomMapper.MapList<EmployeeDocumentTypeModel, EmployeeDocumentType>(result);
        }

        public async Task<List<EmployeeDocumentStatus>> GetDocStatuses()
        {
            var result = await _context.EmployeeDocumentStatuses.ToListAsync();
            return result;
        }
        private Dictionary<String, String> GetFileExtention()
        {
            Dictionary<String, String> result = new Dictionary<string, string>();
            result.Add("application/mac-binhex40", "hqx");
            result.Add("application/mac-compactpro", "cpt");
            result.Add("text/x-comma-separated-values", "csv");
            result.Add("text/comma-separated-values", "csv");
            result.Add("text/csv", "csv");
            result.Add("application/csv", "csv");
            result.Add("application/macbinary", "bin");
            result.Add("application/x-photoshop", "psd");
            result.Add("application/octet-stream", "dll");
            result.Add("application/oda", "oda");
            result.Add("application/pdf", "pdf");
            result.Add("application/x-download", "pdf");
            result.Add("application/postscript", "ai");
            result.Add("application/smil", "smi");
            result.Add("application/vnd.mif", "mif");
            result.Add("application/excel", "xls");
            result.Add("application/vnd.ms-excel", "xls");
            result.Add("application/msexcel", "xls");
            result.Add("application/powerpoint", "ppt");
            result.Add("application/vnd.ms-powerpoint", "ppt");
            result.Add("application/wbxml", "wbxml");
            result.Add("application/wmlc", "wmlc");
            result.Add("application/x-director", "dir");
            result.Add("application/x-dvi", "dvi");
            result.Add("application/x-gtar", "gtar");
            result.Add("application/x-gzip", "gz");
            result.Add("application/x-httpd-php", "php");
            result.Add("application/x-httpd-php-source", "phps");
            result.Add("application/x-javascript", "js");
            result.Add("application/x-shockwave-flash", "swf");
            result.Add("application/x-stuffit", "sit");
            result.Add("application/x-tar", "tar");
            result.Add("application/xhtml+xml", "xhtml");
            result.Add("application/x-zip", "zip");
            result.Add("application/zip", "zip");
            result.Add("application/x-zip-compressed", "zip");
            result.Add("audio/midi", "midi");
            result.Add("audio/mpeg", "mp3");
            result.Add("audio/mpg", "mp3");
            result.Add("audio/x-aiff", "aif");
            result.Add("audio/x-pn-realaudio", "ram");
            result.Add("audio/x-pn-realaudio-plugin", "rpm");
            result.Add("audio/x-realaudio", "ra");
            result.Add("video/vnd.rn-realvideo", "rv");
            result.Add("audio/x-wav", "wav");
            result.Add("image/bmp", "bmp");
            result.Add("image/gif", "gif");
            result.Add("image/jpeg", "jpg");
            result.Add("image/pjpeg", "jpg");
            result.Add("image/png", "png");
            result.Add("image/x-png", "png");
            result.Add("image/tiff", "tiff");
            result.Add("text/css", "css");
            result.Add("text/html", "html");
            result.Add("text/plain", "txt");
            result.Add("text/x-log", "log");
            result.Add("text/richtext", "rtx");
            result.Add("text/rtf", "rtf");
            result.Add("text/xml", "xml");
            result.Add("video/mpeg", "mpg");
            result.Add("video/quicktime", "mov");
            result.Add("video/x-msvideo", "avi");
            result.Add("video/x-sgi-movie", "movie");
            result.Add("application/msword", "doc");
            result.Add("application/vnd.openxmlformats-officedocument.wordprocessingml.document", "docx");
            result.Add("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "xlsx");
            result.Add("message/rfc822", "eml");
            return result;

        }
        public String GetDocumentPath(BaseController controller)
        {
            String path = controller.Server.MapPath("~/Content/EmployeeDocuments") + "\\";
            Directory.CreateDirectory(path);
            return path;
        }
        public async Task SaveFile(String fileData, Int32 documentId, BaseController controller)
        {
            if (!String.IsNullOrEmpty(fileData))
            {
                int finishMimePos = fileData.IndexOf(",");
                String data = fileData.Substring(finishMimePos + 1);
                String info = fileData.Substring(0, fileData.IndexOf(","));
                info = info.Replace("base64", "").Replace(";", "").Replace("data:", "").Trim();
                Dictionary<String, String> mime = this.GetFileExtention();
                String ext = "";
                if (mime.ContainsKey(info))
                {
                    ext = "." + mime[info];
                }
                String path = this.GetDocumentPath(controller) + documentId + ext;
                File.WriteAllBytes(path, Convert.FromBase64String(data));
                var model_ = await _context.EmployeeDocuments.SingleOrDefaultAsync(x => x.EmployeeDocumentId == documentId);
                if (model_ != null)
                {
                    model_.DocumentPath = documentId + ext;
                    model_.DocumentContentType = info;
                    await _context.SaveChangesAsync();
                }
            }

        }
        public async Task<Int32> Save(EmployeeDocument dbModel, List<EmployeeDocumentNote> notes, String fileData, BaseController controller)
        {
            if (dbModel.EmployeeDocumentId == 0)
            {
                _context.EmployeeDocuments.Add(dbModel);
                await _context.SaveChangesAsync();
            }
            else
            {
                var model = await _context.EmployeeDocuments.SingleOrDefaultAsync(x => x.EmployeeDocumentId == dbModel.EmployeeDocumentId);
                if (model != null)
                {
                    model.ConsumerId = dbModel.ConsumerId;
                    model.DocumentTypeId = dbModel.DocumentTypeId;
                    model.DocumentStatusId = dbModel.DocumentStatusId;
                    model.EmployeeId = dbModel.EmployeeId;
                    model.AddedById = dbModel.AddedById;
                    model.UpdatedById = dbModel.UpdatedById;
                    model.DateCreated = dbModel.DateCreated;
                    model.DateUpdated = dbModel.DateUpdated;
                    model.DocumentPath = dbModel.DocumentPath;
                    model.DateDocument = dbModel.DateDocument;
                    var old_notes = _context.EmployeeDocumentNotes.Where(x => x.DocumentId == dbModel.EmployeeDocumentId);
                    _context.EmployeeDocumentNotes.RemoveRange(old_notes);
                }
                await _context.SaveChangesAsync();
            }
            if (notes.Count > 0)
            {
                foreach (var note in notes)
                {
                    note.DocumentId = dbModel.EmployeeDocumentId;
                    _context.EmployeeDocumentNotes.Add(note);
                }
                await _context.SaveChangesAsync();
            }
            await this.SaveFile(fileData, dbModel.EmployeeDocumentId, controller);
            return dbModel.EmployeeDocumentId;
        }
        public async Task<Boolean> Delete(Int32 id, BaseController controller)
        {
            var model = await _context.EmployeeDocuments.SingleOrDefaultAsync(x => x.EmployeeDocumentId == id);
            if (model != null)
            {
                var filePath = this.GetDocumentPath(controller) + model.DocumentPath;
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                var notes_old = _context.EmployeeDocumentNotes.Where(x => x.DocumentId == id);
                if (notes_old != null)
                {
                    _context.EmployeeDocumentNotes.RemoveRange(notes_old);
                }
                _context.EmployeeDocuments.Remove(model);

                await _context.SaveChangesAsync();
                return true;
            }
            return false;

        }

        public async Task<EmployeeDocument> GetDocument(Int32 id)
        {
            var model = await _context.EmployeeDocuments.SingleOrDefaultAsync(x => x.EmployeeDocumentId == id);
            return model;
        }
    }
}
