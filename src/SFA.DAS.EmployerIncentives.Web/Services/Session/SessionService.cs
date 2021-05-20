using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace SFA.DAS.EmployerIncentives.Web.Services.Session
{
    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _environment;

        public SessionService(IHttpContextAccessor httpContextAccessor, string environment)
        {
            _httpContextAccessor = httpContextAccessor;
            _environment = environment;
        }

        public void Set(string key, object value)
        {
            _httpContextAccessor.HttpContext.Session.SetString(_environment + "_" + key,
                JsonConvert.SerializeObject(value));
        }

        public T Get<T>(string key)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            key = _environment + "_" + key;

            if (session.Keys.All(k => k != key))
            {
                return default(T);
            }

            var value = session.GetString(key);

            return string.IsNullOrWhiteSpace(value) ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

    }
}
