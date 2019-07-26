using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using ROHV.Core.Attributes;
using ITCraftFrame.BoundData;
using ITCraftFrame;
using System.Web.Configuration;
using ROHV.Core.Database;
using ROHV.Core.User;
using ROHV.Core.Models;

namespace ROHV.Core.Services
{
    public class EmailService

    {
        private static string _emailSeprator { set; get; }
        private static object _lock = new object();
        private static bool _isOverrideSmtpSettings { set; get; }
        static EmailService()
        {
            lock (_lock)
            {
                _emailSeprator = WebConfigurationManager.AppSettings["EmailSeparator"] ?? ";";
                _isOverrideSmtpSettings = Boolean.Parse(WebConfigurationManager.AppSettings["isOverrideSmtp"] ?? "false"); 
            }
        }
        public class FileAttachment
        {
            public byte[] FileBytes { get; set; }
            public String Name { get; set; }
        }
        public static async Task Send(String to, String toName, String subject, String message, String fromEmail = null)
        {
            var user = UserManagment.GetUserByEmail(fromEmail);
            var mailMessage = _createMailMessageData(to, toName, subject, message, user);

            using (var smtp = new SmtpClient())
            {
                _overrideSmtpSettings(smtp, user);
                await smtp.SendMailAsync(mailMessage);
            }
        }

        private static void _overrideSmtpSettings(SmtpClient smtp, UserModel user)
        {

           if (_isOverrideSmtpSettings && user != null && !string.IsNullOrEmpty(user.Email) && !string.IsNullOrEmpty(user.EmailPassword))
            {
                smtp.Credentials = new System.Net.NetworkCredential(user.Email, user.EmailPassword);
            }
        }

        private static string getNameFromUser(UserModel user)
        {
            string userName = null;
           
            if (user != null)
            {
                userName = String.Format("{0} {1}", user?.FirstName, user?.LastName);
            }
            return userName;
        }

        private static MailMessage _createMailMessageData(string to, string toName, string subject, string message, UserModel user = null, List<FileAttachment> files = null)
        {
            List<string> EmailService = to.Split(new String[] { _emailSeprator }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var mailMessage = new MailMessage();
            EmailService.ForEach(x =>
            {
                if (!string.IsNullOrEmpty(x))
                {
                    mailMessage.To.Add(new MailAddress(x.Trim(), toName, Encoding.UTF8));
                }
            });

            mailMessage.Subject = subject;
            mailMessage.Body = message;
            mailMessage.IsBodyHtml = true;
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                string fromName = getNameFromUser(user);
                mailMessage.From = new MailAddress(user.Email, fromName ?? user.Email, Encoding.UTF8);
            }

            if (files != null)
            {
                files.ForEach(file =>
                {
                    Attachment att = new Attachment(new MemoryStream(file.FileBytes), file.Name);
                    mailMessage.Attachments.Add(att);
                });
            }

            return mailMessage;
        }

        public static async Task SendBoundEmail(String to, String toName, String subject, String emailTemplateName, List<Object> emailInputData, String fromEmail = null)
        {
            List<FieldDataBindingModel> boundData = BoundDataManager.GetBoundDataList<EmailBoundAttribute>(emailInputData);

            var bodyTemplate = GetEmailTemplate(emailTemplateName);
            var body = StringProcessor.GetStringWithSubstitutions(bodyTemplate, boundData);
            await Send(to, toName, subject, body, fromEmail);
        }

        public static string GetEmailTemplate(string templateName)
        {
            string result = String.Empty;
            var emailPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates", String.Format("{0}.html", templateName));

            result = IOFileService.ReadFile(emailPath);

            return result;
        }

        public static async Task SendEmailWithAttach(String to, String toName, String subject, String message, List<FileAttachment> files, String fromEmail = null)
        {
            var user = UserManagment.GetUserByEmail(fromEmail);
            var mailMessage = _createMailMessageData(to, toName, subject, message, user, files);
            using (var smtp = new SmtpClient())
            {
                _overrideSmtpSettings(smtp, user);
                await smtp.SendMailAsync(mailMessage);
            }
        }
    }
}
