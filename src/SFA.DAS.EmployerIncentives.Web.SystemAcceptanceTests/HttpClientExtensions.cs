using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests
{
    public static class HttpClientExtensions
    {

        public static async Task<HttpResponseMessage> PostValueAsync<T>(this HttpClient client, string url, T data)
        {
            return await client.PostAsync(url, data.GetStringContent());
        }


        public static StringContent GetStringContent(this object obj)
            => new StringContent(JsonConvert.SerializeObject(obj), Encoding.Default, "application/json");
    }
}
