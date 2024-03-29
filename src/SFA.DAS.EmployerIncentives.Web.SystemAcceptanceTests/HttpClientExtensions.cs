﻿using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
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
            var responseValue = JsonSerializer.Deserialize<T>(content);

            return (response.StatusCode, responseValue);
        }
    }
}
