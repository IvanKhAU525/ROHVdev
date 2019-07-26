using ROHV.Core.Database;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System;
using ROHV.Core.Services;
using ITCraftFrame;
using ROHV.Core.Models;
using ROHV.Core.Enums;

namespace ROHV.Core.Consumer
{
    public class ConsumerServicesManagement : Services.BaseModel
    {
        public ConsumerServicesManagement(RayimContext context) : base(context) { }
        public async Task<List<ConsumerService>> GetServices(Int32 consumerId)
        {
            var services = await _context.ConsumerServices.Where(x => x.ConsumerId == consumerId).OrderBy(x => x.EffectiveDate).ToListAsync();
            return services;
        }

        public async Task<ConsumerService> Save(ConsumerService newData)
        {
            ConsumerService dbModel = null;
            var isNew = newData.ConsumerServiceId == 0;
            if (isNew)
            {
                dbModel = _context.ConsumerServices.Add(newData);
            }
            else
            {
                dbModel = await _context.ConsumerServices.SingleOrDefaultAsync(x => x.ConsumerServiceId == newData.ConsumerServiceId);
                if (dbModel != null)
                {
                    CustomMapper.MapEntity(newData, dbModel);
                }
            }
            await _context.SaveChangesAsync();

            if (isNew)
            {
                _context.Entry(dbModel).Reference(c => c.SystemUser).Load(); ;
            }
            return CustomMapper.MapEntity(dbModel, newData); ;
        }

        public async Task<Boolean> Delete(Int32 serviceId)
        {
            var model = await _context.ConsumerServices.SingleOrDefaultAsync(x => x.ConsumerServiceId == serviceId);
            if (model != null)
            {
                _context.ConsumerServices.Remove(model);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;

        }

        public async Task DeleteAll(Int32 consumerId)
        {
            var models = _context.ConsumerServices.Where(x => x.ConsumerId == consumerId);
            if (models.Count() > 0)
            {
                _context.ConsumerServices.RemoveRange(models);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<decimal?> GetTotalHours(ConsumerService consumerService)
        {
            decimal? result = 0;
            if (consumerService.ServiceId != null && consumerService.UsedHoursStartDate != null && consumerService.UsedHoursEndDate != null && consumerService.ConsumerId != null)
            {
                result = await _context.vTimeSheetDatas.Where(x => x.ClientID == consumerService.ConsumerId && x.ServiceID == consumerService.ServiceId &&
                           x.Date1 >= consumerService.UsedHoursStartDate && x.Date1 <= consumerService.UsedHoursEndDate).SumAsync(x => x.SessionDuration) ?? 0m;
            }
            return result;
        }

        public async Task<ConsumerServiceModel> GetService(int serviceId, bool withRelatedData = false)
        {
            var service = await _context.ConsumerServices.FirstOrDefaultAsync(x => x.ConsumerServiceId == serviceId);
            ConsumerServiceModel returnData = CustomMapper.MapEntity<ConsumerServiceModel>(service);

            if (withRelatedData && service != null && service.ConsumerId != null)
            {
                ConsumerEmployeeManagement manageEmployees = new ConsumerEmployeeManagement(_context);
                var employees = await manageEmployees.GetEmployees(service.ConsumerId.Value, service.ServiceId);
                returnData.ConsumerEmployeeList = CustomMapper.MapList<ConsumerEmployeeModel, ConsumerEmployee>(employees);
            }
            return returnData;
        }

        public static List<FileMetaDataModel> GetFilesByConsumerServicesId(int id)
        {
            using (RayimContext context = new RayimContext())
            {
                var files = context.FileMetaDatas.Where(x => x.ParentEntityId == id && x.ParentEntityTypeId == (int)EntityTypes.ConsumerServices).ToList();

                return CustomMapper.MapList<FileMetaDataModel, FileMetaData>(files);
            }
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
            model.ParentEntityTypeId = (int)EntityTypes.ConsumerServices;
            model.FileDisplayName = fileName;
            FileDataService.SaveFile(model, fileData);
            var savedFile = FileDataService.SaveFileMetaData(_context, model);
            return (fileId: savedFile.Id, fileName: model.FileDisplayName);
        }

        private string GetFileNameBasedOnId(int id) => string.Format("{0}_{1}", id, (int)EntityTypes.ConsumerServices);
    }
}
