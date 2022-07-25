namespace SFA.DAS.EmployerIncentives.Web.Services.Security
{
    public interface IAccountEncodingService
    {
        string Encode(long value);
        long Decode(string value);
    }
}
