using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications.Types;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests
{
    public static class TestData
    {
        public static class User
        {
            public static Guid AccountDocumentId = Guid.Parse("901b1fd4-c6af-4963-a955-f143d9b6f447");
            public static Guid AccountOwnerUserId = Guid.Parse("a367001d-265f-49ab-b6ed-ad51f5b5338c");
            public static string AuthenticatedHashedId = "VBKBLD";
        }

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
                public virtual LegalEntityDto LegalEntity => new LegalEntityDto { AccountId = AccountId, AccountLegalEntityId = AccountLegalEntityId, LegalEntityName = "Organisation 33333", IsAgreementSigned = true };
            }

            public class WithSingleLegalEntityWithEligibleApprenticeships
            {
                public long AccountId { get; } = 20001;
                public string HashedAccountId => "MLB7J9";
                public long AccountLegalEntityId => 30001;
                public string HashedAccountLegalEntityId => "MLP7DD";

                public List<LegalEntityDto> LegalEntities => new List<LegalEntityDto> { LegalEntity };
                public List<ApprenticeDto> Apprentices => new List<ApprenticeDto> { Apprentice1, Apprentice2, Apprentice3 };

                public virtual LegalEntityDto LegalEntity => new LegalEntityDto { AccountId = AccountId, AccountLegalEntityId = AccountLegalEntityId, LegalEntityName = $"Organisation {AccountLegalEntityId}", IsAgreementSigned = true, HashedLegalEntityId = "VKGFT" };
                public ApprenticeDto Apprentice1 => new ApprenticeDto { ApprenticeshipId = 1, FirstName = "Adam", FullName = "Adam 1 Glover", LastName = "Glover", CourseName = "Early Years Educator Level 3", StartDate = new DateTime(2020, 8, 1), Uln = 12345678 };
                public ApprenticeDto Apprentice2 => new ApprenticeDto { ApprenticeshipId = 2, FirstName = "Mary", FullName = "Mary 2 Lyman", LastName = "Lyman", CourseName = "Assistant accountant Level 3", StartDate = new DateTime(2020, 9, 1), Uln = 23456789 };
                public ApprenticeDto Apprentice3 => new ApprenticeDto { ApprenticeshipId = 3, FirstName = "Sebastian", FullName = "Sebastian 3 Lawrence", LastName = "Lawrence", CourseName = "General Welder (Arc Processes) Level 2", StartDate = new DateTime(2020, 10, 1), Uln = 456789012 };
            }

            public class WithMultipleLegalEntitiesWithNoEligibleApprenticeships
            {
                public long AccountId { get; } = 30002;
                public string HashedAccountId => "VBKBLD";

                public List<LegalEntityDto> LegalEntities => new List<LegalEntityDto> { LegalEntity1, LegalEntity2, LegalEntity3 };

                public long AccountLegalEntityId1 => 40001;
                public string HashedAccountLegalEntityId1 => "MLG4LW";
                public LegalEntityDto LegalEntity1 => new LegalEntityDto { AccountId = AccountId, AccountLegalEntityId = AccountLegalEntityId1, LegalEntityName = $"Organisation {AccountLegalEntityId1}", IsAgreementSigned = true};
                public long AccountLegalEntityId2 => 40002;
                public string HashedAccountLegalEntityId2 => "VBGNWB";
                public LegalEntityDto LegalEntity2 => new LegalEntityDto { AccountId = AccountId, AccountLegalEntityId = AccountLegalEntityId2, LegalEntityName = $"Organisation {AccountLegalEntityId2}", IsAgreementSigned = true};
                public long AccountLegalEntityId3 => 40003;
                public string HashedAccountLegalEntityId3 => "VKD7X7";
                public LegalEntityDto LegalEntity3 => new LegalEntityDto { AccountId = AccountId, AccountLegalEntityId = AccountLegalEntityId3, LegalEntityName = $"Organisation {AccountLegalEntityId3}", IsAgreementSigned = true};
            }

            public class WithPreviousApplicationsForFirstLegalEntity : WithMultipleLegalEntitiesWithEligibleApprenticeships
            {
                public ApprenticeApplicationModel Application1 => new ApprenticeApplicationModel { AccountId = AccountId, ApplicationDate = new DateTime(2020, 09, 01), FirstName = "Jane", LastName = "Doe", TotalIncentiveAmount = 1500m, LegalEntityName = $"Organisation {AccountLegalEntityId1}", ULN = 900004567, CourseName = "Accounting", FirstPaymentStatus = new PaymentStatusModel { LearnerMatchFound = false, PausePayments = false } };
                public ApprenticeApplicationModel Application2 => new ApprenticeApplicationModel { AccountId = AccountId, ApplicationDate = new DateTime(2020, 08, 14), FirstName = "Robert", LastName = "Smith", TotalIncentiveAmount = 2000m, LegalEntityName = $"Organisation {AccountLegalEntityId1}", ULN = 9565565665, CourseName = "Bar Tending", FirstPaymentStatus = new PaymentStatusModel { LearnerMatchFound = true, HasDataLock = true, PausePayments = false } };
                public ApprenticeApplicationModel Application3 => new ApprenticeApplicationModel { AccountId = AccountId, ApplicationDate = new DateTime(2020, 10, 05), FirstName = "Andrew", LastName = "Digby-Jones", TotalIncentiveAmount = 2000m, LegalEntityName = $"Organisation {AccountLegalEntityId1}", ULN = 9968575765, CourseName = "Zoo Keeper", FirstPaymentStatus = new PaymentStatusModel { LearnerMatchFound = true, HasDataLock = false, InLearning = true, PaymentAmount = 1500, PaymentDate = new DateTime(2021, 2, 1), PausePayments = false } };
                public ApprenticeApplicationModel Application4 => new ApprenticeApplicationModel { AccountId = AccountId, ApplicationDate = new DateTime(2020, 10, 08), FirstName = "Steve", LastName = "Craddock", TotalIncentiveAmount = 1500m, LegalEntityName = $"Organisation {AccountLegalEntityId1}", ULN = 9968445765, CourseName = "Sausage Making", FirstPaymentStatus = new PaymentStatusModel { LearnerMatchFound = true, HasDataLock = false, InLearning = false, PausePayments = false } };
                public ApprenticeApplicationModel Application5 => new ApprenticeApplicationModel { AccountId = AccountId, ApplicationDate = new DateTime(2020, 11, 01), FirstName = "Helen", LastName = "Taylor", TotalIncentiveAmount = 1500m, LegalEntityName = $"Organisation {AccountLegalEntityId1}", ULN = 9763445765, CourseName = "Finance Management", FirstPaymentStatus = new PaymentStatusModel { LearnerMatchFound = true, HasDataLock = false, InLearning = true, PausePayments = true } };
            }

            public class WithPreviousApprenticeshipIncentiveForFirstLegalEntity : WithMultipleLegalEntitiesWithEligibleApprenticeships
            {
                public List<ApprenticeshipIncentiveModel> ApprenticeshipIncentives => new List<ApprenticeshipIncentiveModel> { ApprenticeshipIncentive1, ApprenticeshipIncentive2, ApprenticeshipIncentive3, ApprenticeshipIncentive4, ApprenticeshipIncentive5 };
                public ApprenticeshipIncentiveModel ApprenticeshipIncentive1 => new ApprenticeshipIncentiveModel { Id = Guid.Parse("65e6b350-e78a-464f-93c0-0b3b1f3ab411").ToString(), ApprenticeshipId = 1, FirstName = "Jane", LastName = "Doe", CourseName = "Accounting", StartDate = new DateTime(2021, 05, 01), Uln = 900004567};
                public ApprenticeshipIncentiveModel ApprenticeshipIncentive2 => new ApprenticeshipIncentiveModel { Id = Guid.Parse("8c5e0ed3-0364-413d-a939-b061ed093879").ToString(), ApprenticeshipId = 2, FirstName = "Robert", LastName = "Smith", CourseName = "Bar Tending", StartDate = new DateTime(2021, 04, 01), Uln = 9565565665};
                public ApprenticeshipIncentiveModel ApprenticeshipIncentive3 => new ApprenticeshipIncentiveModel { Id = Guid.Parse("b32c2bc4-73e4-43bb-a6d9-25a7707ea2e6").ToString(), ApprenticeshipId = 3, FirstName = "Andrew", LastName = "Digby-Jones", CourseName = "Zoo Keeper", StartDate = new DateTime(2021, 06, 12), Uln = 9968575765};
                public ApprenticeshipIncentiveModel ApprenticeshipIncentive4 => new ApprenticeshipIncentiveModel { Id = Guid.Parse("590eb25a-3f64-4c48-aba6-414391f0efd6").ToString(), ApprenticeshipId = 4, FirstName = "Steve", LastName = "Craddock", CourseName = "Sausage Making", StartDate = new DateTime(2021, 06, 01), Uln = 9968445765};
                public ApprenticeshipIncentiveModel ApprenticeshipIncentive5 => new ApprenticeshipIncentiveModel { Id = Guid.Parse("472a7374-3e57-4869-8aaf-d7030e793c36").ToString(), ApprenticeshipId = 5, FirstName = "Helen", LastName = "Taylor", CourseName = "Finance Management", StartDate = new DateTime(2021, 06, 29), Uln = 9763445765};

                public ApprenticeApplicationModel Application1 => new ApprenticeApplicationModel { AccountId = AccountId, ApplicationDate = new DateTime(2020, 09, 01), FirstName = "Jane", LastName = "Doe", TotalIncentiveAmount = 1500m, LegalEntityName = $"Organisation {AccountLegalEntityId1}", ULN = 900004567, CourseName = "Accounting", FirstPaymentStatus = new PaymentStatusModel { LearnerMatchFound = false, PausePayments = false } };
                public List<ApprenticeApplicationModel> ApprenticeApplications => new List<ApprenticeApplicationModel> { Application1};

                public GetApplicationsModel GetApplicationsResponse => new GetApplicationsModel
                {
                    ApprenticeApplications = ApprenticeApplications
                };

            }

            public class WithMultipleLegalEntitiesWithEligibleApprenticeships
            {
                public long AccountId { get; } = 30003;
                public string HashedAccountId => "VKGK4B";

                public List<LegalEntityDto> LegalEntities => new List<LegalEntityDto> { LegalEntity1, LegalEntity2 };

                public long AccountLegalEntityId1 => 40004;
                public string HashedAccountLegalEntityId1 => "M76GLY";
                public LegalEntityDto LegalEntity1 => new LegalEntityDto { AccountId = AccountId, AccountLegalEntityId = AccountLegalEntityId1, LegalEntityName = $"Organisation {AccountLegalEntityId1}", IsAgreementSigned = true };
                public long AccountLegalEntityId2 => 40005;
                public string HashedAccountLegalEntityId2 => "VW6RJG";
                public LegalEntityDto LegalEntity2 => new LegalEntityDto { AccountId = AccountId, AccountLegalEntityId = AccountLegalEntityId2, LegalEntityName = $"Organisation {AccountLegalEntityId2}", IsAgreementSigned = true };

                public List<ApprenticeDto> Apprentices => new List<ApprenticeDto> { Apprentice1, Apprentice2, Apprentice3 };
                public ApprenticeDto Apprentice1 => new ApprenticeDto { ApprenticeshipId = 1, FirstName = "Adam", FullName = "Adam 1 Glover", LastName = "Glover", CourseName = "Early Years Educator Level 3", StartDate = new DateTime(2020, 8, 1), Uln = 12345678 };
                public ApprenticeDto Apprentice2 => new ApprenticeDto { ApprenticeshipId = 2, FirstName = "Mary", FullName = "Mary 2 Lyman", LastName = "Lyman", CourseName = "Assistant accountant Level 3", StartDate = new DateTime(2020, 9, 1), Uln = 23456789 };
                public ApprenticeDto Apprentice3 => new ApprenticeDto { ApprenticeshipId = 3, FirstName = "Sebastian", FullName = "Sebastian 3 Lawrence", LastName = "Lawrence", CourseName = "General Welder (Arc Processes) Level 2", StartDate = new DateTime(2020, 10, 1), Uln = 34567890 };

            }
            public class WithApplicationWithAllInEligibleEmployerStartDates
            {
                public Guid ApplicationId = Guid.Parse("18073eb3-f22a-4ab3-9726-dfe99b911d54");

                public long AccountId { get; } = 6005;
                public string HashedAccountId => "VWDYNG";

                public List<LegalEntityDto> LegalEntities => new List<LegalEntityDto> { LegalEntity1 };

                public long AccountLegalEntityId1 => 6006;
                public string HashedAccountLegalEntityId1 => "V6PNBL";
                public LegalEntityDto LegalEntity1 => new LegalEntityDto { AccountId = AccountId, AccountLegalEntityId = AccountLegalEntityId1, LegalEntityName = $"Organisation {AccountLegalEntityId1}" };
                
                public List<ApprenticeDto> Apprentices => new List<ApprenticeDto> { Apprentice1, Apprentice2, Apprentice3 };
                public ApprenticeDto Apprentice1 => new ApprenticeDto { ApprenticeshipId = 1, FirstName = "Adam", FullName = "Adam 1 Glover", LastName = "Glover", CourseName = "Early Years Educator Level 3", StartDate = new DateTime(2020, 8, 1), Uln = 12345678, EmploymentStartDate = new DateTime(2020, 1, 1) };
                public ApprenticeDto Apprentice2 => new ApprenticeDto { ApprenticeshipId = 2, FirstName = "Mary", FullName = "Mary 2 Lyman", LastName = "Lyman", CourseName = "Assistant accountant Level 3", StartDate = new DateTime(2020, 9, 1), Uln = 23456789, EmploymentStartDate = new DateTime(2020, 1, 1) };
                public ApprenticeDto Apprentice3 => new ApprenticeDto { ApprenticeshipId = 3, FirstName = "Sebastian", FullName = "Sebastian 3 Lawrence", LastName = "Lawrence", CourseName = "General Welder (Arc Processes) Level 2", StartDate = new DateTime(2020, 10, 1), Uln = 34567890, EmploymentStartDate = new DateTime(2020, 1, 1) };

                public ApplicationResponse ApplicationResponse => new ApplicationResponse
                    {
                        Application = new IncentiveApplicationDto
                        {
                            AccountLegalEntityId = AccountLegalEntityId1,
                            BankDetailsRequired = true,
                            Apprenticeships = new List<IncentiveApplicationApprenticeshipDto>
                            {
                                new IncentiveApplicationApprenticeshipDto
                                {
                                    ApprenticeshipId = Apprentice1.ApprenticeshipId,
                                    CourseName = Apprentice1.CourseName,
                                    FirstName = Apprentice1.FirstName,
                                    LastName =   Apprentice1.LastName,
                                    TotalIncentiveAmount = 2000m,
                                    PlannedStartDate = Apprentice1.StartDate,
                                    EmploymentStartDate = Apprentice1.EmploymentStartDate,
                                    Uln = Apprentice1.Uln,
                                    HasEligibleEmploymentStartDate = false
                                },
                                new IncentiveApplicationApprenticeshipDto
                                {
                                    ApprenticeshipId = Apprentice2.ApprenticeshipId,
                                    CourseName = Apprentice2.CourseName,
                                    FirstName = Apprentice2.FirstName,
                                    LastName =   Apprentice2.LastName,
                                    TotalIncentiveAmount = 2000m,
                                    PlannedStartDate = Apprentice2.StartDate,
                                    EmploymentStartDate = Apprentice2.EmploymentStartDate,
                                    Uln = Apprentice2.Uln,
                                    HasEligibleEmploymentStartDate = false
                                },
                                new IncentiveApplicationApprenticeshipDto
                                {
                                    ApprenticeshipId = Apprentice3.ApprenticeshipId,
                                    CourseName = Apprentice3.CourseName,
                                    FirstName = Apprentice3.FirstName,
                                    LastName =   Apprentice3.LastName,
                                    TotalIncentiveAmount = 2000m,
                                    PlannedStartDate = Apprentice3.StartDate,
                                    EmploymentStartDate = Apprentice3.EmploymentStartDate,
                                    Uln = Apprentice3.Uln,
                                    HasEligibleEmploymentStartDate = false
                                }
                            }
                        }
                    };
            }

            public class WithApplicationWithSomeInEligibleEmployerStartDates
            {
                public Guid ApplicationId = Guid.Parse("18073eb3-f22a-4ab3-9726-dfe99b911d55");

                public long AccountId { get; } = 6007;
                public string HashedAccountId => "MRB49Y";

                public List<LegalEntityDto> LegalEntities => new List<LegalEntityDto> { LegalEntity1 };

                public long AccountLegalEntityId1 => 6008;
                public string HashedAccountLegalEntityId1 => "MYJPDY";
                public LegalEntityDto LegalEntity1 => new LegalEntityDto { AccountId = AccountId, AccountLegalEntityId = AccountLegalEntityId1, LegalEntityName = $"Organisation {AccountLegalEntityId1}" };

                public List<ApprenticeDto> Apprentices => new List<ApprenticeDto> { Apprentice1, Apprentice2, Apprentice3 };
                public ApprenticeDto Apprentice1 => new ApprenticeDto { ApprenticeshipId = 1, FirstName = "Adam", FullName = "Adam 1 Glover", LastName = "Glover", CourseName = "Early Years Educator Level 3", StartDate = new DateTime(2020, 8, 1), Uln = 12345678, EmploymentStartDate = new DateTime(2020, 1, 1) };
                public ApprenticeDto Apprentice2 => new ApprenticeDto { ApprenticeshipId = 2, FirstName = "Mary", FullName = "Mary 2 Lyman", LastName = "Lyman", CourseName = "Assistant accountant Level 3", StartDate = new DateTime(2020, 9, 1), Uln = 23456789, EmploymentStartDate = new DateTime(2021, 6, 1) };
                public ApprenticeDto Apprentice3 => new ApprenticeDto { ApprenticeshipId = 3, FirstName = "Sebastian", FullName = "Sebastian 3 Lawrence", LastName = "Lawrence", CourseName = "General Welder (Arc Processes) Level 2", StartDate = new DateTime(2020, 10, 1), Uln = 34567890, EmploymentStartDate = new DateTime(2020, 1, 1) };

                public ApplicationResponse ApplicationResponse => new ApplicationResponse
                {
                    Application = new IncentiveApplicationDto
                    {
                        AccountLegalEntityId = AccountLegalEntityId1,
                        BankDetailsRequired = true,
                        Apprenticeships = new List<IncentiveApplicationApprenticeshipDto>
                            {
                                new IncentiveApplicationApprenticeshipDto
                                {
                                    ApprenticeshipId = Apprentice1.ApprenticeshipId,
                                    CourseName = Apprentice1.CourseName,
                                    FirstName = Apprentice1.FirstName,
                                    LastName =   Apprentice1.LastName,
                                    TotalIncentiveAmount = 2000m,
                                    PlannedStartDate = Apprentice1.StartDate,
                                    EmploymentStartDate = Apprentice1.EmploymentStartDate,
                                    Uln = Apprentice1.Uln,
                                    HasEligibleEmploymentStartDate = false
                                },
                                new IncentiveApplicationApprenticeshipDto
                                {
                                    ApprenticeshipId = Apprentice2.ApprenticeshipId,
                                    CourseName = Apprentice2.CourseName,
                                    FirstName = Apprentice2.FirstName,
                                    LastName =   Apprentice2.LastName,
                                    TotalIncentiveAmount = 2000m,
                                    PlannedStartDate = Apprentice2.StartDate,
                                    EmploymentStartDate = Apprentice2.EmploymentStartDate,
                                    Uln = Apprentice2.Uln,
                                    HasEligibleEmploymentStartDate = true
                                },
                                new IncentiveApplicationApprenticeshipDto
                                {
                                    ApprenticeshipId = Apprentice3.ApprenticeshipId,
                                    CourseName = Apprentice3.CourseName,
                                    FirstName = Apprentice3.FirstName,
                                    LastName =   Apprentice3.LastName,
                                    TotalIncentiveAmount = 2000m,
                                    PlannedStartDate = Apprentice3.StartDate,
                                    EmploymentStartDate = Apprentice3.EmploymentStartDate,
                                    Uln = Apprentice3.Uln,
                                    HasEligibleEmploymentStartDate = false
                                }
                            }
                    }
                };
            }

            public class WithApplicationWithAllEligibleEmployerStartDates
                {
                    public Guid ApplicationId = Guid.Parse("18073eb3-f22a-4ab3-9726-dfe99b911d56");

                    public long AccountId { get; } = 6009;
                    public string HashedAccountId => "MN47XD";

                    public List<LegalEntityDto> LegalEntities => new List<LegalEntityDto> { LegalEntity1 };

                    public long AccountLegalEntityId1 => 6008;
                    public string HashedAccountLegalEntityId1 => "MYJPDY";
                    public LegalEntityDto LegalEntity1 => new LegalEntityDto { AccountId = AccountId, AccountLegalEntityId = AccountLegalEntityId1, LegalEntityName = $"Organisation {AccountLegalEntityId1}" };

                    public List<ApprenticeDto> Apprentices => new List<ApprenticeDto> { Apprentice1, Apprentice2, Apprentice3 };
                    public ApprenticeDto Apprentice1 => new ApprenticeDto { ApprenticeshipId = 1, FirstName = "Adam", FullName = "Adam 1 Glover", LastName = "Glover", CourseName = "Early Years Educator Level 3", StartDate = new DateTime(2020, 8, 1), Uln = 12345678, EmploymentStartDate = new DateTime(2020, 1, 1) };
                    public ApprenticeDto Apprentice2 => new ApprenticeDto { ApprenticeshipId = 2, FirstName = "Mary", FullName = "Mary 2 Lyman", LastName = "Lyman", CourseName = "Assistant accountant Level 3", StartDate = new DateTime(2020, 9, 1), Uln = 23456789, EmploymentStartDate = new DateTime(2021, 6, 1) };
                    public ApprenticeDto Apprentice3 => new ApprenticeDto { ApprenticeshipId = 3, FirstName = "Sebastian", FullName = "Sebastian 3 Lawrence", LastName = "Lawrence", CourseName = "General Welder (Arc Processes) Level 2", StartDate = new DateTime(2020, 10, 1), Uln = 34567890, EmploymentStartDate = new DateTime(2020, 1, 1) };

                    public ApplicationResponse ApplicationResponse => new ApplicationResponse
                    {
                        Application = new IncentiveApplicationDto
                        {
                            AccountLegalEntityId = AccountLegalEntityId1,
                            BankDetailsRequired = true,
                            Apprenticeships = new List<IncentiveApplicationApprenticeshipDto>
                            {
                                new IncentiveApplicationApprenticeshipDto
                                {
                                    ApprenticeshipId = Apprentice1.ApprenticeshipId,
                                    CourseName = Apprentice1.CourseName,
                                    FirstName = Apprentice1.FirstName,
                                    LastName =   Apprentice1.LastName,
                                    TotalIncentiveAmount = 2000m,
                                    PlannedStartDate = Apprentice1.StartDate,
                                    EmploymentStartDate = Apprentice1.EmploymentStartDate,
                                    Uln = Apprentice1.Uln,
                                    HasEligibleEmploymentStartDate = true
                                },
                                new IncentiveApplicationApprenticeshipDto
                                {
                                    ApprenticeshipId = Apprentice2.ApprenticeshipId,
                                    CourseName = Apprentice2.CourseName,
                                    FirstName = Apprentice2.FirstName,
                                    LastName =   Apprentice2.LastName,
                                    TotalIncentiveAmount = 2000m,
                                    PlannedStartDate = Apprentice2.StartDate,
                                    EmploymentStartDate = Apprentice2.EmploymentStartDate,
                                    Uln = Apprentice2.Uln,
                                    HasEligibleEmploymentStartDate = true
                                },
                                new IncentiveApplicationApprenticeshipDto
                                {
                                    ApprenticeshipId = Apprentice3.ApprenticeshipId,
                                    CourseName = Apprentice3.CourseName,
                                    FirstName = Apprentice3.FirstName,
                                    LastName =   Apprentice3.LastName,
                                    TotalIncentiveAmount = 2000m,
                                    PlannedStartDate = Apprentice3.StartDate,
                                    EmploymentStartDate = Apprentice3.EmploymentStartDate,
                                    Uln = Apprentice3.Uln,
                                    HasEligibleEmploymentStartDate = true
                                }
                            }
                        }
                    };

                }

            public class WithoutASignedAgreement
            {
                public long AccountId { get; } = 30037;
                public string HashedAccountId => "VKGKBB";
                public long AccountLegalEntityId => 30037;
                public string HashedAccountLegalEntityId => "VKGKBB";
                public List<LegalEntityDto> LegalEntities => new List<LegalEntityDto> { LegalEntity };
                public virtual LegalEntityDto LegalEntity => new LegalEntityDto { AccountId = AccountId, AccountLegalEntityId = AccountLegalEntityId, LegalEntityName = "Organisation 30037", IsAgreementSigned = false };
            }

            public class WithInitialApplicationForASingleEntity : WithSingleLegalEntityWithEligibleApprenticeships
            {
                public Guid ApplicationId = Guid.Parse("18073eb3-f22a-4ab3-9726-dfe99b911d53");
                public ApplicationResponse ApplicationResponse => new ApplicationResponse
                {
                    Application = new IncentiveApplicationDto
                    {
                        AccountLegalEntityId = AccountLegalEntityId,
                        NewAgreementRequired = false,
                        Apprenticeships = new IncentiveApplicationApprenticeshipDto[]
                        {
                            new IncentiveApplicationApprenticeshipDto
                            {
                                ApprenticeshipId = 1,
                                CourseName = "Computing...",
                                LastName = "Shipman",
                                FirstName = "Harry",
                                TotalIncentiveAmount = 2000m,
                                HasEligibleEmploymentStartDate = true
                            },
                            new IncentiveApplicationApprenticeshipDto
                            {
                                ApprenticeshipId = 2,
                                CourseName = "T&D ...",
                                LastName = "Leeman",
                                FirstName = "Thomas",
                                TotalIncentiveAmount = 1000m,
                                HasEligibleEmploymentStartDate = true
                            },
                            new IncentiveApplicationApprenticeshipDto
                            {
                                ApprenticeshipId = 3,
                                CourseName = "Water Treatment Technician, Level: 3 (Standard)",
                                LastName = "Johnson",
                                FirstName = "Michael",
                                TotalIncentiveAmount = 2000m,
                                HasEligibleEmploymentStartDate = true
                            },
                            new IncentiveApplicationApprenticeshipDto
                            {
                                ApprenticeshipId = 4,
                                CourseName = "Relationship Manager (Banking), Level: 6 (Standard)",
                                LastName = "Roberts",
                                FirstName = "Jack",
                                TotalIncentiveAmount = 1500m,
                                HasEligibleEmploymentStartDate = true
                            },
                            new IncentiveApplicationApprenticeshipDto
                            {
                                ApprenticeshipId = 5,
                                CourseName = "Non-destructive testing (NDT) operator, Level: 2 (Standard)",
                                LastName = "Smith",
                                FirstName = "Steven",
                                TotalIncentiveAmount = 2000m,
                                HasEligibleEmploymentStartDate = true
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
                            NewAgreementRequired = false,
                            Apprenticeships = new IncentiveApplicationApprenticeshipDto[]
                            {
                                new IncentiveApplicationApprenticeshipDto
                                {
                                    ApprenticeshipId = 1,
                                    CourseName = "Computing...",
                                    LastName = "Shipman",
                                    FirstName = "Harry",
                                    TotalIncentiveAmount = 2000m,
                                    HasEligibleEmploymentStartDate = true
                                },
                                new IncentiveApplicationApprenticeshipDto
                                {
                                    ApprenticeshipId = 2,
                                    CourseName = "T&D ...",
                                    LastName = "Leeman",
                                    FirstName = "Thomas",
                                    TotalIncentiveAmount = 1000m,
                                    HasEligibleEmploymentStartDate = true
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

                public ApplicationResponse GetApplicationResponseWithFirstTwoApprenticesSelectedAndExtensionNotSigned =>
                    new ApplicationResponse
                    {
                        Application = new IncentiveApplicationDto
                        {
                            AccountLegalEntityId = AccountLegalEntityId,
                            NewAgreementRequired = true,
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
            }

            public class WithInitialApplicationAndBankingDetails : WithInitialApplicationForASingleEntity
            {
                public BankingDetailsDto BankingDetails =>
                    new BankingDetailsDto
                    {
                        LegalEntityId = AccountLegalEntityId,
                        SubmittedByName = "Uncle Bob",
                        VendorCode = "000000",
                        ApplicationValue = ApplicationResponse.Application.Apprenticeships.Sum(x => x.TotalIncentiveAmount),
                        SubmittedByEmail = "bob.martin@email.com",
                        SignedAgreements = new List<SignedAgreementDto>
                        {
                            new SignedAgreementDto { SignedByEmail = "jon.skeet@google.com", SignedByName = "Jon Skeet", SignedDate = DateTime.Parse("01-09-2020 12:34:59")}
                        }
                    };
            }
        }
    }
}
