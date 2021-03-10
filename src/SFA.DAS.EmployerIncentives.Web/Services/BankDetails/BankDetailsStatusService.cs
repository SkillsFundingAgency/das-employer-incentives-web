using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;

namespace SFA.DAS.EmployerIncentives.Web.Services.BankDetails
{
    public class BankDetailsStatusService : IBankDetailsStatusService
    {
        private readonly ILegalEntitiesService _legalEntitiesService;
        private const string BankDetailsSuppliedStatus = "Requested";

        public BankDetailsStatusService(ILegalEntitiesService legalEntitiesService)
        {
            _legalEntitiesService = legalEntitiesService;
        }

        public async Task<LegalEntityModel> RecordBankDetailsComplete(string accountId, string accountLegalEntityId)
        {
            var legalEntity = await _legalEntitiesService.Get(accountId, accountLegalEntityId);
            
            if (String.IsNullOrWhiteSpace(legalEntity.VrfCaseStatus))
            {
                legalEntity.VrfCaseStatus = BankDetailsSuppliedStatus;
                await _legalEntitiesService.UpdateVrfCaseStatus(legalEntity);
            }

            return await Task.FromResult(legalEntity);
        }
    }
}
