using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class EmailSenderService
    {
        public async Task<string> SendEmailAsync(string email, string subject, string body)
        {
            var message = new MimeMessage();
            
        }
    }
}
