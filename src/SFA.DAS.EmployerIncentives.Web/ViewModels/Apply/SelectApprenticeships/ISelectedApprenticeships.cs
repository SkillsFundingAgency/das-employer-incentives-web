using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply.SelectApprenticeships
{
    public interface ISelectedApprenticeships
    {
        public string AccountId { get; set; }
        public string AccountLegalEntityId { get; set; }
        List<string> SelectedApprenticeships { get; }
    }
}