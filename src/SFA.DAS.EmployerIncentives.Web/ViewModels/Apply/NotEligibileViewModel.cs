﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class NotEligibileViewModel : IViewModel
    {
        public string Title => "Not eligible for the payment";

        public Guid ApplicationId { get; }
        public string AccountId { get; }
        public string AccountLegalEntityId { get; }
        public List<ApplicationApprenticeship> Apprentices { get; }
        public string OrganisationName { get; set; }
        public bool AllInEligible { get; set; }

        public NotEligibileViewModel(ApplicationConfirmationViewModel applicationConfirmationViewModel) 
        {
            ApplicationId = applicationConfirmationViewModel.ApplicationId;
            AccountId = applicationConfirmationViewModel.AccountId;
            AccountLegalEntityId = applicationConfirmationViewModel.AccountLegalEntityId;
            Apprentices = applicationConfirmationViewModel.Apprentices.Where(a => !a.HasEligibleEmploymentStartDate).ToList();
            OrganisationName = applicationConfirmationViewModel.OrganisationName;
            AllInEligible = applicationConfirmationViewModel.Apprentices.All(a => !a.HasEligibleEmploymentStartDate);
        }
    }
}