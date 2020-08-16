using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Services.Applications.Types;
using System;
using System.Net.Http;
using System.Threading.Tasks;


namespace SFA.DAS.EmployerIncentives.Web.Services.Applications
{
    public class BankingDetailsService : IBankingDetailsService
    {
        private readonly HttpClient _client;

        public BankingDetailsService(HttpClient client)
        {
            _client = client;
        }

        public async Task<BankingDetailsDto> GetBankingDetails(long accountId, Guid applicationId)
        {
            var url = OuterApiRoutes.GetBankingDetailsUrl(accountId, applicationId);

            var (_, data) = await _client.GetDataAsync<BankingDetailsDto>(url);

            return data;
        }
    }
}
