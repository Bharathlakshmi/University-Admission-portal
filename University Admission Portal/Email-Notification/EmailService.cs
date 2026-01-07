using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace University_Admission_Portal.Email_Notification
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = false);
    }
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = false)
        {
            var fromAddress = new MailAddress(_settings.From, _settings.DisplayName);
            var toAddress = new MailAddress(toEmail);


            using var smtp = new SmtpClient
            {
                Host = _settings.SmtpServer,
                Port = _settings.Port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password)
            };

            using var mail = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            await smtp.SendMailAsync(mail);
        }
    }
}
