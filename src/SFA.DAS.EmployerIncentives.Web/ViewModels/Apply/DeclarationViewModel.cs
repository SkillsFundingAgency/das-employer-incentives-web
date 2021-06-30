using System;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class DeclarationViewModel : IViewModel
    {
        public string AccountId { get; }
        public Guid ApplicationId { get; }
        public string ManageApprenticesUrl { get; set;  }

        public string Title => "Declaration";

        public string OrganisationName { get; set; }

        public string AgreementsUrl 
        {
            get
            {
                if (!ManageApprenticesUrl.EndsWith("/"))
                {
                    ManageApprenticesUrl += "/";
                }

                return $"{ManageApprenticesUrl}accounts/{AccountId}/agreements";
            }
        }

        public DeclarationViewModel(string accountId, Guid applicationId, string organisationName, string manageApprenticesUrl)
        {
            AccountId = accountId;
            ApplicationId = applicationId;
            OrganisationName = organisationName;
            ManageApprenticesUrl = manageApprenticesUrl;
        }
    }
}