using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Applications;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.PaymentsController
{
    public class WhenPaymentApplicationsAccessed
    {
        private Web.Controllers.PaymentsController _sut;
        private Mock<IApplicationService> _service;
        private Fixture _fixture;
        private string _accountId;

        [SetUp]
        public void Arrange()
        {
            _service = new Mock<IApplicationService>();
            _sut = new Web.Controllers.PaymentsController(_service.Object);
            _fixture = new Fixture();
            _accountId = _fixture.Create<string>();
        }

        [Test]
        public async Task Then_the_view_contains_summary_for_submitted_applications()
        {
            // Arrange
            var applications = new List<ApprenticeApplicationModel>();
            applications.AddRange(_fixture.CreateMany<ApprenticeApplicationModel>(5));
            foreach(var application in applications)
            {
                application.Status = "Submitted";
            }
            _service.Setup(x => x.GetList(_accountId)).ReturnsAsync(applications);

            var result = await _sut.ListPayments(_accountId) as ViewResult;

            var viewModel = result.Model as ViewApplicationsViewModel;
            viewModel.Should().NotBeNull();
            viewModel.Applications.Count().Should().Be(applications.Count());
        }

        [Test]
        public async Task Then_the_view_contains_summary_for_only_submitted_applications()
        {
            // Arrange
            var applications = new List<ApprenticeApplicationModel>();
            applications.AddRange(_fixture.CreateMany<ApprenticeApplicationModel>(5));
            applications[2].Status = "Submitted";
            applications[4].Status = "Submitted";

            _service.Setup(x => x.GetList(_accountId)).ReturnsAsync(applications);

            var result = await _sut.ListPayments(_accountId) as ViewResult;

            var viewModel = result.Model as ViewApplicationsViewModel;
            viewModel.Should().NotBeNull();
            viewModel.Applications.Count().Should().Be(applications.Count(x => x.Status == "Submitted"));
        }
    }
}
