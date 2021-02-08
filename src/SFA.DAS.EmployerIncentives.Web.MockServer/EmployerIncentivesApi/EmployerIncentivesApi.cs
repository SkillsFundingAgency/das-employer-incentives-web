using Newtonsoft.Json;
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
using System.Security.Claims;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications.Types;

namespace SFA.DAS.EmployerIncentives.Web.MockServer.EmployerIncentivesApi
{
    public class EmployerIncentivesApiBuilder
    {
        private readonly WireMockServer _server;
        private readonly List<Claim> _claims;

        public static EmployerIncentivesApiBuilder Create(int port)
        {
            return new EmployerIncentivesApiBuilder(port);
        }

        private EmployerIncentivesApiBuilder(int port)
        {
            _claims = new List<Claim>();
            _server = WireMockServer.StartWithAdminInterface(port);
        }
        public EmployerIncentivesApiBuilder WithAccountOwnerUserId()
        {
            var userId = TestData.User.AccountOwnerUserId;
            AddOrReplaceClaim(EmployerClaimTypes.UserId, userId.ToString());
            return this;
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

            AddClaim(EmployerClaimTypes.Account, data.HashedAccountId);

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

            AddClaim(EmployerClaimTypes.Account, data.HashedAccountId);

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

            AddClaim(EmployerClaimTypes.Account, data.HashedAccountId);

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

            AddClaim(EmployerClaimTypes.Account, data.HashedAccountId);

            return this;
        }

        public EmployerIncentivesApiBuilder WithPreviousApplications()
        {
            var data = new TestData.Account.WithPreviousApplicationsForFirstLegalEntity();

            var applications = new List<ApprenticeApplicationModel> { data.Application1, data.Application2, data.Application3, data.Application4, data.Application5 };
            var getApplicationsResponse = new GetApplicationsModel { ApprenticeApplications = applications, BankDetailsStatus = BankDetailsStatus.NotSupplied };
            _server
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{data.AccountId}/legalentity/{data.AccountLegalEntityId1}/applications")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithBody(JsonConvert.SerializeObject(getApplicationsResponse)));

            AddClaim(EmployerClaimTypes.Account, data.HashedAccountId);

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
                      .WithPath($"/apprenticeships")
                      .WithParam("accountid", data.AccountId.ToString())
                      .WithParam("accountlegalentityid", data.LegalEntities.First().AccountLegalEntityId.ToString())
                      .UsingGet()
                      )
                  .RespondWith(
              Response.Create()
                  .WithBody(JsonConvert.SerializeObject(data.Apprentices, TestHelper.DefaultSerialiserSettings))
                  .WithStatusCode(HttpStatusCode.OK));

            AddClaim(EmployerClaimTypes.Account, data.HashedAccountId);

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
                        .WithPath(x => x.Contains($"/accounts/{data.AccountId}/applications/") && !x.Contains("accountlegalentity"))
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
                        .WithPath(x => x.Contains($"/accounts/{data.AccountId}/applications") && x.Contains("accountlegalentity"))
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(data.AccountLegalEntityId.ToString())
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

            AddClaim(EmployerClaimTypes.Account, data.HashedAccountId);

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

            AddClaim(EmployerClaimTypes.Account, data.HashedAccountId);

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

            AddClaim(EmployerClaimTypes.Account, data.HashedAccountId);

            return this;
        }

        public EmployerIncentivesApiBuilder WithBankingDetails()
        {
            var data = new TestData.Account.WithInitialApplicationAndBankingDetails();

            _server
                .Given(
                    Request
                        .Create()
                        .WithPath($"/email/bank-details-reminder")
                        .UsingPost()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(string.Empty));

            _server
                .Given(
                    Request
                        .Create()
                        .WithPath(x => x.Contains($"accounts/{data.AccountId}/applications/") && x.Contains("/bankingDetails"))
                        .WithParam("hashedAccountId", $"{data.HashedAccountId}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonConvert.SerializeObject(data.BankingDetails, TestHelper.DefaultSerialiserSettings))
                        .WithStatusCode(HttpStatusCode.OK));

            return this;
        }

        public EmployerIncentivesApiBuilder WithCreateApplication()
        {
            var data = new TestData.Account.WithMultipleLegalEntitiesWithEligibleApprenticeships();
            _server
               .Given(
                   Request
                       .Create()
                       .WithPath(x => x.Contains($"accounts/{data.AccountId}/applications"))
                       .UsingPost()
               )
               .RespondWith(
                   Response.Create()
                       .WithBody(string.Empty)
                       .WithStatusCode(HttpStatusCode.OK));

            _server
               .Given(
                   Request
                       .Create()
                       .WithPath(x => x.Contains($"accounts/{data.AccountId}/applications"))
                       .UsingPatch()
               )
               .RespondWith(
                   Response.Create()
                       .WithBody(string.Empty)
                       .WithStatusCode(HttpStatusCode.OK));

            var applicationResponse = new ApplicationResponse 
            {
                Application = new IncentiveApplicationDto
                {
                    AccountLegalEntityId = 123,
                    BankDetailsRequired = true,
                    Apprenticeships = new List<IncentiveApplicationApprenticeshipDto>
                    {
                        new IncentiveApplicationApprenticeshipDto
                        {
                            ApprenticeshipId = 1234,
                            CourseName = "Early Years Educator Level 3",
                            FirstName = "Adam",
                            LastName = "Glover",
                            TotalIncentiveAmount = 2000m,
                            PlannedStartDate = new DateTime(2020, 9, 1),
                            Uln = 12345678
                        }
                    }
                }
            };

            _server
                .Given(
                    Request
                        .Create()
                        .WithPath(x => x.Contains($"accounts/{data.AccountId}/applications"))
                        .WithParam("includeApprenticeships")
                        .UsingGet()
                 )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonConvert.SerializeObject(applicationResponse))
                        .WithStatusCode(HttpStatusCode.OK));

            _server
                .Given(
                    Request
                        .Create()
                        .WithPath(x => x.Contains($"accounts/{data.AccountId}/applications") && x.Contains("accountlegalentity"))
                        .UsingGet()
                 )
                .RespondWith(
                    Response.Create()
                        .WithBody("123")
                        .WithStatusCode(HttpStatusCode.OK));

            return this;
        }

        public EmployerIncentivesApi Build()
        {
            _server.LogEntriesChanged += _server_LogEntriesChanged;
            return new EmployerIncentivesApi(_server, _claims);
        }

        private void AddClaim(string type, string value)
        {
            _claims.Add(new Claim(type, value));
        }

        private void AddOrReplaceClaim(string type, string value)
        {
            var existing = _claims.SingleOrDefault(c => c.Type == type);
            if (existing != null)
            {
                _claims.Remove(existing);
            }
            _claims.Add(new Claim(type, value));
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
        public List<Claim> Claims { get; }

        public EmployerIncentivesApi(WireMockServer server, List<Claim> claims)
        {
            _server = server;
            Claims = claims;
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
