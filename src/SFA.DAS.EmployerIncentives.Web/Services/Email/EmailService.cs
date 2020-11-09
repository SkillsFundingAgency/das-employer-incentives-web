using SFA.DAS.EmployerIncentives.Web.Services.Email.Types;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly HttpClient _client;

        public EmailService(HttpClient client)
        {
            _client = client;
        }

        public async Task SendBankDetailsRequiredEmail(SendBankDetailsEmailRequest request)
        {
            var response = await _client.PostAsJsonAsync(OuterApiRoutes.Email.SendBankDetailsRequiredEmail(), request);

            response.EnsureSuccessStatusCode();
        }

        public async Task SendBankDetailsReminderEmail(SendBankDetailsEmailRequest request)
        {
            var response = await _client.PostAsJsonAsync(OuterApiRoutes.Email.SendBankDetailsReminderEmail(), request);

            response.EnsureSuccessStatusCode();
        }
    }
}
