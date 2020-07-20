using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;

namespace SFA.DAS.EmployerIncentives.Web.MockServer
{
    public static class TestData
    {
        public static class Account
        {
            public class WithNoLegalEntites
            {
                public long AccountId => 11111;
                public string HashedAccountId => "MDDJNB";
            }

            public class WithSingleLegalEntityWithNoEligibleApprenticeships
            {
                public long AccountId { get; } = 22222;
                public string HashedAccountId => "MWG69Y";
                public LegalEntityDto LegalEntity => new LegalEntityDto { AccountId = AccountId, AccountLegalEntityId = 33333, LegalEntityName = "Organisation 33333" };
            }
        }        
    }
}
