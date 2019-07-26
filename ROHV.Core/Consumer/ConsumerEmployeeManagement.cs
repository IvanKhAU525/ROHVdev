using ROHV.Core.Database;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System;
using ROHV.Core.Services;
using ROHV.Core.Enums;
using ROHV.Core.Models;


namespace ROHV.Core.Consumer
{
    public class ConsumerEmployeeManagement : BaseModel
    {
        public ConsumerEmployeeManagement(RayimContext context) : base(context) { }
        public async Task<List<ConsumerEmployee>> GetEmployees(Int32 consumerId, Int32? serviceId = null)
        {
            var employeesQuery = _context.ConsumerEmployees.Where(x => x.ConsumerId == consumerId);
            if (serviceId != null)
            {
                employeesQuery = employeesQuery.Where(x => x.ServiceId == serviceId.Value);
            }


            var employees = await employeesQuery.OrderBy(x => x.Contact.LastName).ThenBy(x => x.Contact.FirstName).ToListAsync();
            return employees;
        }

        public async Task<Int32> Save(ConsumerEmployee dbModel)
        {
            if (dbModel.ConsumerEmployeeId == 0)
            {
                _context.ConsumerEmployees.Add(dbModel);
            }
            else
            {
                var model = await _context.ConsumerEmployees.SingleOrDefaultAsync(x => x.ConsumerEmployeeId == dbModel.ConsumerEmployeeId);

                if (model != null)
                {
                    ITCraftFrame.CustomMapper.MapEntity(dbModel, model);
                }
            }
            await _context.SaveChangesAsync();
            return dbModel.ConsumerEmployeeId;
        }

        public (int fileId, string fileName) SaveConsumerEmployeeFile(SystemUser user, int id, string fileData, string fileName, int? fileId = null)
        {
            bool isNew = fileId == null;
            FileMetaDataModel model = isNew ? new FileMetaDataModel() : FileDataService.GetFileMetadata(_context, fileId.Value);
            if (isNew)
            {
                model.CreatedOn = DateTime.UtcNow;
                model.AddedById = user.UserId;
            }
            else
            {
                model.UpdatedOn = DateTime.UtcNow;
                model.UpdatedById = user.UserId;
            }
            model.ParentEntityId = id;
            model.ParentEntityTypeId = (int)EntityTypes.ConsumerEmployee;
            var fileInfo = Utils.GetFileDataFromBase64String(fileData);
            model.FileContentType = fileInfo.FileType;
            model.FileDisplayName = fileName;

            var filePath = IOFileService.GetEmployeeConsumerFilePath(string.Format("{0}.{1}", GetFileNameBasedOnId(id), fileInfo.Extension));
            model.FilePath = filePath;

            IOFileService.SaveBase64File(filePath, fileData);
            var savedFile = FileDataService.SaveFileMetaData(_context, model);

            return (fileId: savedFile.Id, fileName: model.FileDisplayName);
        }

        public static List<FileMetaDataModel> GetFilesByConsumerEmployeeId(int id)
        {
            using(RayimContext context = new RayimContext())
            {
                var files = context.FileMetaDatas.Where(x => x.ParentEntityId == id && x.ParentEntityTypeId == (int)EntityTypes.ConsumerEmployee).ToList();

                return ITCraftFrame.CustomMapper.MapList<FileMetaDataModel, FileMetaData>(files);
            }
        }
        public static string GetFileNameBasedOnId (int id)
        {
            return string.Format("{0}_{1}", id, (int)EntityTypes.ConsumerEmployee);
        }

        public async Task<Boolean> Delete(Int32 serviceId)
        {
            var model = await _context.ConsumerEmployees.SingleOrDefaultAsync(x => x.ConsumerEmployeeId == serviceId);
            if (model != null)
            {
                _context.ConsumerEmployees.Remove(model);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task DeleteAll(Int32 consumerId)
        {
            var models = _context.ConsumerEmployees.Where(x => x.ConsumerId == consumerId);
            if (models.Count() > 0)
            {
                _context.ConsumerEmployees.RemoveRange(models);
                await _context.SaveChangesAsync();
            }
        }
    }
}
