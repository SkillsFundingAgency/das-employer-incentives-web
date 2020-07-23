﻿using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController.SelectNewApprenticeshipsTests
{
    public class WhenSelectingApprentices : ApplyControllerTestBase
    {
        private string _hashedAccountId;
        private string _hashedLegalEntityId;
        private ViewResult _result;
        private IEnumerable<ApprenticeDto> _apprenticeData;
        private SelectApprenticeshipsViewModel _model;
        private ApprenticesQuery _getApprenticesQuery;

        [SetUp]
        public async Task Arrange()
        {
            _apprenticeData = Fixture.CreateMany<ApprenticeDto>();
            _hashedAccountId = Guid.NewGuid().ToString();
            _hashedLegalEntityId = Guid.NewGuid().ToString();

            var accountId = Fixture.Create<long>();
            HashingService.Setup(x => x.DecodeValue(_hashedAccountId)).Returns(accountId);

            var legalEntityId = Fixture.Create<long>();
            HashingService.Setup(x => x.DecodeValue(_hashedLegalEntityId)).Returns(legalEntityId);

            _getApprenticesQuery = It.IsAny<ApprenticesQuery>();
            ApprenticesServiceMock
                .Setup(x => x.Get(It.Is<ApprenticesQuery>(q =>
                    q.AccountId == accountId && q.AccountLegalEntityId == legalEntityId)))
                .ReturnsAsync(_apprenticeData);

            _result = await Sut.SelectApprenticeships(_hashedAccountId, _hashedLegalEntityId);
            _model = (SelectApprenticeshipsViewModel)_result.Model;
        }

        [Test]
        public void Then_page_title_is_set()
        {
            _model.Title.Should().Be(SelectApprenticeshipsViewModel.SelectApprenticeshipsMessage);
        }

        [Test]
        public void Then_accountId_is_set()
        {
            _model.AccountId.Should().Be(_hashedAccountId);
        }

        [Test]
        public void Then_legalEntityId_is_set()
        {
            _model.AccountLegalEntityId.Should().Be(_hashedLegalEntityId);
        }

        [Test]
        public void Then_validation_target_control_is_set()
        {
            _model.FirstCheckboxId.Should().Be("new-apprenticeships-" + _model.Apprenticeships.First().Id);
        }

        [Test]
        public void Then_should_have_no_apprentices_selected_by_default()
        {
            _model.HasSelectedApprenticeships.Should().BeFalse();
        }

        [Test]
        public void Then_should_display_a_list_of_apprentices()
        {
            _model.Apprenticeships.Should()
                .BeEquivalentTo(_apprenticeData,
                    opt => opt
                        .Excluding(x => x.Id)
                        .Excluding(x => x.FullName)
                    );
        }

        [Test]
        public void Then_should_have_apprentices_ordered_by_last_name()
        {
            _model.Apprenticeships.Should().BeInAscendingOrder(x => x.LastName);
        }

        [Test]
        public async Task Then_should_show_error_if_no_selection_is_made()
        {
            _model.SelectedApprenticeships.Clear();

            var result = Sut.SelectApprenticeships(_hashedAccountId, _hashedLegalEntityId, new SelectApprenticeshipsViewModel
            {
                Apprenticeships = _model.Apprenticeships
            });
            var viewResult = await result as ViewResult;

            viewResult.Should().NotBeNull();
            Sut.ViewData.ModelState.IsValid.Should().BeFalse();
            Sut.ViewData.ModelState.Single(x => x.Key == _model.FirstCheckboxId).Value.Errors
                .Should().Contain(x => x.ErrorMessage == SelectApprenticeshipsViewModel.SelectApprenticeshipsMessage);
            viewResult?.ViewName.Should().BeNullOrEmpty();
        }

        [Test]
        public async Task Then_the_Declaration_page_is_displayed()
        {
            _model.SelectedApprenticeships.Add(_model.Apprenticeships.Last().Id);

            var result = Sut.SelectApprenticeships(_hashedAccountId, _hashedLegalEntityId, _model);
            var redirectResult = await result as RedirectToActionResult;

            redirectResult.Should().NotBeNull();
            redirectResult?.ActionName.Should().Be("Declaration");
        }
    }
}