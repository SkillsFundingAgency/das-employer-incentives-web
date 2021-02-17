
namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class NeedBankDetailsViewModel : IViewModel
    {
        public string AccountHomeUrl { get; set; }

        public string Title => "We need your organisation's bank details";

        public string OrganisationName { get; set; }
    }
}
