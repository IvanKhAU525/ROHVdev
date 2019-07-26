using ITCraftFrame;
using ROHV.Core.Database;
using ROHV.Core.Enums;
using ROHV.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Services
{
    public static class FileDataService
    {
        private static string GetFileNameBasedOnId(FileMetaDataModel model) => string.Format("{0}_{1}_{2}", model.ParentEntityId, (int)model.ParentEntityTypeId, Guid.NewGuid());
        public static FileMetaDataModel SaveFileDataWithFile(RayimContext context, FileMetaDataModel model, string fileData)
        {

            ProcessOldData(context, model, fileData);
            SaveFile(model, fileData);
            return SaveFileMetaData(context, model);
        }

        private static void ProcessOldData(RayimContext context, FileMetaDataModel model, string fileData)
        {
            if (model.Id > 0)
            {
                FileMetaData dbFileData = context.FileMetaDatas.FirstOrDefault(x => x.Id == model.Id) ?? new FileMetaData();
                if (!string.IsNullOrEmpty(fileData) && !string.IsNullOrEmpty(dbFileData.FilePath))
                {
                    IOFileService.DeleteFile(dbFileData.FilePath);
                }
                if (string.IsNullOrEmpty(fileData))
                {
                    model.FilePath = dbFileData.FilePath;
                }
            }
        }

        public static FileMetaDataModel SaveFile(FileMetaDataModel model, string fileData)
        {
            if (model == null)
            {
                model = new FileMetaDataModel();
            }
            if (!string.IsNullOrEmpty(fileData))
            {
                var fileInfo = Utils.GetFileDataFromBase64String(fileData);
                model.FileContentType = fileInfo.FileType;
                var filePath = IOFileService.GetFilePath(string.Format("{0}.{1}", GetFileNameBasedOnId(model), fileInfo.Extension), (EntityTypes)model.ParentEntityTypeId);
                model.FilePath = filePath;                           
                IOFileService.SaveBase64File(filePath, fileData);
            }
            AdjustFileExtension(model);
            return model;
        }

        private static void AdjustFileExtension(FileMetaDataModel model)
        {            
            if (!string.IsNullOrEmpty(model.FileDisplayName))
            {
                string extension = IOFileService.GetFileExtension(model.FilePath);

                if (!model.FileDisplayName.EndsWith(extension))
                {
                    model.FileDisplayName += extension;
                }
            }
        }

        public static FileMetaDataModel SaveFileMetaData(RayimContext context, FileMetaDataModel model)
        {
            bool isNew = model.Id == 0;
            FileMetaData dbFileData = context.FileMetaDatas.FirstOrDefault(x => x.Id == model.Id) ?? new FileMetaData();
            CustomMapper.MapEntity(model, dbFileData);
            if (isNew)
            {
                context.FileMetaDatas.Add(dbFileData);
            }
            context.SaveChanges();
            return CustomMapper.MapEntity<FileMetaDataModel>(dbFileData);
        }

        public static Boolean DeleteFileData(RayimContext context, int fileId)
        {
            FileMetaData dbFileData = context.FileMetaDatas.FirstOrDefault(x => x.Id == fileId);
            if (dbFileData != null)
            {
                if (!string.IsNullOrEmpty(dbFileData.FilePath))
                {
                    IOFileService.DeleteFile(dbFileData.FilePath);
                }
                context.FileMetaDatas.Remove(dbFileData);
                context.SaveChanges();
            }
            return dbFileData != null;
        }

        public static FileMetaDataModel GetFileMetadata(RayimContext context, int fileId)
        {
            var dbEntity = context.FileMetaDatas.FirstOrDefault(x => x.Id == fileId);
            return CustomMapper.MapEntity<FileMetaDataModel>(dbEntity);
        }

        public static List<FileMetaDataModel> GetConsumerFiles(RayimContext context, int consumerId)
        {
            var dbFiles = context.FileMetaDatas.Where(x => x.ParentEntityId == consumerId && x.ParentEntityTypeId == (int)EntityTypes.Consumer).ToList();
            return CustomMapper.MapList<FileMetaDataModel, FileMetaData>(dbFiles);
        }

        public static void UpdateFileMetaData(FileMetaDataModel inputModel, int userId)
        {
            if (inputModel.Id == 0)
            {
                inputModel.AddedById = userId;
                inputModel.CreatedOn = DateTime.UtcNow;
            }

            inputModel.UpdatedById = userId;
            inputModel.UpdatedOn = DateTime.UtcNow;
        }
    }
}
