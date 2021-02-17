namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class AddBankDetailsViewModel : IViewModel
    {
        public string Title => $"Add {OrganisationName}'s bank account details";

        public string OrganisationName { get; set; }
    }
}
