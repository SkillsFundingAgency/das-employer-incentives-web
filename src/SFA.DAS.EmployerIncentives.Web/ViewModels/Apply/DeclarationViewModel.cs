namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class DeclarationViewModel
    {
        public string AccountId { get; }

        public DeclarationViewModel(string accountId)
        {
            AccountId = accountId;
        }
    }
}