namespace SFA.DAS.EmployerIncentives.Web.Infrastructure
{
    public static class PolicyNames
    {
        public static string IsAuthenticated => nameof(IsAuthenticated);
        public static string HasEmployerAccount => nameof(HasEmployerAccount);
        public static string AllowAnonymous => nameof(AllowAnonymous);
    }
}
