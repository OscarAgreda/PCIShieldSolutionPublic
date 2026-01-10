using System.Collections.Generic;
using System.Threading.Tasks;
using MimeKit;
namespace PCIShield.Domain.Interfaces
{
    public interface IMimeKitEmailSender
    {
        Task SendEmailAsync(string to, string from, string subject, string body);
        Task<IEnumerable<MimeMessage>> GetMessagesAsync();
    }
}