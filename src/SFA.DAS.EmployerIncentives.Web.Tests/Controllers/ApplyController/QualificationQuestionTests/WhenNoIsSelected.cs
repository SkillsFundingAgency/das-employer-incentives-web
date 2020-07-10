﻿using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController.QualificationQuestionTests
{
    [TestFixture]
    public class WhenNoIsSelected
    {
        private Web.Controllers.ApplyController _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new Web.Controllers.ApplyController();
        }

        [Test]
        public async Task Then_The_Shutter_Page_Is_Displayed()
        {
            var accountId = "ABC123";
            var viewModel = new QualificationQuestionViewModel { HasTakenOnNewApprentices = false };

            var result = await _sut.QualificationQuestion(accountId, viewModel);

            var redirectResult = result as RedirectToActionResult;
            redirectResult.ActionName.Should().Be("CannotApply");
        }
    }
}