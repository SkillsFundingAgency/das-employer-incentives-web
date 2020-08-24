namespace SFA.DAS.EmployerIncentives.Web.Services.Security
{
    public interface IDataEncryptionService
    {
        string Encrypt(string raw);
    }
}