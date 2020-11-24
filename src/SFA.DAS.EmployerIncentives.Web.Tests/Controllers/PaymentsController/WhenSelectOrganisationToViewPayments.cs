
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Applications;
using SFA.DAS.HashingService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.PaymentsController
{
    [TestFixture]
    public class WhenSelectOrganisationToViewPayments
    {
        private Web.Controllers.PaymentsController _sut;
        private Mock<IApplicationService> _applicationService;
        private Mock<ILegalEntitiesService> _legalEntitiesService;
        private Mock<IHashingService> _hashingService;
        private Fixture _fixture;
        private string _accountId;

        [SetUp]
        public void Arrange()
        {
            _applicationService = new Mock<IApplicationService>();
            _legalEntitiesService = new Mock<ILegalEntitiesService>();
            _hashingService = new Mock<IHashingService>();
            _sut = new Web.Controllers.PaymentsController(_applicationService.Object, _legalEntitiesService.Object, _hashingService.Object);
            _fixture = new Fixture();
            _accountId = _fixture.Create<string>();
        }

        [Test]
        public async Task Then_the_payments_are_displayed_if_the_account_only_has_one_organisation()
        {
            var legalEntities = new List<LegalEntityModel> { new LegalEntityModel { AccountId = _accountId, AccountLegalEntityId = _fixture.Create<string>() } };
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            var result = await _sut.ListPayments(_accountId, string.Empty, string.Empty);

            var redirectResult = result as RedirectToActionResult;
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be("ListPaymentsForLegalEntity");
        }

        [Test]
        public async Task Then_the_organisations_associated_with_the_account_are_presented_to_choose_from()
        {
            var legalEntities = new List<LegalEntityModel> 
            { 
                new LegalEntityModel { AccountId = _accountId, AccountLegalEntityId = _fixture.Create<string>() },
                new LegalEntityModel { AccountId = _accountId, AccountLegalEntityId = _fixture.Create<string>() },
                new LegalEntityModel { AccountId = _accountId, AccountLegalEntityId = _fixture.Create<string>() }
            };
            _legalEntitiesService.Setup(x => x.Get(_accountId)).ReturnsAsync(legalEntities);

            var result = await _sut.ListPayments(_accountId, string.Empty, string.Empty);

            var redirectResult = result as RedirectToActionResult;
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be("ChooseOrganisation");
        }

        [Test]
        public async Task Then_the_payments_are_shown_for_the_selected_account_organisation()
        {
            var accountLegalEntityId = _fixture.Create<string>();
            var model = new ChooseOrganisationViewModel 
            { 
                AccountId = _accountId,
                Selected = accountLegalEntityId
            };

            var result = await _sut.ChooseOrganisation(model);

            var redirectResult = result as RedirectToActionResult;
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be("ListPaymentsForLegalEntity");
            redirectResult.RouteValues["accountId"].Should().Be(_accountId);
            redirectResult.RouteValues["accountLegalEntityId"].Should().Be(accountLegalEntityId);
        }
    }
}
