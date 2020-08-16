using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Models;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Models
{
    class ApplicationInformationForExternalVerificationModelTests
    {
        [Test]
        public void Given_New_Application_ToPsvString_concatenates_data_into_pipe_separated_string()
        {
            const string expected = "DW5T8V|00000000|Bob Martin|bob@gmail.com|3000";

            var sut = new ApplicationInformationForExternalVerificationModel()
            {
                HashedLegalEntityId = "DW5T8V",
                IncentiveAmount = 3000,
                SubmittedByFullName = "Bob Martin",
                SubmittedByEmailAddress = "bob@gmail.com",
            };

            var actual = sut.ToPsvString();

            actual.Should().Be(expected);
        }

        [Test]
        public void Given_New_Application_GetReturnUrl_returns_link_to_confirmation_page()
        {

        }
    }
}
