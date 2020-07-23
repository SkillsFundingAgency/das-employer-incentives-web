namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class DeclarationViewModel : ViewModel
    {
        public string AccountId { get; }

        public DeclarationViewModel(string accountId) : base("Declaration")
        {
            AccountId = accountId;
        }
    }
}