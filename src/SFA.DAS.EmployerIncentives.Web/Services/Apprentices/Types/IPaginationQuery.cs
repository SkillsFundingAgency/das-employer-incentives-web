namespace SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types
{
    public interface IPaginationQuery
    {
        int PageSize { get; }
        int Offset { get; }
        int StartIndex { get; }
    }
}
