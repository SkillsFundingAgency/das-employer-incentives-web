namespace SFA.DAS.EmployerIncentives.Web.Services.Session
{
    public interface ISessionService
    {
        void Set(string key, object value);
        T Get<T>(string key);
    }
}
