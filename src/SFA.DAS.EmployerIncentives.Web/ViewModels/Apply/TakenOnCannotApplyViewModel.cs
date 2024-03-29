﻿namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class TakenOnCannotApplyViewModel : CannotApplyViewModel
    {
        public TakenOnCannotApplyViewModel(string accountId, string commitmentsBaseUrl, string accountsBaseUrl, string organisationName) : base(accountId, accountsBaseUrl, organisationName)
        {
        }

        public new string Title => $"{OrganisationName} does not have any eligible apprentices";
    }
}
