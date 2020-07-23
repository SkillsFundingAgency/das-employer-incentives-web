using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests
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
                public long AccountLegalEntityId => 33333;
                public string HashedAccountLegalEntityId => "V9YW7W";
                public List<LegalEntityDto> LegalEntities => new List<LegalEntityDto> { LegalEntity };
                public LegalEntityDto LegalEntity => new LegalEntityDto { AccountId = AccountId.ToString(), AccountLegalEntityId = AccountLegalEntityId.ToString(), LegalEntityName = "Organisation 33333" };
            }

            public class WithSingleLegalEntityWithEligibleApprenticeships
            {
                public long AccountId { get; } = 20001;
                public string HashedAccountId => "MLB7J9";
                public long AccountLegalEntityId => 30001;
                public string HashedAccountLegalEntityId => "MLP7DD";

                public List<LegalEntityDto> LegalEntities => new List<LegalEntityDto> { LegalEntity};
                public List<ApprenticeDto> Apprentices => new List<ApprenticeDto> { Apprentice1, Apprentice2, Apprentice3 };

                public LegalEntityDto LegalEntity => new LegalEntityDto { AccountId = AccountId.ToString(), AccountLegalEntityId = AccountLegalEntityId.ToString(), LegalEntityName = $"Organisation {AccountLegalEntityId}" };
                public ApprenticeDto Apprentice1 => new ApprenticeDto { FirstName = "Adam", FullName = "Adam 1 Glover", LastName= "Glover", CourseName = "Early Years Educator Level 3" };
                public ApprenticeDto Apprentice2 => new ApprenticeDto { FirstName = "Mary", FullName = "Mary 2 Lyman", LastName = "Lyman", CourseName = "Assistant accountant Level 3" };
                public ApprenticeDto Apprentice3 => new ApprenticeDto { FirstName = "Sebastian", FullName = "Sebastian 3 Lawrence", LastName = "Lawrence", CourseName = "General Welder (Arc Processes) Level 2" };                
            }

            public class WithMultipleLegalEntities
            {
                public long AccountId { get; } = 30002;
                public string HashedAccountId => "VBKBLD";                

                public List<LegalEntityDto> LegalEntities => new List<LegalEntityDto> { LegalEntity1, LegalEntity2, LegalEntity3 };

                public long AccountLegalEntityId1 => 40001;
                public string HashedAccountLegalEntityId1 => "MLG4LW";
                public LegalEntityDto LegalEntity1 => new LegalEntityDto { AccountId = AccountId.ToString(), AccountLegalEntityId = AccountLegalEntityId1.ToString(), LegalEntityName = $"Organisation {AccountLegalEntityId1}" };
                public long AccountLegalEntityId2 => 40002;
                public string HashedAccountLegalEntityId2 => "VBGNWB";
                public LegalEntityDto LegalEntity2 => new LegalEntityDto { AccountId = AccountId.ToString(), AccountLegalEntityId = AccountLegalEntityId2.ToString(), LegalEntityName = $"Organisation {AccountLegalEntityId2}" };
                public long AccountLegalEntityId3 => 40003;
                public string HashedAccountLegalEntityId3 => "VKD7X7";
                public LegalEntityDto LegalEntity3 => new LegalEntityDto { AccountId = AccountId.ToString(), AccountLegalEntityId = AccountLegalEntityId3.ToString(), LegalEntityName = $"Organisation {AccountLegalEntityId3}" };
            }
        }        
    }
}
