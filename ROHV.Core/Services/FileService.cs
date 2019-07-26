using ROHV.Core.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROHV.Core.Services
{
    public static class IOFileService
    {
        private const string _employeeConsumerFilePath = "Content/EmployeeConsumers";
        private const string _servicesConsumerFilePath = "Content/ConsumerServices";
        private const string _consumerFilePath = "Content/Consumer";
        private const string _baseFilePath = "Content";

        public static string ReadFile(string emailPath)
        {
            string result = String.Empty;
            if (File.Exists(emailPath))
            {
                result = readTextFromFile(emailPath);
            }
            return result;
        }

        private static string readTextFromFile(string emailPath)
        {
            using (var inStream = new FileStream(emailPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(inStream))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        public static string GetEmployeeConsumerFilePath(string file = null)
        {
            return GetFilePath(file, EntityTypes.ConsumerEmployee);
        }
        public static string GetFilePath(string file = null, EntityTypes fileType = EntityTypes.Consumer)
        {
           var fileTypeDirectory = getDirectoryForEntity(fileType);
            var paths = new List<string>() { AppDomain.CurrentDomain.BaseDirectory, fileTypeDirectory };

            if (!string.IsNullOrEmpty(file))
            {
                paths.Add(file);
            }

            return Path.Combine(paths.ToArray());
        }

        private static string getDirectoryForEntity(EntityTypes fileType)
        {
            string path = _baseFilePath;
            switch(fileType)
            {
                case EntityTypes.Consumer:
                    path = _consumerFilePath;
                    break;
                case EntityTypes.ConsumerEmployee:
                    path = _employeeConsumerFilePath;
                    break;
                case EntityTypes.ConsumerServices:
                    path = _servicesConsumerFilePath;
                    break;
            }
            return path;
        }

        public static string GetFileExtension(string filePath)
        {
            var result = "";
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                result = Path.GetExtension(filePath);
            }
            return result;
        }

        public static string GetConsumerServicesFilePath(string file = null)
        {
            return GetFilePath(file, EntityTypes.ConsumerServices);
        }

        public static void SaveBase64File(string path, string fileData)
        {
            string directoryPath = Path.GetDirectoryName(path);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            int finishMimePos = fileData.IndexOf(",");
            String data = fileData.Substring(finishMimePos + 1);

            File.WriteAllBytes(path, Convert.FromBase64String(data));
        }


    }
}
