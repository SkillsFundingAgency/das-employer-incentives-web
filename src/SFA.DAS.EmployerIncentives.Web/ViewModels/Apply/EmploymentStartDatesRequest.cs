using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class EmploymentStartDatesRequest
    {
        public string AccountId { get; set; }
        public string AccountLegalEntityId { get; set; }

        public Guid ApplicationId { get; set; }

        public List<int?> EmploymentStartDateYears { get; set; }

        public List<int?> EmploymentStartDateMonths { get; set; }
        public List<int?> EmploymentStartDateDays { get; set; }

        public static string EmploymentStartDateYearsPropertyName => nameof(EmploymentStartDateYears);
        public static string EmploymentStartDateMonthsPropertyName => nameof(EmploymentStartDateMonths);

        public static string EmploymentStartDateDaysPropertyName => nameof(EmploymentStartDateDays);

    }
}
