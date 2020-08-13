using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels
{
    public class ViewModel
    {
        public ViewModel(string title)
        {
            Title = title;
        }

        public string Title { get; }
    }
}