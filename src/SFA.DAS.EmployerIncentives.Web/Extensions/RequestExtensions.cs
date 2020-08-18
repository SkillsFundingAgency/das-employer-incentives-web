using Microsoft.AspNetCore.Http;

namespace SFA.DAS.EmployerIncentives.Web.Extensions
{
    public static class RequestExtensions
    {
        public static string GetRequestUrlRoot(this HttpRequest request)
        {
            var url = $"{request.Scheme}://{request.Host}";
            return url;
        }
    }
}
