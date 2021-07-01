using System.Collections.Generic;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;

namespace SFA.DAS.EmployerIncentives.Web.Validators
{
    public interface IEmploymentStartDateValidator
    {
        IEnumerable<DateValidationResult> Validate(EmploymentStartDatesRequest request);
    }
}
