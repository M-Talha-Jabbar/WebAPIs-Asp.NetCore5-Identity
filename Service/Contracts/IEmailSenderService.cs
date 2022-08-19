using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IEmailSenderService
    {
        Task<string> SendEmailAsync(string email, string subject, string body);
    }
}
