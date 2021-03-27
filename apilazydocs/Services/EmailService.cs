using ApiLazyDoc.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ApiLazyDoc.Services
{
    public class EmailService
    {
        private AppSettings _appSettings { get; }
        private readonly IWebHostEnvironment _env;
        public EmailService(IOptions<AppSettings> appSettings, IWebHostEnvironment env)
        {
            this._appSettings = appSettings.Value;
            this._env = env;
        }

        public async Task SendConfirmationMail(string toEmail, string userName, Guid userId)
        {
            MailMessage mail = new MailMessage()
            {
                From = new MailAddress(this._appSettings.EmailSettings.UsernameEmail, "Lazydocs")
            };
            mail.To.Add(new MailAddress(toEmail));



            mail.Subject = "Account validation";
            mail.Body = GetAccountConfirmationBody(userName, userId);
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            using (SmtpClient smtp = new SmtpClient(this._appSettings.EmailSettings.PrimaryDomain, this._appSettings.EmailSettings.PrimaryPort))
            {
                smtp.Credentials = new NetworkCredential(this._appSettings.EmailSettings.UsernameEmail, this._appSettings.EmailSettings.UsernamePassword);
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(mail);
            }
        }

        public string GetAccountConfirmationBody(string userName, Guid userId)
        {
            var template = _env.ContentRootPath
                + Path.DirectorySeparatorChar.ToString()
                + "Resources"
                + Path.DirectorySeparatorChar.ToString()
                + "EmailTemplate"
                + Path.DirectorySeparatorChar.ToString()
                + "AccountValidationTemplate.html";

            //var builder = new StreamReader(template);
            string validationURL = $"{_appSettings.AppUrl}confirmaccount/{userId}";
            string body = File.ReadAllText(template, System.Text.Encoding.UTF8).Replace("{", "{{").Replace("}", "}}").Replace("{{0}}", "{0}").Replace("{{1}}", "{1}");
            return string.Format(body, userName, validationURL);
        }
    }
}
