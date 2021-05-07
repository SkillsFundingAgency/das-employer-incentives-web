﻿using System;
using SFA.DAS.EmployerIncentives.Web.Models;
using System.Collections.Generic;
using SFA.DAS.EmployerIncentives.Web.Validators;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class EmploymentStartDatesViewModel : IViewModel
    {
        public string Title => $"When did {ApprenticeName} join {OrganisationName}?";
        
        public string OrganisationName { get; set; }
        public string AccountId { get; set; }
        public Guid ApplicationId { get; set; }
        public List<ApplicationApprenticeshipModel> Apprentices { get; set; }
        public List<DateValidationResult> DateValidationResults { get; set; }
        public string ApprenticeName 
        {
            get 
            { 
                if (Apprentices.Count == 1)
                {
                    return Apprentices[0].FullName;
                }

                return "they";
            }
        } 
    }
}
