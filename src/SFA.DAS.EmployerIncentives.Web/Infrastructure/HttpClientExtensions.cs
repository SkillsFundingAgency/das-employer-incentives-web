using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SFA.DAS.EmployerIncentives.Web.Infrastructure
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PatchAsJsonAsync<T>(this HttpClient client, string url, T data)
        {
            return await client.PatchAsync(url, data.GetStringContent());
        }

        public static string ToUrlString(this string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : HttpUtility.UrlEncode(value);
        }

        public static async Task<(HttpStatusCode, T)> GetDataAsync<T>(this HttpClient client, string url)
        {
            using var response = await client.GetAsync(url);
            return await ProcessResponse<T>(response);
        }

        private static async Task<(HttpStatusCode, T)> ProcessResponse<T>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NoContent)
                return (response.StatusCode, default);

            var content = await response.Content.ReadAsStringAsync();
            var responseValue = JsonConvert.DeserializeObject<T>(content);

            return (response.StatusCode, responseValue);
        }

        public static StringContent GetStringContent(this object obj)
            => new StringContent(JsonConvert.SerializeObject(obj), Encoding.Default, "application/json");
    }
}
