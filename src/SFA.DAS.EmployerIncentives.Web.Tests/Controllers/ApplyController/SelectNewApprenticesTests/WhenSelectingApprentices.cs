using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController.SelectNewApprenticesTests
{
    public class WhenSelectingApprentices : ApplyControllerTestBase
    {
        private string _accountId;
        private ViewResult _result;
        private IEnumerable<ApprenticeModel> _apprenticeData;
        private SelectApprenticesViewModel _model;

        [SetUp]
        public async Task Arrange()
        {
            var fixture = new Fixture();
            _apprenticeData = fixture.CreateMany<ApprenticeModel>();
            ServiceMock.Setup(x => x.GetSampleApprentices()).Returns(_apprenticeData);
            _accountId = Guid.NewGuid().ToString();
            _result = await Sut.SelectApprentices(_accountId);
            _model = (SelectApprenticesViewModel)_result.Model;
        }

        [Test]
        public void Then_page_title_is_set()
        {
            _model.Title.Should().Be(SelectApprenticesViewModel.SelectApprenticesMessage);
        }

        [Test]
        public void Then_accountId_is_set()
        {
            _model.AccountId.Should().Be(_accountId);
        }

        [Test]
        public void Then_validation_target_control_is_set()
        {
            _model.FirstCheckboxId.Should().Be("new-apprentices-" + _model.Apprentices.First().Id);
        }

        [Test]
        public void Then_should_have_no_apprentices_selected_by_default()
        {
            _model.HasSelectedApprentices.Should().BeFalse();
        }

        [Test]
        public void Then_should_display_a_list_of_apprentices()
        {
            _model.Apprentices.Should().BeEquivalentTo(_apprenticeData);
        }

        [Test]
        public void Then_should_have_apprentices_ordered_by_last_name()
        {
            _model.Apprentices.Should().BeInAscendingOrder(x => x.LastName);
        }

        [Test]
        public async Task Then_should_show_error_if_no_selection_is_made()
        {
            _model.SelectedApprentices.Clear();

            var result = Sut.SelectApprentices(_accountId, new SelectApprenticesViewModel());
            var viewResult = await result as ViewResult;

            viewResult.Should().NotBeNull();
            Sut.ViewData.ModelState.IsValid.Should().BeFalse();
            Sut.ViewData.ModelState.Single(x => x.Key == _model.FirstCheckboxId).Value.Errors
                .Should().Contain(x => x.ErrorMessage == SelectApprenticesViewModel.SelectApprenticesMessage);
            viewResult?.ViewName.Should().BeNullOrEmpty();
        }

        [Test]
        public async Task Then_the_Declaration_page_is_displayed()
        {
            _model.SelectedApprentices.Add(_model.Apprentices.Last().Id);

            var result = Sut.SelectApprentices(_accountId, _model);
            var redirectResult = await result as RedirectToActionResult;

            redirectResult.Should().NotBeNull();
            redirectResult?.ActionName.Should().Be("Declaration");
        }
    }
}