using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Models;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Models
{
    class ApprenticeModelTests
    {
        [Test]
        public void Apprentice_DisplayName_is_space_separated_FirstName_LastName()
        {
            var sut = new Fixture().Create<ApprenticeModel>();

            var expected = $"{sut.FirstName} {sut.LastName}";

            sut.DisplayName.Should().Be(expected);
        }
    }
}
