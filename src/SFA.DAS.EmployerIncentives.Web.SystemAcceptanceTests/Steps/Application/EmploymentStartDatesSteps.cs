using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services;
using SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Extensions;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using SFA.DAS.HashingService;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using SFA.DAS.EmployerIncentives.Web.Services.Applications.Types;
using System;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Steps.Application
{
    [Binding]
    [Scope(Feature = "EmploymentStartDates")]
    public class EmploymentStartDatesSteps : StepsBase
    {
        private readonly TestContext _testContext;
        private readonly IHashingService _hashingService;
        private TestData.Account.WithInitialApplicationForASingleEntity _data;
        private HttpResponseMessage _response;
        private LegalEntityDto _legalEntity;

        public EmploymentStartDatesSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
            _hashingService = _testContext.HashingService;
            _data = new TestData.Account.WithInitialApplicationForASingleEntity();
            _legalEntity = _data.LegalEntities.First();
            _testContext.AddOrReplaceClaim(EmployerClaimTypes.Account, _data.HashedAccountId);
        }

        [Given(@"an initial application has been created")]
        public void GivenAnInitialApplicationHasBeenCreated()
        {
            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath(x => x.Contains($"/accounts/{_data.AccountId}/applications/") && !x.Contains("accountlegalentity"))
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(JsonConvert.SerializeObject(_data.GetApplicationResponseWithFirstTwoApprenticesSelected, TestHelper.DefaultSerialiserSettings)));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_data.AccountId}/applications/{_data.ApplicationId}/accountlegalentity")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(_data.AccountLegalEntityId.ToString()));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_data.AccountId}/legalentities/{_legalEntity.AccountLegalEntityId}")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonConvert.SerializeObject(_legalEntity))
                        .WithStatusCode(HttpStatusCode.OK));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath($"/accounts/{_data.AccountId}/applications")
                        .UsingPost()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.Created)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(string.Empty));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath(x =>
                            x.Contains($"accounts/{_data.AccountId}/applications") &&
                            x.Contains("accountlegalentity")) // applicationid is generated in application service so will vary per request
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(_data.AccountLegalEntityId.ToString()));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath(x => x.Contains($"accounts/{_data.AccountId}/applications/{_data.ApplicationId}/apprenticeships"))
                        .UsingPatch()
                    )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                    );
        }

        [Given(@"the employer has selected apprentices for the application")]
        [When(@"the employer has selected apprentices for the application")]
        public async Task WhenTheEmployerHasSelectedApprenticesForTheApplication()
        {
            var apprenticeships = _data.Apprentices.ToApprenticeshipModel(_hashingService).ToArray();
            var url = $"{_data.HashedAccountId}/apply/{_data.HashedAccountLegalEntityId}/select-apprentices";
            var form = new KeyValuePair<string, string>("SelectedApprenticeships", apprenticeships.First().Id);

            _response = await _testContext.WebsiteClient.PostFormAsync(url, form);
        }

        [Then(@"the employer is asked to supply employment start dates for the selected apprentices")]
        public void ThenTheEmployerIsAskedToSupplyEmploymentStartDatesForTheSelectedApprentices()
        {
            _response.RequestMessage.RequestUri.PathAndQuery.Should().StartWith($"/{_data.HashedAccountId}/apply/");
            _response.RequestMessage.RequestUri.PathAndQuery.Should().EndWith("join-organisation");
        }

        [When(@"the employer supplies valid start dates for the selected apprentices")]
        public async Task WhenTheEmployerSuppliesValidStartDatesForTheSelectedApprentices()
        {
            var apprenticeships = _data.Apprentices.ToApprenticeshipModel(_hashingService).ToArray();

            var values = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("AccountId", _data.HashedAccountId),
                new KeyValuePair<string, string>("AccountLegalEntityId", _data.HashedAccountLegalEntityId),
                new KeyValuePair<string, string>("ApplicationId", _data.ApplicationId.ToString()),
                new KeyValuePair<string, string>("ApprenticeshipIds", apprenticeships[0].Id),
                new KeyValuePair<string, string>("EmploymentStartDateDays", "10"),
                new KeyValuePair<string, string>("EmploymentStartDateMonths", "4"),
                new KeyValuePair<string, string>("EmploymentStartDateYears", "2021"),
                new KeyValuePair<string, string>("ApprenticeshipIds", apprenticeships[1].Id),
                new KeyValuePair<string, string>("EmploymentStartDateDays", "20"),
                new KeyValuePair<string, string>("EmploymentStartDateMonths", "5"),
                new KeyValuePair<string, string>("EmploymentStartDateYears", "2021")
            };
            var url = $"{_data.HashedAccountId}/apply/{_data.ApplicationId}/join-organisation";
            
            _response = await _testContext.WebsiteClient.PostFormAsync(url, values.ToArray());
        }

        [Then(@"the employer is asked to confirm their selected apprentices")]
        public void ThenTheEmployerIsAskedToConfirmTheirSelectedApprentices()
        {
            _response.RequestMessage.RequestUri.PathAndQuery.Should().StartWith($"/{_data.HashedAccountId}/apply/confirm-apprentices/");

            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ApplicationConfirmationViewModel;
            model.Should().NotBeNull();
            _response.Should().HaveBackLink($"/{_data.HashedAccountId}/apply/{_data.ApplicationId}/join-organisation");
            model.Should().HaveTitle("Confirm apprentices");
        }

        [When(@"the employer supplies invalid start dates for the selected apprentices")]
        public async Task WhenTheEmployerSuppliesInvalidStartDatesForTheSelectedApprentices()
        {
            var apprenticeships = _data.Apprentices.ToApprenticeshipModel(_hashingService).ToArray();

            var values = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("AccountId", _data.HashedAccountId),
                new KeyValuePair<string, string>("AccountLegalEntityId", _data.HashedAccountLegalEntityId),
                new KeyValuePair<string, string>("ApplicationId", _data.ApplicationId.ToString()),
                new KeyValuePair<string, string>("ApprenticeshipIds", apprenticeships[0].Id),
                new KeyValuePair<string, string>("EmploymentStartDateDays", "32"),
                new KeyValuePair<string, string>("EmploymentStartDateMonths", "4"),
                new KeyValuePair<string, string>("EmploymentStartDateYears", "2021"),
                new KeyValuePair<string, string>("ApprenticeshipIds", apprenticeships[1].Id),
                new KeyValuePair<string, string>("EmploymentStartDateDays", "20"),
                new KeyValuePair<string, string>("EmploymentStartDateMonths", "13"),
                new KeyValuePair<string, string>("EmploymentStartDateYears", "2021")
            };
            var url = $"{_data.HashedAccountId}/apply/{_data.ApplicationId}/join-organisation";

            _response = await _testContext.WebsiteClient.PostFormAsync(url, values.ToArray());
        }

        [When(@"the employer supplied employment start dates fall into next phase window")]
        public async Task WhenTheEmployerSuppliedEmploymentStartDatesFallIntoNextPhaseWindow()
        {
            var response = new ApplicationResponse
            {
                Application = new IncentiveApplicationDto
                {
                    AccountLegalEntityId = _data.AccountLegalEntityId,
                    NewAgreementRequired = true,
                    Apprenticeships = new IncentiveApplicationApprenticeshipDto[0]
                }
            };

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath(x => x.Contains($"accounts/{_data.AccountId}/applications"))
                        .WithParam("includeApprenticeships")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonConvert.SerializeObject(response))
                        .WithStatusCode(HttpStatusCode.OK));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath(x => x.Contains($"accounts/{_data.AccountId}/applications"))
                        .UsingPatch()
                )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonConvert.SerializeObject(_data.ApplicationResponse))
                        .WithStatusCode(HttpStatusCode.OK));

            var apprenticeships = _data.Apprentices.ToApprenticeshipModel(_hashingService).ToArray();

            var values = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("AccountId", _data.HashedAccountId),
                new KeyValuePair<string, string>("AccountLegalEntityId", _data.HashedAccountLegalEntityId),
                new KeyValuePair<string, string>("ApplicationId", _data.ApplicationId.ToString()),
                new KeyValuePair<string, string>("ApprenticeshipIds", apprenticeships[0].Id),
                new KeyValuePair<string, string>("EmploymentStartDateDays", "1"),
                new KeyValuePair<string, string>("EmploymentStartDateMonths", "9"),
                new KeyValuePair<string, string>("EmploymentStartDateYears", "2021"),
                new KeyValuePair<string, string>("ApprenticeshipIds", apprenticeships[1].Id),
                new KeyValuePair<string, string>("EmploymentStartDateDays", "1"),
                new KeyValuePair<string, string>("EmploymentStartDateMonths", "10"),
                new KeyValuePair<string, string>("EmploymentStartDateYears", "2021")
            };
            var url = $"{_data.HashedAccountId}/apply/{_data.ApplicationId}/join-organisation";

            _response = await _testContext.WebsiteClient.PostFormAsync(url, values.ToArray());
        }

        [When(@"the employer supplies some ineligible start dates for the selected apprentices")]
        public async Task WhenTheEmployerSuppliesSomeIneligibleStartDatesForTheSelectedApprentices()
        {
            var response = new ApplicationResponse
            {
                Application = new IncentiveApplicationDto
                {
                    AccountLegalEntityId = _data.AccountLegalEntityId,
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
                                EmploymentStartDate = new DateTime(2021, 05, 01),
                                StartDatesAreEligible = false
                            },
                            new IncentiveApplicationApprenticeshipDto
                            {
                                ApprenticeshipId = 2,
                                CourseName = "T&D ...",
                                LastName = "Leeman",
                                FirstName = "Thomas",
                                TotalIncentiveAmount = 1000m,
                                EmploymentStartDate = new DateTime(2021, 05, 01),
                                StartDatesAreEligible = true
                            },
                            new IncentiveApplicationApprenticeshipDto
                            {
                                ApprenticeshipId = 3,
                                CourseName = "Water Treatment Technician, Level: 3 (Standard)",
                                LastName = "Johnson",
                                FirstName = "Michael",
                                TotalIncentiveAmount = 2000m,
                                EmploymentStartDate = new DateTime(2021, 05, 01),
                                StartDatesAreEligible = false
                            }
                        }
                }
            };

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath(x => x.Contains($"accounts/{_data.AccountId}/applications"))
                        .WithParam("includeApprenticeships")
                        .UsingGet()
                 )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonConvert.SerializeObject(response))
                        .WithStatusCode(HttpStatusCode.OK));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath(x => x.Contains($"accounts/{_data.AccountId}/applications"))
                        .UsingPatch()
                    )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonConvert.SerializeObject(_data.ApplicationResponse))
                        .WithStatusCode(HttpStatusCode.OK));

            await WhenTheEmployerSuppliesValidStartDatesForTheSelectedApprentices();
        }

        [When(@"the employer supplies all ineligible start dates for the selected apprentices")]
        public async Task WhenTheEmployerSuppliesAllIneligibleStartDatesForTheSelectedApprentices()
        {
            var response = new ApplicationResponse
            {
                Application = new IncentiveApplicationDto
                {
                    AccountLegalEntityId = _data.AccountLegalEntityId,
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
                                EmploymentStartDate = new DateTime(2021, 05, 01),
                                StartDatesAreEligible = false
                            },
                            new IncentiveApplicationApprenticeshipDto
                            {
                                ApprenticeshipId = 2,
                                CourseName = "T&D ...",
                                LastName = "Leeman",
                                FirstName = "Thomas",
                                TotalIncentiveAmount = 1000m,
                                EmploymentStartDate = new DateTime(2021, 05, 01),
                                StartDatesAreEligible = false
                            },
                            new IncentiveApplicationApprenticeshipDto
                            {
                                ApprenticeshipId = 3,
                                CourseName = "Water Treatment Technician, Level: 3 (Standard)",
                                LastName = "Johnson",
                                FirstName = "Michael",
                                TotalIncentiveAmount = 2000m,
                                EmploymentStartDate = new DateTime(2021, 05, 01),
                                StartDatesAreEligible = false
                            }
                        }
                }
            };

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath(x => x.Contains($"accounts/{_data.AccountId}/applications"))
                        .WithParam("includeApprenticeships")
                        .UsingGet()
                 )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonConvert.SerializeObject(response))
                        .WithStatusCode(HttpStatusCode.OK));

            _testContext.EmployerIncentivesApi.MockServer
                .Given(
                    Request
                        .Create()
                        .WithPath(x => x.Contains($"accounts/{_data.AccountId}/applications"))
                        .UsingPatch()
                    )
                .RespondWith(
                    Response.Create()
                        .WithBody(JsonConvert.SerializeObject(_data.ApplicationResponse))
                        .WithStatusCode(HttpStatusCode.OK));

            await WhenTheEmployerSuppliesValidStartDatesForTheSelectedApprentices();
        }

        [Then(@"the employer is asked to change their submitted dates")]
        public void ThenTheEmployerIsAskedToChangeTheirSubmittedDates()
        {
            _response.RequestMessage.RequestUri.PathAndQuery.Should().Be($"/{_data.HashedAccountId}/apply/{_data.ApplicationId}/join-organisation");

            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as EmploymentStartDatesViewModel;
            model.Should().NotBeNull();
            model.DateValidationResults.Should().NotBeEmpty();
            _response.Should().HaveBackLink($"/{_data.HashedAccountId}/apply/select-apprentices/{_data.ApplicationId}");
        }

        [Then(@"the employer is informed one more of their selected apprentices are ineligible")]
        public void ThenTheEmployerIsInformedOneOrMoreOfTheirSelectedApprenticesAreInEligible()
        {
            _response.RequestMessage.RequestUri.PathAndQuery.Should().Be($"/{_data.HashedAccountId}/apply/confirm-apprentices/{_data.ApplicationId}");

            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as NotEligibleViewModel;
            model.Should().NotBeNull();
            model.Should().HaveTitle("Not eligible for the payment");
            model.AllInEligible.Should().BeFalse();
            model.Apprentices.Count.Should().Be(2);
            _response.Should().HaveLink("[data-linktype='noneligible-continue']", $"/{_data.HashedAccountId}/apply/confirm-apprentices/{_data.ApplicationId}?all=false");
            _response.Should().HaveLink("[data-linktype='noneligible-change']", $"/{_data.HashedAccountId}/apply/select-apprentices/{_data.ApplicationId}");
            _response.Should().HaveBackLink($"/{_data.HashedAccountId}/apply/{_data.ApplicationId}/join-organisation");
        }

        [Then(@"the employer is informed all of their selected apprentices are ineligible")]
        public void ThenTheEmployerIsInformedAllOfTheirSelectedApprenticesAreInEligible()
        {
            _response.RequestMessage.RequestUri.PathAndQuery.Should().Be($"/{_data.HashedAccountId}/apply/confirm-apprentices/{_data.ApplicationId}");

            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as NotEligibleViewModel;
            model.Should().NotBeNull();
            model.Should().HaveTitle("Not eligible for the payment");
            model.AllInEligible.Should().BeTrue();
            model.Apprentices.Count.Should().Be(3);
            _response.Should().HaveLink("[data-linktype='noneligible-cancel']", $"/{_data.HashedAccountId}/{_data.HashedAccountLegalEntityId}/hire-new-apprentice-payment");
            _response.Should().HaveLink("[data-linktype='noneligible-change']", $"/{_data.HashedAccountId}/apply/select-apprentices/{_data.ApplicationId}");
            _response.Should().HaveBackLink($"/{_data.HashedAccountId}/apply/{_data.ApplicationId}/join-organisation");
        }

        [Then(@"the employer is offered the option to change their employment start dates")]
        public void ThenTheEmployerIsOfferedTheOptionToChangeTheirEmploymentStartDates()
        {
            _response.Should().HaveLink("[data-linktype='noneligible-change']", $"/{_data.HashedAccountId}/apply/select-apprentices/{_data.ApplicationId}");
        }


        [Given(@"the employer is informed one or more of their selected apprentices are ineligible")]
        public async Task GivenTheEmployerIsInformedOneOrMoreOfTheirSelectedApprenticesAreIneligible()
        {
            await WhenTheEmployerSuppliesSomeIneligibleStartDatesForTheSelectedApprentices();
        }

        [When(@"the employer accepts to remove ineligible apprenticeships from the application")]
        public async Task WhenTheEmployerAcceptsToRemoveIneligibleApprenticeshipsFromTheApplication()
        {
            var url = $"/{_data.HashedAccountId}/apply/confirm-apprentices/{_data.ApplicationId}?all=false";
            _response = await _testContext.WebsiteClient.GetAsync(url);
        }

        [Then(@"the employer is asked to confirm their apprenticeships selection")]
        public void ThenTheEmployerIsAskedToConfirmTheirApprenticeshipsSelection()
        {
            var viewResult = _testContext.ActionResult.LastViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as ApplicationConfirmationViewModel;
            model.Should().NotBeNull();
            _response.Should().HaveBackLink($"/{_data.HashedAccountId}/apply/confirm-apprentices/{_data.ApplicationId}?all=true");
            model.Should().HaveTitle("Confirm apprentices");
        }

        [Then(@"the employer is asked to sign the agreement variation")]
        public void ThenTheEmployerIsAskedToSignTheAgreementVariation()
        {
            _response.EnsureSuccessStatusCode();
            _response.RequestMessage.RequestUri.PathAndQuery.Should().Be($"/{_data.HashedAccountId}/apply/{_data.ApplicationId}/join-organisation");

            var viewResult = _testContext.ActionResult.LastViewResult;

            viewResult.Should().NotBeNull();
            var model = viewResult.Model as NewAgreementRequiredViewModel;
            model.Should().NotBeNull();


            model.Should().HaveTitle($"{_legalEntity.LegalEntityName} needs to accept a new employer agreement");
            model.AccountId.Should().Be(_data.HashedAccountId);
        }

    }
}
