using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels
{
    public static class ViewModelExtensions
    {
        public static ChooseOrganisationViewModel AddOrganisations(this ChooseOrganisationViewModel viewModel, IEnumerable<LegalEntityModel> legalEntities)
        {
            viewModel.Organisations = new List<OrganisationViewModel>();

            legalEntities.OrderBy(n => n.Name).ToList().ForEach(o => viewModel.Organisations.Add(
                new OrganisationViewModel
                {
                    Name = o.Name,
                    AccountLegalEntityId = o.AccountLegalEntityId
                }));

            return viewModel;
        }
    }
}
