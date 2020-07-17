using SFA.DAS.EmployerIncentives.Web.Models;
using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.Services
{
    public interface IDataService
    {
        IEnumerable<ApprenticeshipModel> GetEligibleApprenticeships();
    }
}