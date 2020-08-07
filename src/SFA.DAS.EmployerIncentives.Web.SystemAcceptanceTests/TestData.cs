using System;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using System.Collections.Generic;
using SFA.DAS.EmployerIncentives.Web.Services.Applications.Types;

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
                public virtual LegalEntityDto LegalEntity => new LegalEntityDto { AccountId = AccountId, AccountLegalEntityId = AccountLegalEntityId, LegalEntityName = "Organisation 33333", HasSignedIncentivesTerms = true };
            }

            public class WithSingleLegalEntityWithEligibleApprenticeships
            {
                public long AccountId { get; } = 20001;
                public string HashedAccountId => "MLB7J9";
                public long AccountLegalEntityId => 30001;
                public string HashedAccountLegalEntityId => "MLP7DD";

                public List<LegalEntityDto> LegalEntities => new List<LegalEntityDto> { LegalEntity };
                public List<ApprenticeDto> Apprentices => new List<ApprenticeDto> { Apprentice1, Apprentice2, Apprentice3 };

                public virtual LegalEntityDto LegalEntity => new LegalEntityDto { AccountId = AccountId, AccountLegalEntityId = AccountLegalEntityId, LegalEntityName = $"Organisation {AccountLegalEntityId}", HasSignedIncentivesTerms = true };
                public ApprenticeDto Apprentice1 => new ApprenticeDto { ApprenticeshipId = 1,  FirstName = "Adam", FullName = "Adam 1 Glover", LastName= "Glover", CourseName = "Early Years Educator Level 3" };
                public ApprenticeDto Apprentice2 => new ApprenticeDto { ApprenticeshipId = 2, FirstName = "Mary", FullName = "Mary 2 Lyman", LastName = "Lyman", CourseName = "Assistant accountant Level 3" };
                public ApprenticeDto Apprentice3 => new ApprenticeDto { ApprenticeshipId = 3, FirstName = "Sebastian", FullName = "Sebastian 3 Lawrence", LastName = "Lawrence", CourseName = "General Welder (Arc Processes) Level 2" };
            }

            public class WithMultipleLegalEntitiesWithNoEligibleApprenticeships
            {
                public long AccountId { get; } = 30002;
                public string HashedAccountId => "VBKBLD";

                public List<LegalEntityDto> LegalEntities => new List<LegalEntityDto> { LegalEntity1, LegalEntity2, LegalEntity3 };

                public long AccountLegalEntityId1 => 40001;
                public string HashedAccountLegalEntityId1 => "MLG4LW";
                public LegalEntityDto LegalEntity1 => new LegalEntityDto { AccountId = AccountId, AccountLegalEntityId = AccountLegalEntityId1, LegalEntityName = $"Organisation {AccountLegalEntityId1}", HasSignedIncentivesTerms = true };
                public long AccountLegalEntityId2 => 40002;
                public string HashedAccountLegalEntityId2 => "VBGNWB";
                public LegalEntityDto LegalEntity2 => new LegalEntityDto { AccountId = AccountId, AccountLegalEntityId = AccountLegalEntityId2, LegalEntityName = $"Organisation {AccountLegalEntityId2}", HasSignedIncentivesTerms = true };
                public long AccountLegalEntityId3 => 40003;
                public string HashedAccountLegalEntityId3 => "VKD7X7";
                public LegalEntityDto LegalEntity3 => new LegalEntityDto { AccountId = AccountId, AccountLegalEntityId = AccountLegalEntityId3, LegalEntityName = $"Organisation {AccountLegalEntityId3}", HasSignedIncentivesTerms = true };
            }

            public class WithMultipleLegalEntitiesWithEligibleApprenticeships
            {
                public long AccountId { get; } = 30003;
                public string HashedAccountId => "VKGK4B";

                public List<LegalEntityDto> LegalEntities => new List<LegalEntityDto> { LegalEntity1, LegalEntity2 };

                public long AccountLegalEntityId1 => 40004;
                public string HashedAccountLegalEntityId1 => "M76GLY";
                public LegalEntityDto LegalEntity1 => new LegalEntityDto { AccountId = AccountId, AccountLegalEntityId = AccountLegalEntityId1, LegalEntityName = $"Organisation {AccountLegalEntityId1}", HasSignedIncentivesTerms = true };
                public long AccountLegalEntityId2 => 40005;
                public string HashedAccountLegalEntityId2 => "VW6RJG";
                public LegalEntityDto LegalEntity2 => new LegalEntityDto { AccountId = AccountId, AccountLegalEntityId = AccountLegalEntityId2, LegalEntityName = $"Organisation {AccountLegalEntityId2}", HasSignedIncentivesTerms = true };

                public List<ApprenticeDto> Apprentices => new List<ApprenticeDto> { Apprentice1, Apprentice2, Apprentice3 };
                public ApprenticeDto Apprentice1 => new ApprenticeDto { ApprenticeshipId = 1, FirstName = "Adam", FullName = "Adam 1 Glover", LastName = "Glover", CourseName = "Early Years Educator Level 3" };
                public ApprenticeDto Apprentice2 => new ApprenticeDto { ApprenticeshipId = 2, FirstName = "Mary", FullName = "Mary 2 Lyman", LastName = "Lyman", CourseName = "Assistant accountant Level 3" };
                public ApprenticeDto Apprentice3 => new ApprenticeDto { ApprenticeshipId = 3, FirstName = "Sebastian", FullName = "Sebastian 3 Lawrence", LastName = "Lawrence", CourseName = "General Welder (Arc Processes) Level 2" };

            }

            public class WithoutASignedAgreement
            {
                public long AccountId { get; } = 30037;
                public string HashedAccountId => "VKGKBB";
                public long AccountLegalEntityId => 30037;
                public string HashedAccountLegalEntityId => "VKGKBB";
                public List<LegalEntityDto> LegalEntities => new List<LegalEntityDto> { LegalEntity };
                public virtual LegalEntityDto LegalEntity => new LegalEntityDto { AccountId = AccountId, AccountLegalEntityId = AccountLegalEntityId, LegalEntityName = "Organisation 30037", HasSignedIncentivesTerms = false };
            }

            public class WithInitialApplicationForASingleEntity : WithSingleLegalEntityWithEligibleApprenticeships
            {
                public Guid ApplicationId = Guid.NewGuid();
                public ApplicationResponse ApplicationResponse => new ApplicationResponse
                {
                    Application = new IncentiveApplicationDto
                    {
                        AccountLegalEntityId = AccountLegalEntityId,
                        Apprenticeships = new IncentiveApplicationApprenticeshipDto[]
                        {
                            new IncentiveApplicationApprenticeshipDto
                            {
                                ApprenticeshipId = 1,
                                CourseName = "Computing...",
                                LastName = "Shipman",
                                FirstName = "Harry",
                                TotalIncentiveAmount = 2000m
                            },
                            new IncentiveApplicationApprenticeshipDto
                            {
                                ApprenticeshipId = 2,
                                CourseName = "T&D ...",
                                LastName = "Leeman",
                                FirstName = "Thomas",
                                TotalIncentiveAmount = 1000m
                            },
                            new IncentiveApplicationApprenticeshipDto
                            {
                                ApprenticeshipId = 3,
                                CourseName = "Water Treatment Technician, Level: 3 (Standard)",
                                LastName = "Johnson",
                                FirstName = "Michael",
                                TotalIncentiveAmount = 2000m
                            },
                            new IncentiveApplicationApprenticeshipDto
                            {
                                ApprenticeshipId = 4,
                                CourseName = "Relationship Manager (Banking), Level: 6 (Standard)",
                                LastName = "Roberts",
                                FirstName = "Jack",
                                TotalIncentiveAmount = 1500m
                            },
                            new IncentiveApplicationApprenticeshipDto
                            {
                                ApprenticeshipId = 5,
                                CourseName = "Non-destructive testing (NDT) operator, Level: 2 (Standard)",
                                LastName = "Smith",
                                FirstName = "Steven",
                                TotalIncentiveAmount = 2000m
                            }
                        }
                    }
                };

                public ApplicationResponse GetApplicationResponseWithFirstTwoApprenticesSelected =>
                    new ApplicationResponse
                    {
                        Application = new IncentiveApplicationDto
                        {
                            AccountLegalEntityId = AccountLegalEntityId,
                            Apprenticeships = new IncentiveApplicationApprenticeshipDto[]
                            {
                                new IncentiveApplicationApprenticeshipDto
                                {
                                    ApprenticeshipId = 1,
                                    CourseName = "Computing...",
                                    LastName = "Shipman",
                                    FirstName = "Harry",
                                    TotalIncentiveAmount = 2000m
                                },
                                new IncentiveApplicationApprenticeshipDto
                                {
                                    ApprenticeshipId = 2,
                                    CourseName = "T&D ...",
                                    LastName = "Leeman",
                                    FirstName = "Thomas",
                                    TotalIncentiveAmount = 1000m
                                }
                            }
                        }
                    };

                public ApplicationResponse GetApplicationResponseWithFirstTwoApprenticesSelectedAndAnAdditionalApprentice =>
                    new ApplicationResponse
                    {
                        Application = new IncentiveApplicationDto
                        {
                            AccountLegalEntityId = AccountLegalEntityId,
                            Apprenticeships = new IncentiveApplicationApprenticeshipDto[]
                            {
                                new IncentiveApplicationApprenticeshipDto
                                {
                                    ApprenticeshipId = 1,
                                    CourseName = "Computing...",
                                    LastName = "Shipman",
                                    FirstName = "Harry",
                                    TotalIncentiveAmount = 2000m
                                },
                                new IncentiveApplicationApprenticeshipDto
                                {
                                    ApprenticeshipId = 2,
                                    CourseName = "T&D ...",
                                    LastName = "Leeman",
                                    FirstName = "Thomas",
                                    TotalIncentiveAmount = 1000m
                                },
                                new IncentiveApplicationApprenticeshipDto
                                {
                                    ApprenticeshipId = 99,
                                    CourseName = "Additional No longer valid ...",
                                    LastName = "Nora",
                                    FirstName = "Moon",
                                    TotalIncentiveAmount = 1000m
                                }
                            }
                        }
                    };
            }
        }
    }
}
