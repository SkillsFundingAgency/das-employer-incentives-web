
namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Applications
{
    public class NoApplicationsViewModel : IViewModel
    {
        public string Title => $"{OrganisationName} does not have any hire a new apprentice payment applications";
        public string OrganisationName { get; set; }
        public string AccountId { get; set; }
        public string AccountLegalEntityId { get; set; }
    }
}
