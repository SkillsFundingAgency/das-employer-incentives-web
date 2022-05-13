using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration
{
    public static class EmploymentCheckErrorCodes
    {
        private static readonly Dictionary<string, string> EmploymentCheckErrorCodeMappings = new Dictionary<string,string>
        {
            { "NinoNotFound", "Check and update National Insurance number" },
            { "PAYENotFound", "Check and update PAYE scheme" },
            { "NinoAndPAYENotFound", "Check and update PAYE scheme and National Insurance number" }
        };

        public static Dictionary<string, string> DisplayText => EmploymentCheckErrorCodeMappings;
    }
}
