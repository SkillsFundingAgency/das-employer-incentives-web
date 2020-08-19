using Newtonsoft.Json;
using SFA.DAS.EmployerIncentives.Web.Services;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using WireMock.Logging;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

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
                .WithBody(JsonConvert.SerializeObject(new List<LegalEntityDto>() { data.LegalEntity }, TestHelper.DefaultSerialiserSettings)));

            _server
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{data.AccountId}/legalentities/{data.AccountLegalEntityId}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonConvert.SerializeObject(data.LegalEntity, TestHelper.DefaultSerialiserSettings)));

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
                .WithBody(JsonConvert.SerializeObject(data.LegalEntities, TestHelper.DefaultSerialiserSettings)));

            _server
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{data.AccountId}/legalentities/{data.AccountLegalEntityId}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonConvert.SerializeObject(data.LegalEntity, TestHelper.DefaultSerialiserSettings)));

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
                  .WithBody(JsonConvert.SerializeObject(data.Apprentices, TestHelper.DefaultSerialiserSettings))
                  .WithStatusCode(HttpStatusCode.OK));

            return this;
        }

        public EmployerIncentivesApiBuilder WithMultipleLegalEntities()
        {
            var data = new TestData.Account.WithMultipleLegalEntitiesWithNoEligibleApprenticeships();

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
                .WithBody(JsonConvert.SerializeObject(data.LegalEntities, TestHelper.DefaultSerialiserSettings)));

            _server
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{data.AccountId}/legalentities/{data.AccountLegalEntityId1}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonConvert.SerializeObject(data.LegalEntity1, TestHelper.DefaultSerialiserSettings)));

            _server
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{data.AccountId}/legalentities/{data.AccountLegalEntityId2}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonConvert.SerializeObject(data.LegalEntity2, TestHelper.DefaultSerialiserSettings)));

            _server
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{data.AccountId}/legalentities/{data.AccountLegalEntityId3}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonConvert.SerializeObject(data.LegalEntity3, TestHelper.DefaultSerialiserSettings)));

            return this;
        }

        public EmployerIncentivesApiBuilder WithMultipleLegalEntityWithEligibleApprenticeships()
        {
            var data = new TestData.Account.WithMultipleLegalEntitiesWithEligibleApprenticeships();

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
                .WithBody(JsonConvert.SerializeObject(data.LegalEntities, TestHelper.DefaultSerialiserSettings)));

            _server
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{data.AccountId}/legalentities/{data.AccountLegalEntityId1}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonConvert.SerializeObject(data.LegalEntity1, TestHelper.DefaultSerialiserSettings)));

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
                  .WithBody(JsonConvert.SerializeObject(data.Apprentices, TestHelper.DefaultSerialiserSettings))
                  .WithStatusCode(HttpStatusCode.OK));

            return this;
        }

        public EmployerIncentivesApiBuilder WithInitialApplication()
        {
            var data = new TestData.Account.WithInitialApplicationForASingleEntity();

            _server
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{data.AccountId}/applications")
                        .UsingPost()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.Created));

            _server
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{data.AccountId}/applications/*")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(JsonConvert.SerializeObject(data.ApplicationResponse, TestHelper.DefaultSerialiserSettings))
                    );

            _server
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{data.AccountId}/applications")
                        .UsingPut()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK));

            return this;
        }

        public EmployerIncentivesApiBuilder WithoutASignedAgreement()
        {
            var data = new TestData.Account.WithoutASignedAgreement();

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
                        .WithBody(JsonConvert.SerializeObject(data.LegalEntities, TestHelper.DefaultSerialiserSettings)));

            _server
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{data.AccountId}/legalentities/{data.AccountLegalEntityId}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonConvert.SerializeObject(data.LegalEntities.First(), TestHelper.DefaultSerialiserSettings)));

            return this;
        }

        public EmployerIncentivesApiBuilder WithApplicationConfirmation()
        {
            var data = new TestData.Account.WithInitialApplicationForASingleEntity();

            _server
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{data.AccountId}/applications")
                        .UsingPatch()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK));

            return this;
        }

        public EmployerIncentivesApiBuilder WithBankingDetails()
        {
            var data = new TestData.Account.WithInitialApplicationAndBankingDetails();
            var url = OuterApiRoutes.GetBankingDetailsUrl(data.AccountId, data.ApplicationId, data.HashedAccountId);

            _server
                .Given(
                    Request
                        .Create()
                        .WithPath(url)
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonConvert.SerializeObject(data.BankingDetails, TestHelper.DefaultSerialiserSettings))
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
            if (_server.IsStarted)
            {
                _server.Stop();
            }
        }
    }
}
