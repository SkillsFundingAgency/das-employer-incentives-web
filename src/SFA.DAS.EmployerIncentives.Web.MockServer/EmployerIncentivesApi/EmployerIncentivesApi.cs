using Newtonsoft.Json;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using WireMock.Logging;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests;
using System.Linq;

namespace SFA.DAS.EmployerIncentives.Web.MockServer.EmployerIncentivesApi
{
    public class EmployerIncentivesApiBuilder
    {
        private readonly WireMockServer _server;
        public static EmployerIncentivesApiBuilder Create(int port)
        {
            return new EmployerIncentivesApiBuilder(port);
        }

        private EmployerIncentivesApiBuilder(int port)
        { 
            _server = WireMockServer.StartWithAdminInterface(port);
        }

        public EmployerIncentivesApiBuilder WithAccountWithNoLegalEntities()
        {
            var data = new TestData.Account.WithNoLegalEntites();

            _server
            .Given(
                    Request
                    .Create()
                    .WithPath($"/accounts/{data.AccountId}/legalentities")
                    .UsingGet()
                    )
                .RespondWith(
            Response.Create()
                .WithStatusCode(HttpStatusCode.NotFound));

            return this;
        }

        public EmployerIncentivesApiBuilder WithAccountWithSingleLegalEntityWithNoEligibleApprenticeships()
        {
            var data = new TestData.Account.WithSingleLegalEntityWithNoEligibleApprenticeships();

            _server
            .Given(
                    Request
                    .Create()
                    .WithPath($"/accounts/{data.AccountId}/legalentities")
                    .UsingGet()
                    )
                .RespondWith(
            Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBody(JsonConvert.SerializeObject(new List<LegalEntityDto>() { data.LegalEntity })));

            _server
              .Given(
                      Request
                      .Create()
                      .WithPath($"/apprenticeships")
                      .WithParam("accountid", data.AccountId.ToString())
                      .WithParam("accountlegalentityid", data.LegalEntity.AccountLegalEntityId.ToString())
                      .UsingGet()
                      )
                  .RespondWith(
              Response.Create()
                  .WithStatusCode(HttpStatusCode.NotFound));

            return this;
        }

        public EmployerIncentivesApiBuilder WithSingleLegalEntityWithEligibleApprenticeships()
        {
            var data = new TestData.Account.WithSingleLegalEntityWithEligibleApprenticeships();

            _server
            .Given(
                    Request
                    .Create()
                    .WithPath($"/accounts/{data.AccountId}/legalentities")
                    .UsingGet()
                    )
                .RespondWith(
            Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBody(JsonConvert.SerializeObject(data.LegalEntities)));

            _server
              .Given(
                      Request
                      .Create()
                      .WithPath($"/apprenticeships")
                      .WithParam("accountid", data.AccountId.ToString())
                      .WithParam("accountlegalentityid", data.LegalEntities.First().AccountLegalEntityId.ToString())
                      .UsingGet()
                      )
                  .RespondWith(
              Response.Create()
                  .WithBody(JsonConvert.SerializeObject(data.Apprentices))
                  .WithStatusCode(HttpStatusCode.OK));

            return this;
        }

        public EmployerIncentivesApi Build()
        {
            _server.LogEntriesChanged += _server_LogEntriesChanged;
            return new EmployerIncentivesApi(_server);
        }

        private void _server_LogEntriesChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (LogEntry newItem in e.NewItems)
            {
                Debug.WriteLine("============================= TestEmployerIncentivesApi MockServer called ================================");
                Debug.WriteLine(JsonConvert.SerializeObject(TestHelper.Map(newItem), Formatting.Indented));
                Debug.WriteLine("==========================================================================================================");
            }
        }
    }

#pragma warning disable S3881 // "IDisposable" should be implemented correctly
    public class EmployerIncentivesApi : IDisposable
#pragma warning restore S3881 // "IDisposable" should be implemented correctly
    {
        private readonly WireMockServer _server;
        public EmployerIncentivesApi(WireMockServer server)
        {
            _server = server; 
        }

        public void Dispose()
        {
            if(_server.IsStarted)
            {
                _server.Stop();                
            }
        }
    }
}
