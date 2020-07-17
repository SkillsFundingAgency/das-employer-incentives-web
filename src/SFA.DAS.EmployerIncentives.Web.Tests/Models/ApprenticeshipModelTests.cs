using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Models;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Models
{
    class ApprenticeshipModelTests
    {
        [Test]
        public void Apprentice_DisplayName_is_space_separated_FirstName_LastName()
        {
            var sut = new Fixture().Create<ApprenticeshipModel>();

            var expected = $"{sut.FirstName} {sut.LastName}";

            sut.DisplayName.Should().Be(expected);
        }
    }
}
