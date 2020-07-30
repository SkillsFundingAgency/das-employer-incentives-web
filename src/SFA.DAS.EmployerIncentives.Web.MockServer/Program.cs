using SFA.DAS.EmployerIncentives.Web.MockServer.EmployerIncentivesApi;
using System;

namespace SFA.DAS.EmployerIncentives.Web.MockServer
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var employerIncenticesApi = EmployerIncentivesApiBuilder
                .Create(8081)
                .WithAccountWithNoLegalEntities()
                .WithAccountWithSingleLegalEntityWithNoEligibleApprenticeships()
                .WithSingleLegalEntityWithEligibleApprenticeships()
                .WithMultipleLegalEntities()
                .WithInitialApplication()
                .WithApplicationConfirmation()
                .Build();

            Console.WriteLine("Press any key to stop the servers");
            Console.ReadKey();

            employerIncenticesApi.Dispose();
        }
    }
}
