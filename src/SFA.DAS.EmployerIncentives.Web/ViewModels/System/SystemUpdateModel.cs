namespace SFA.DAS.EmployerIncentives.Web.ViewModels.System
{
    public class SystemUpdateModel
    {
        public string Title => "Sorry, the service is being updated – apply for a new apprentice payment – GOV.UK";
        public string AccountHomeUrl { get; set; }
        private string ManageApprenticeshipSiteUrl { get; set; }

        public SystemUpdateModel(string accountId,string manageApprenticeshipSiteUrl)
        {
            ManageApprenticeshipSiteUrl = manageApprenticeshipSiteUrl;
            if (!manageApprenticeshipSiteUrl.EndsWith("/"))
            {
                ManageApprenticeshipSiteUrl += "/";
            }
            AccountHomeUrl = $"{ManageApprenticeshipSiteUrl}accounts/{accountId}/teams";
        }
    }
}
