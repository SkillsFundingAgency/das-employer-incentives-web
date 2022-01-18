using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Models;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Models
{
    [TestFixture]
    public class PaymentStatusModelTests
    {
        private PaymentStatusModel _sut;

        [SetUp]
        public void Arrange()
        {
            _sut = new PaymentStatusModel()
            {
                PaymentIsStopped = false,
                LearnerMatchFound = true,
                HasDataLock = false,
                InLearning = true,
                PausePayments = false,
                RequiresNewEmployerAgreement = false,
                WithdrawnByCompliance = false,
                WithdrawnByEmployer = false,
                DisplayEmploymentCheckResult = false,
                EmploymentCheckPassed = true
            };
        }

        [Test]
        public void Then_show_payment_status_is_not_set_when_no_conditions_are_met()
        {
            // Act / Assert
            _sut.ShowPaymentStatus.Should().BeFalse();
        }

        [Test]
        public void Then_show_payment_status_is_set_when_payment_is_stopped()
        {
            // Arrange
            _sut.PaymentIsStopped = true;

            // Act / Assert
            _sut.ShowPaymentStatus.Should().BeTrue();
        }

        [Test]
        public void Then_show_payment_status_is_set_when_learner_match_not_found()
        {
            // Arrange
            _sut.LearnerMatchFound = false;

            // Act / Assert
            _sut.ShowPaymentStatus.Should().BeTrue();
        }

        [Test]
        public void Then_show_payment_status_is_set_when_has_data_lock()
        {
            // Arrange
            _sut.HasDataLock = true;

            // Act / Assert
            _sut.ShowPaymentStatus.Should().BeTrue();
        }

        [Test]
        public void Then_show_payment_status_is_set_when_not_in_learning()
        {
            // Arrange
            _sut.InLearning = false;

            // Act / Assert
            _sut.ShowPaymentStatus.Should().BeTrue();
        }

        [Test]
        public void Then_show_payment_status_is_set_when_payments_paused()
        {
            // Arrange
            _sut.PausePayments = true;

            // Act / Assert
            _sut.ShowPaymentStatus.Should().BeTrue();
        }

        [Test]
        public void Then_show_payment_status_is_set_when_requires_new_employer_agreement()
        {
            // Arrange
            _sut.RequiresNewEmployerAgreement = true;

            // Act / Assert
            _sut.ShowPaymentStatus.Should().BeTrue();
        }

        [Test]
        public void Then_show_payment_status_is_set_when_withdrawn_by_compliance()
        {
            // Arrange
            _sut.WithdrawnByCompliance = true;

            // Act / Assert
            _sut.ShowPaymentStatus.Should().BeTrue();
        }

        [Test]
        public void Then_show_payment_status_is_set_when_withdrawn_by_employer()
        {
            // Arrange
            _sut.WithdrawnByEmployer = true;

            // Act / Assert
            _sut.ShowPaymentStatus.Should().BeTrue();
        }

        [Test]
        public void Then_show_payment_status_is_set_when_employment_check_failed()
        {
            // Arrange
            _sut.DisplayEmploymentCheckResult = true;
            _sut.EmploymentCheckPassed = false;

            // Act / Assert
            _sut.ShowPaymentStatus.Should().BeTrue();
        }

        [Test]
        public void Then_show_payment_status_is_not_set_when_employment_check_passed()
        {
            // Arrange
            _sut.DisplayEmploymentCheckResult = true;
            _sut.EmploymentCheckPassed = true;

            // Act / Assert
            _sut.ShowPaymentStatus.Should().BeFalse();
        }

        [Test]
        public void Then_show_payment_status_is_not_set_when_employment_check_result_not_found()
        {
            // Arrange
            _sut.DisplayEmploymentCheckResult = true;
            _sut.EmploymentCheckPassed = null;

            // Act / Assert
            _sut.ShowPaymentStatus.Should().BeFalse();
        }

        [TestCase(null)]
        [TestCase(false)]
        [TestCase(true)]
        public void Then_show_payment_status_is_not_set_when_employment_check_feature_toggle_off(bool? employmentCheckStatus)
        {
            // Arrange
            _sut.DisplayEmploymentCheckResult = false;
            _sut.EmploymentCheckPassed = employmentCheckStatus;

            // Act / Assert
            _sut.ShowPaymentStatus.Should().BeFalse();
        }
    }
}
