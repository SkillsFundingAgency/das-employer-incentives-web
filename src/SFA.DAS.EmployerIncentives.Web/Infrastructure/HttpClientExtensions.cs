using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Infrastructure
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PatchAsJsonAsync<T>(this HttpClient client, string url, T data)
        {
            return await client.PatchAsync(url, data.GetStringContent());
        }

        public static StringContent GetStringContent(this object obj)
            => new StringContent(JsonConvert.SerializeObject(obj), Encoding.Default, "application/json");
    }
}
