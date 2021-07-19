using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Cancel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.CancelController.ConfirmApprenticesTests
{
    public class WhenConfirmApprentices
    {
        private string _hashedAccountId;
        private string _hashedAccountLegalEntityId;
        private string _organisationName;
        private string _emailAddress;
        private IEnumerable<ApprenticeshipIncentiveModel> _apprenticeshipIncentiveData;
        private Mock<ILegalEntitiesService> _mockLegalEntitiesService;
        private Mock<IApprenticeshipIncentiveService> _mockApprenticeshipIncentiveService;
        private Web.Controllers.CancelController _sut;
        private IActionResult _result;
        private CancelledApprenticeshipsViewModel _model;

        [SetUp]
        public async Task Arrange()
        {
            _apprenticeshipIncentiveData = new Fixture().CreateMany<ApprenticeshipIncentiveModel>();
            _hashedAccountId = Guid.NewGuid().ToString();
            _hashedAccountLegalEntityId = Guid.NewGuid().ToString();
            _organisationName = Guid.NewGuid().ToString();

            _mockLegalEntitiesService = new Mock<ILegalEntitiesService>();
            _mockLegalEntitiesService
                .Setup(m => m.Get(_hashedAccountId, _hashedAccountLegalEntityId))
                .ReturnsAsync(new LegalEntityModel() { Name = _organisationName });
            
            _mockApprenticeshipIncentiveService = new Mock<IApprenticeshipIncentiveService>();
            _mockApprenticeshipIncentiveService
                .Setup(m => m.GetList(_hashedAccountId, _hashedAccountLegalEntityId))
                .ReturnsAsync(_apprenticeshipIncentiveData);

            var selected = new SelectApprenticeshipsRequest()
            {
                AccountId = _hashedAccountId,
                AccountLegalEntityId = _hashedAccountLegalEntityId,
                SelectedApprenticeships = _apprenticeshipIncentiveData.Where(a => a.Selected).Select(a => a.Id).ToList()
            };

            _emailAddress = "test@one.lv";
            var claims = new[] {new Claim(EmployerClaimTypes.EmailAddress, _emailAddress)};
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            _sut = new Web.Controllers.CancelController(_mockApprenticeshipIncentiveService.Object, _mockLegalEntitiesService.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>()),
                ControllerContext = new ControllerContext {HttpContext = new DefaultHttpContext {User = user}}
            };

            _mockApprenticeshipIncentiveService
                .Setup(m => m.Cancel(_hashedAccountLegalEntityId, _apprenticeshipIncentiveData.Where(a => a.Selected),
                            _hashedAccountId, _emailAddress))
                        .Verifiable();

            _result = await _sut.Cancelled(selected, _hashedAccountId);
            _model = ((ViewResult)_result).Model as CancelledApprenticeshipsViewModel;            
        }

        [Test]
        public async Task Then_redirects_to_payments_page_when_no_apprenticeships_selected()
        {
            // arrange
            var selected = new SelectApprenticeshipsRequest()
            {
                AccountId = _hashedAccountId,
                AccountLegalEntityId = _hashedAccountLegalEntityId,
                SelectedApprenticeships = new List<string>()
            };

            // act
            var result = (await _sut.Cancelled(selected, _hashedAccountId)) as RedirectToActionResult;

            // assert  
            result.ControllerName.Should().Be("Payments");
            result.ActionName.Should().Be("ListPaymentsForLegalEntity");
            result.RouteValues["accountId"].Should().Be(_hashedAccountId);
            result.RouteValues["accountLegalEntityId"].Should().Be(_hashedAccountLegalEntityId);
        }

        [Test]
        public void Then_page_title_is_set()
        {
            _model.Title.Should().Be("Application cancelled");
        }

        [Test]
        public void Then_accountId_is_set()
        {
            _model.AccountId.Should().Be(_hashedAccountId);
        }

        [Test]
        public void Then_legalEntityId_is_set()
        {
            _model.AccountLegalEntityId.Should().Be(_hashedAccountLegalEntityId);
        }

        [Test]
        public void Then_organisationName_is_set()
        {
            _model.OrganisationName.Should().Be(_organisationName);
        }

        [Test]
        public void Then_apprenticeships_are_withdrawn()
        {
            _mockApprenticeshipIncentiveService
               .Verify(m => m.Cancel(_hashedAccountLegalEntityId, 
                   It.Is<IEnumerable<ApprenticeshipIncentiveModel>>(x => VerifySelected(x)),
                   _hashedAccountId,
                   _emailAddress
                   ), Times.Once);
        }

        private bool VerifySelected(IEnumerable<ApprenticeshipIncentiveModel> list)
        {
            try
            {
                list.Should().BeEquivalentTo(_apprenticeshipIncentiveData.Where(a => a.Selected));
                return true;
            }
            catch
            {
                return false;
            }            
        }

        [Test]
        public void Then_should_display_a_list_of_withdrawn_apprentices()
        {
            // assert 
            _model.ApprenticeshipIncentives.Should()
                .BeEquivalentTo(_apprenticeshipIncentiveData.Where(a => a.Selected));
        }
    }
}
