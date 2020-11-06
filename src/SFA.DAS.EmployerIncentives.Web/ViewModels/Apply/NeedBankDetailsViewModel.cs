
namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class NeedBankDetailsViewModel : ViewModel
    {
        public NeedBankDetailsViewModel() : base("We need your organisation's bank details")
        {
        }

        public string AccountHomeUrl { get; set; }
    }
}
