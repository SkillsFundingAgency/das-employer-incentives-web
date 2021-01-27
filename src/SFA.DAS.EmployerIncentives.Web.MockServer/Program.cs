using SFA.DAS.EmployerIncentives.Web.MockServer.CosmosDb;
using SFA.DAS.EmployerIncentives.Web.MockServer.EmployerIncentivesApi;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.MockServer
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var employerIncentivesApi = EmployerIncentivesApiBuilder
                .Create(8083)
                .WithAccountWithNoLegalEntities()
                .WithAccountWithSingleLegalEntityWithNoEligibleApprenticeships()
                .WithSingleLegalEntityWithEligibleApprenticeships()
                .WithMultipleLegalEntities()
                .WithMultipleLegalEntityWithEligibleApprenticeships()
                .WithInitialApplication()
                .WithoutASignedAgreement()
                .WithApplicationConfirmation()
                .WithAccountOwnerUserId()
                .WithBankingDetails()
                .WithPreviousApplications()
                .WithCreateApplication()
                .Build();

            var readStore = await AccountsReadStoreBuilder.Create();
            await readStore.WithAccountForAccountOwnerUserId(new TestData.Account.WithSingleLegalEntityWithEligibleApprenticeships().AccountId);
            readStore.Build();

            var webSite = new LocalWebSite(employerIncentivesApi.Claims)
                .Build()
                .Run();

            Console.WriteLine("Press any key to stop the servers");
            Console.ReadKey();

            employerIncentivesApi.Dispose();
            webSite.Dispose();
        }
    }
}
