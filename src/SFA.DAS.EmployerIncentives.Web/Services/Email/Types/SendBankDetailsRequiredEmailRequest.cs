
namespace SFA.DAS.EmployerIncentives.Web.Services.Email.Types
{
    public class SendBankDetailsRequiredEmailRequest
    {
        public long AccountId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string EmailAddress { get; set; }
        public string AddBankDetailsUrl { get; set; }
    }
}
