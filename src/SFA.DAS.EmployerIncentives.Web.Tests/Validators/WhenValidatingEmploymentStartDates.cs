using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Validators;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Apply;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Validators
{
    [TestFixture]
    public class WhenValidatingEmploymentStartDates
    {
        private Fixture _fixture;
        private string _accountId;
        private Guid _applicationId;
        private EmploymentStartDatesRequest _request;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();
            _accountId = _fixture.Create<string>();
            _applicationId = Guid.NewGuid();
            _request = new EmploymentStartDatesRequest
            {
                AccountId = _accountId,
                ApplicationId = _applicationId,
                EmploymentStartDateDays = new List<int>(),
                EmploymentStartDateMonths = new List<int>(),
                EmploymentStartDateYears = new List<int>()
            };
        }

        [Test]
        public void Then_error_is_returned_if_no_dates_present()
        {
            var validator = new EmploymentStartDateValidator();

            // Act
            var result = validator.Validate(_request).ToList();

            // Assert
            result.Count.Should().Be(1);
            result[0].Index.Should().Be(0);
            result[0].ValidationMessage.Should().Be(EmploymentStartDateValidator.InvalidFieldErrorMessage);
        }

        [TestCase(0, 1, 2021)]
        [TestCase(1, 0, 2021)]
        [TestCase(1, 1, 0)]
        public void Then_errors_are_returned_if_any_of_the_date_fields_are_empty(int day, int month, int year)
        {
            // Arrange
            _request.EmploymentStartDateDays.Add(day);
            _request.EmploymentStartDateMonths.Add(month);
            _request.EmploymentStartDateYears.Add(year);

            var validator = new EmploymentStartDateValidator();

            // Act
            var result = validator.Validate(_request).ToList();

            // Assert
            result.Count.Should().Be(1);
            result[0].Index.Should().Be(0);
            result[0].ValidationMessage.Should().Be(EmploymentStartDateValidator.InvalidFieldErrorMessage);
        }

        [Test]
        public void Then_the_error_lines_are_reported_for_the_invalid_date_when_multiple_start_dates_are_submitted()
        {
            // Arrange
            _request.EmploymentStartDateDays.AddRange(new List<int> {5, 0, 20, 1});
            _request.EmploymentStartDateMonths.AddRange(new List<int> { 10, 8, 5, 0 });
            _request.EmploymentStartDateYears.AddRange(new List<int> { 2021, 2021, 2021, 2021 });

            var validator = new EmploymentStartDateValidator();

            // Act
            var result = validator.Validate(_request).ToList();

            // Assert
            result.Count.Should().Be(2);
            result[0].Index.Should().Be(1);
            result[0].ValidationMessage.Should().Be(EmploymentStartDateValidator.InvalidFieldErrorMessage);
            result[1].Index.Should().Be(3);
            result[1].ValidationMessage.Should().Be(EmploymentStartDateValidator.InvalidFieldErrorMessage);
            result.FirstOrDefault(x => x.Index == 0).Should().BeNull();
            result.FirstOrDefault(x => x.Index == 2).Should().BeNull();
        }

        [Test]
        public void Then_error_is_returned_for_invalid_month()
        {
            // Arrange
            _request.EmploymentStartDateDays.Add(1);
            _request.EmploymentStartDateMonths.Add(13);
            _request.EmploymentStartDateYears.Add(2021);

            var validator = new EmploymentStartDateValidator();

            // Act
            var result = validator.Validate(_request).ToList();

            // Assert
            result.Count.Should().Be(1);
            result[0].Index.Should().Be(0);
            result[0].ValidationMessage.Should().Be(EmploymentStartDateValidator.InvalidFieldErrorMessage);
        }

        [Test]
        public void Then_error_is_returned_for_invalid_year()
        {
            // Arrange
            _request.EmploymentStartDateDays.Add(1);
            _request.EmploymentStartDateMonths.Add(12);
            _request.EmploymentStartDateYears.Add(2019);

            var validator = new EmploymentStartDateValidator();

            // Act
            var result = validator.Validate(_request).ToList();

            // Assert
            result.Count.Should().Be(1);
            result[0].Index.Should().Be(0);
            result[0].ValidationMessage.Should().Be(EmploymentStartDateValidator.InvalidFieldErrorMessage);
        }

        [TestCase(32, 1)]
        [TestCase(30, 2)]
        [TestCase(32, 3)]
        [TestCase(31, 4)]
        [TestCase(32, 5)]
        [TestCase(31, 6)]
        [TestCase(32, 7)]
        [TestCase(32, 8)]
        [TestCase(31, 9)]
        [TestCase(32, 10)]
        [TestCase(31, 11)]
        [TestCase(32, 12)]
        public void Then_errors_are_returned_for_invalid_day_of_month(int day, int month)
        {
            // Arrange
            _request.EmploymentStartDateDays.Add(day);
            _request.EmploymentStartDateMonths.Add(month);
            _request.EmploymentStartDateYears.Add(2021);

            var validator = new EmploymentStartDateValidator();

            // Act
            var result = validator.Validate(_request).ToList();

            // Assert
            result.Count.Should().Be(1);
            result[0].Index.Should().Be(0);
            result[0].ValidationMessage.Should().Be(EmploymentStartDateValidator.InvalidFieldErrorMessage);
        }
    }
}
