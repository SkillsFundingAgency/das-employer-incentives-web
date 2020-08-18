using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.ViewModels;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;
using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Models
{
    [TestFixture]
    public class ChooseOrganisationViewModelTests
    {
        private Fixture _fixture;
        private List<LegalEntityModel> _organisations;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();

            _organisations = new List<LegalEntityModel>
            {
                new LegalEntityModel { Name = "BOB THE BUILDER", AccountLegalEntityId = _fixture.Create<string>() },
                new LegalEntityModel { Name = "AARDVARK ENTERPRISES", AccountLegalEntityId = _fixture.Create<string>() },
                new LegalEntityModel { Name = "ZOOM TRAINING CO", AccountLegalEntityId = _fixture.Create<string>() },
                new LegalEntityModel { Name = "POSTMAN PAT", AccountLegalEntityId = _fixture.Create<string>() }
            };
        }

        [Test]
        public void Organisation_names_should_be_ordered_alphabetically()
        {
            // Arrange
            var model = new ChooseOrganisationViewModel();

            // Act
            model.AddOrganisations(_organisations);

            // Assert
            model.Organisations[0].Name.Should().Be(_organisations[1].Name);
            model.Organisations[1].Name.Should().Be(_organisations[0].Name);
            model.Organisations[2].Name.Should().Be(_organisations[3].Name);
            model.Organisations[3].Name.Should().Be(_organisations[2].Name);
        }
    }
}
