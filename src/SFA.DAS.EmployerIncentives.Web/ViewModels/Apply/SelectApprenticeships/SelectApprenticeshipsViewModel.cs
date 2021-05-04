using System;
using SFA.DAS.EmployerIncentives.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply.SelectApprenticeships
{
    public class SelectApprenticeshipsViewModel : IViewModel
    {
        public const string SelectApprenticeshipsMessage = "Select which apprentices you want to apply for";

        public string AccountId { get; set; }

        public string AccountLegalEntityId { get; set; }
        public Guid ApplicationId { get; set; }

        public IEnumerable<ApprenticeshipModel> Apprenticeships { get; set; }

        public string FirstCheckboxId => $"new-apprenticeships-{Apprenticeships.FirstOrDefault()?.Id}";

        public string Title => "Which apprentices do you want to apply for?";

        public string OrganisationName { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int StartIndex 
        {
            get 
            {
                var startIndex = 1;
                if (CurrentPage > 1)
                {
                    startIndex = (PageSize * (CurrentPage - 1)) + 1;
                }

                return startIndex;
            }
        }
        public int EndIndex => StartIndex + (Apprenticeships.Count() - 1);

        public bool PreviousPages => (StartIndex > 1);

        public bool MorePages { get; set; }
        public int CurrentPage { get; set; }
        public int Offset { get; set; }
    }
}