using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerIncentives.Web.Services.Security
{
    public class AccountEncodingService : IAccountEncodingService
    {
        private readonly IEncodingService _encodingService;
        public AccountEncodingService(IEncodingService encodingService)
        {
            _encodingService = encodingService;
        }

        public string Encode(long value)
        {
            return _encodingService.Encode(value, EncodingType.AccountId);
        }

        public long Decode(string value)
        {
            return _encodingService.Decode(value, EncodingType.AccountId);
        }
    }
}
