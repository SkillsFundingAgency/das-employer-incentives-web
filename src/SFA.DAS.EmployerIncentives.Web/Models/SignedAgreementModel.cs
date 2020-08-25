using System;

namespace SFA.DAS.EmployerIncentives.Web.Models
{
    public class SignedAgreementModel
    {
        public DateTime SignedDate { get; set; }
        public string SignedByName { get; set; }
        public string SignedByEmail { get; set; }

        public string ToPsvString()
        {
            return string.Join("|", SignedByName, SignedByEmail, SignedDate.ToString("yyyy-MM-ddTHH:mm:ss"));
        }
    }
}