using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IEmailSenderService
    {
        Task<string> SendEmailAsync(string recipientEmail, string recipientName, string link);
    }
}
