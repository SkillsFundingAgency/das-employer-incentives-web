using System;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications.Types
{
    public class SignedAgreementDto
    {
        public DateTime SignedDate { get; set; }
        public string SignedByName { get; set; }
        public string SignedByEmail { get; set; }
    }
}