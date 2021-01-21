using System;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Models
{
    [TestFixture]
    public class ApplicationConfirmationViewModelTests
    {
        private Fixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void When_new_agreement_required_confirm_action_should_be_NewAgreementRequired()
        {
            var model = new ApplicationConfirmationViewModel(_fixture.Create<Guid>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.CreateMany<ApplicationConfirmationViewModel.ApplicationApprenticeship>(), _fixture.Create<bool>(), true);

            model.ConfirmAction.Should().Be("NewAgreementRequired");
        }

        [Test]
        public void When_new_agreement_not_required_confirm_action_should_be_DisplayDeclaration()
        {
            var model = new ApplicationConfirmationViewModel(_fixture.Create<Guid>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.CreateMany<ApplicationConfirmationViewModel.ApplicationApprenticeship>(), _fixture.Create<bool>(), false);

            model.ConfirmAction.Should().Be("DisplayDeclaration");
        }
    }
}
