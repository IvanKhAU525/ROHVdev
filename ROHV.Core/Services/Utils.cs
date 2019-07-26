using Microsoft.Reporting.WebForms;
using ROHV.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ROHV.Core.Services
{
    public class Utils
    {

        public static String ConvertInvalidFilePathChars(String value)
        {
            var invalidChars = new List<Char>(Path.GetInvalidFileNameChars());
            invalidChars.Add('№');

            foreach (var symbol in invalidChars)
            {
                value = value.Replace(symbol.ToString(), " ");
            }

            return value;
        }

        public static Byte[] RenderPDF(ReportViewer reportViewer)
        {
            Warning[] warnings;
            String[] streamids;
            String mimeType,
                   encoding,
                   extension;

            return reportViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);
        }

        public static FileDataInfo GetFileDataFromBase64String(string inputData)
        {
            FileDataInfo result = new FileDataInfo();

            var regex = new Regex("data:([a-zA-Z0-9]+/[a-zA-Z0-9-.+]+).*");
            var match = regex.Match(inputData);
            if (match.Groups.Count > 1)
            {
                result.FileType = match.Groups[1].Value;
                result.Extension = result.FileType.Split('/')[1];
            }

            return result;
        }
    }
}
