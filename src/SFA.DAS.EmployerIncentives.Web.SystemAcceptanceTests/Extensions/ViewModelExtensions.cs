using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.ViewModels;
using System.Net.Http;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Extensions
{
    public static class AssertionsExtensions
    {
        public static ViewModelAssertions Should(this ViewModel instance)
        {
            return new ViewModelAssertions(instance);
        }

        public static ViewResultAssertions Should(this ViewResult instance)
        {
            return new ViewResultAssertions(instance);
        }
        public static HttpResponseMessageAssertions Should(this HttpResponseMessage instance)
        {
            return new HttpResponseMessageAssertions(instance);
        }
    }
}
