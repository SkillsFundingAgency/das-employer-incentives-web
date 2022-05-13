using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration
{
    public static class EmploymentCheckErrorCodes
    {
        public static Dictionary<string, string> DisplayText = new Dictionary<string, string>
        {
            { "NinoNotFound", "Check and update National Insurance number" },
            { "PAYENotFound", "Check and update PAYE scheme" },
            { "NinoAndPAYENotFound", "Check and update PAYE scheme and National Insurance number" }
        };
    }
}
