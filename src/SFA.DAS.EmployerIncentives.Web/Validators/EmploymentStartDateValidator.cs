using System;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.Validators
{
    public class EmploymentStartDateValidator : IEmploymentStartDateValidator
    {
        public const string InvalidFieldErrorMessage = "Enter the date in the correct format";
        private const int SchemeStartYear = 2020;

        public IEnumerable<DateValidationResult> Validate(EmploymentStartDatesRequest request)
        {
            var validationErrors = new List<DateValidationResult>();

            if (request.EmploymentStartDateDays.Count == 0 
                || request.EmploymentStartDateMonths.Count == 0 
                || request.EmploymentStartDateYears.Count == 0)
            {
                validationErrors.Add(new DateValidationResult { Index = 0, ValidationMessage = InvalidFieldErrorMessage });
                return validationErrors;
            }

            for(var index = 0; index < request.EmploymentStartDateDays.Count; index++)
            {
                if (request.EmploymentStartDateDays[index] == 0 
                    || request.EmploymentStartDateMonths[index] == 0 
                    || request.EmploymentStartDateYears[index] == 0)
                {
                    validationErrors.Add(new DateValidationResult { Index = index, ValidationMessage = InvalidFieldErrorMessage });
                }
                else if (request.EmploymentStartDateYears[index] < SchemeStartYear)
                {
                    validationErrors.Add(new DateValidationResult { Index = index, ValidationMessage = InvalidFieldErrorMessage });
                }
                else if (request.EmploymentStartDateMonths[index] > 12)
                {
                    validationErrors.Add(new DateValidationResult { Index = index, ValidationMessage = InvalidFieldErrorMessage });
                }
                else if (request.EmploymentStartDateDays[index] > DateTime.DaysInMonth(request.EmploymentStartDateYears[index], request.EmploymentStartDateMonths[index]))
                {
                    validationErrors.Add(new DateValidationResult { Index = index, ValidationMessage = InvalidFieldErrorMessage });
                }
            }

            return validationErrors;
        }
    }
}
