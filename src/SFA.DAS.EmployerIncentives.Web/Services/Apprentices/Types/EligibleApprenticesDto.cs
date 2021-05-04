using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types
{
    public class EligibleApprenticesDto
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalApprenticeships { get; set; }
        public List<ApprenticeDto> Apprenticeships { get; set; }
        public bool MorePages { get; set; }
        public int Offset { get; set; }
    }
}
