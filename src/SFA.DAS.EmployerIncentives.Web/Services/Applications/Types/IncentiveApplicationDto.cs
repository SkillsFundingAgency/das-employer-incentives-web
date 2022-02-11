using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications.Types
{
    public class IncentiveApplicationDto
    {
        public long AccountLegalEntityId { get; set; }
        public bool BankDetailsRequired { get; set; }
        public bool NewAgreementRequired { get; set; }
        public string SubmittedByEmail { get; set; }
        public IEnumerable<IncentiveApplicationApprenticeshipDto> Apprenticeships { get; set; }
    }
}