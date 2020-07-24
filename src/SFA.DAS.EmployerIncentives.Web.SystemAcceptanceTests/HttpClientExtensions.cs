using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests
{
    public static class HttpClientExtensions
    {

        public static async Task<HttpResponseMessage> PostFormAsync(this HttpClient client, string url, params KeyValuePair<string, string>[] data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(data)
            };

            return await client.SendAsync(request);
        }
    }
}
