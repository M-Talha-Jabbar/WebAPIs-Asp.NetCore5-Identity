using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Service.Contracts;
using Service.Models;
using System;
using System.Threading.Tasks;

namespace Service.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly SmtpSettings smtpSettings;

        public EmailSenderService(IOptions<SmtpSettings> smtpSettings)
        {
            this.smtpSettings = smtpSettings.Value;
        }

        public async Task<string> SendEmailAsync(string recipientEmail, string recipientName, string link)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(smtpSettings.SenderEmail));
            message.To.Add(MailboxAddress.Parse(recipientEmail));
            message.Subject = "Email Confirmation Link";
            message.Body = new TextPart("plain")
            {
                Text = $"Here is your email confirmation link: \n {link}"
            };

            var client = new SmtpClient();

            try
            {
                await client.ConnectAsync(smtpSettings.Server, smtpSettings.Port, true);
                await client.AuthenticateAsync(smtpSettings.SenderEmail, smtpSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                return "Email sent successfully";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            finally{
                client.Dispose();
            }
        }
    }
}
