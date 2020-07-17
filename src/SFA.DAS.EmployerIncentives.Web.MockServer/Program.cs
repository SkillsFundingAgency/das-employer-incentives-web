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
                .Build();

            //var employerIncenticesApi2 = EmployerIncentivesApiBuilder
            //    .Create(8082)
            //    .WithAccountWithNoLegalEntities()
            //    .Build();

            Console.WriteLine("Press any key to stop the servers");
            Console.ReadKey();

            employerIncenticesApi.Dispose();
            //employerIncenticesApi2.Dispose();
        }
    }
}
