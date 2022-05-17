﻿namespace SFA.DAS.EmployerIncentives.Web.ViewModels.System
{
    public class ApplicationsClosedModel
    {
        public string Title => "Applications open on 11 January 2022";
        public string Title => "Applications for the hire a new apprentice incentive payment closed on 20 May 2022.";
        public string AccountHomeUrl { get; set; }
        public string AccountId { get; set; }
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
