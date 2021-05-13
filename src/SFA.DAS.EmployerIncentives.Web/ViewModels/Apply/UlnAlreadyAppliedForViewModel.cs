namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class UlnAlreadyAppliedForViewModel
    {
        public UlnAlreadyAppliedForViewModel(string accountId, string accountLegalEntityId)
        {
            AccountId = accountId;
            AccountLegalEntityId = accountLegalEntityId;
        }

        public string AccountId { get; }

        public string AccountLegalEntityId { get; }
    }
}
