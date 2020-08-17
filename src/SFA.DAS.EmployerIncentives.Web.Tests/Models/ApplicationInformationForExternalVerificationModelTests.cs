using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Models
{
    public class ApplicationInformationForExternalVerificationModelTests
    {
        [Test]
        public void Given_New_Application_ToPsvString_concatenates_data_into_pipe_separated_string()
        {
            const string expected = "DW5T8V|00000000|Bob Martin|bob@gmail.com|3000|Michelle Brown|michelle.brown@education.gov.uk|2020-08-01T12:20:21|Samar Ali|Samar.Ali@education.gov.uk|2020-08-03T15:20:21|Daniel Davies|Daniel.Davies@education.gov.uk|2020-09-30T01:23:45|apps=3";

            var sut = new ApplicationInformationForExternalVerificationModel
            {
                HashedLegalEntityId = "DW5T8V",
                IncentiveAmount = 3000,
                SubmittedByFullName = "Bob Martin",
                SubmittedByEmailAddress = "bob@gmail.com",
                SignedAgreements = new List<SignedAgreementModel>
                {
                    new SignedAgreementModel { SignedByName = "Michelle Brown", SignedByEmail = "michelle.brown@education.gov.uk", SignedDate = DateTime.Parse("01-08-2020 12:20:21", new CultureInfo("en-GB"))},
                    new SignedAgreementModel { SignedByName = "Samar Ali", SignedByEmail = "Samar.Ali@education.gov.uk", SignedDate = DateTime.Parse("03-08-2020 15:20:21", new CultureInfo("en-GB"))},
                    new SignedAgreementModel { SignedByName = "Daniel Davies", SignedByEmail = "Daniel.Davies@education.gov.uk", SignedDate = DateTime.Parse("30-09-2020 01:23:45", new CultureInfo("en-GB"))},
                }
            };

            var actual = sut.ToPsvString();

            actual.Should().Be(expected);
        }
    }
}
