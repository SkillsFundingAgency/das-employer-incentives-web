using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.ViewModels;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Extensions
{
    public static class ViewModelExtensions
    {
        public static ViewModelAssertions Should(this ViewModel instance)
        {
            return new ViewModelAssertions(instance);
        }

        public static ViewResultAssertions Should(this ViewResult instance)
        {
            return new ViewResultAssertions(instance);
        }
    }
}
