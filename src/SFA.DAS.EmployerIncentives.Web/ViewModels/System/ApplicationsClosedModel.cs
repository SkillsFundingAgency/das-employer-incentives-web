namespace SFA.DAS.EmployerIncentives.Web.ViewModels.System
{
    public class ApplicationsClosedModel
    {
        public string Title => "Applications open on 11 January 2022";
        public string AccountHomeUrl { get; set; }
        private string ManageApprenticeshipSiteUrl { get; set; }

        public ApplicationsClosedModel(string accountId,string manageApprenticeshipSiteUrl)
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
