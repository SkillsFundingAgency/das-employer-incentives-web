using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Filters;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Filters.ServiceClosed
{
    [TestFixture]
    public class WhenDisplayingServiceClosedShutterPage
    {
        private RouteData _routeData;
        private ActionExecutingContext _context;
        private DefaultHttpContext _httpContext;
        private ServiceClosedFilterAttribute _sut;
        private TestController _controller;

        [SetUp]
        public void Arrange()
        {
            _controller = new TestController();
            _httpContext = new DefaultHttpContext();
        }

        [TestCase("Home", "Start")]
        [TestCase("Home", "Home")]
        [TestCase("Home", "BeforeStart")]
        [TestCase("ApplicationComplete", "Confirmation")]
        [TestCase("ApplyApprenticeships", "SelectApprenticeships")]
        [TestCase("ApplyApprenticeships", "ConfirmApprenticeships")]
        [TestCase("ApplyApprenticeships", "DisplayDeclaration")]
        [TestCase("Apply", "Default")]
        [TestCase("Apply", "Declaration")]
        [TestCase("Apply", "SubmitApplication")]
        [TestCase("Apply", "CannotApply")]
        [TestCase("Apply", "CannotApplyYet")]
        [TestCase("Apply", "UlnAlreadyAppliedFor")]
        [TestCase("ApplyEmploymentDetails", "EmploymentStartDates")]
        [TestCase("ApplyEmploymentDetails", "SubmitEmploymentStartDates")]
        [TestCase("ApplyOrganisation", "GetChooseOrganisation")]
        [TestCase("ApplyOrganisation", "ChooseOrganisation")]
        [TestCase("ApplyOrganisation", "ValidateTermsSigned")]
        [TestCase("ApplyQualification", "GetQualificationQuestion")]
        [TestCase("ApplyQualification", "QualificationQuestion")]
        [TestCase("BankDetails", "BankDetailsConfirmation")]
        [TestCase("BankDetails", "AddBankDetails")]
        [TestCase("BankDetails", "EnterBankDetails")]
        [TestCase("BankDetails", "NeedBankDetails")]
        [TestCase("BankDetails", "AmendBankDetails")]
        [TestCase("Cancel", "CancelApplication")]
        [TestCase("Cancel", "Confirm")]
        [TestCase("Hub", "Index")]
        [TestCase("Payments", "ListPayments")]
        [TestCase("Payments", "ListPaymentsForLegalEntity")]
        [TestCase("Payments", "NoApplications")]
        [TestCase("Payments", "ChooseOrganisation")]
        [TestCase("System", "ApplicationsClosed")]
        public void Then_the_shutter_page_replaces_all_expected_routes_into_the_service(string controllerName, string actionName)
        {
            // Arrange
            _routeData = new RouteData();
            _routeData.Values.Add("controller", controllerName);
            _routeData.Values.Add("action", actionName);
            _controller.ControllerContext = new ControllerContext
            {
                RouteData = _routeData
            };

            _context = new ActionExecutingContext(
                new ActionContext(_httpContext,
                    _routeData,
                    new ActionDescriptor(),
                    new ModelStateDictionary()
                ),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                _controller);
            
            _sut = new ServiceClosedFilterAttribute();

            // Act
            _sut.OnActionExecuting(_context);

            // Assert
            Assert.That(_context.Result, Is.InstanceOf<RedirectResult>());
            var redirectResult = (RedirectResult)_context.Result;
            Assert.That(redirectResult.Url.Contains("service-closed"));
        }

        [Test]
        public void Then_the_shutter_page_replaces_any_invalid_URLs()
        {
            // Arrange
            _routeData = new RouteData();
            _routeData.Values.Add("controller", "Error");
            _routeData.Values.Add("action", "PageNotFound");
            _controller.ControllerContext = new ControllerContext
            {
                RouteData = _routeData
            };

            _context = new ActionExecutingContext(
                new ActionContext(_httpContext,
                    _routeData,
                    new ActionDescriptor(),
                    new ModelStateDictionary()
                ),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                _controller);

            _sut = new ServiceClosedFilterAttribute();

            // Act
            _sut.OnActionExecuting(_context);

            // Assert
            Assert.That(_context.Result, Is.InstanceOf<RedirectResult>());
            var redirectResult = (RedirectResult)_context.Result;
            Assert.That(redirectResult.Url.Contains("service-closed"));
        }

        [Test]
        public void Then_the_shutter_page_does_not_replace_the_forbidden_error_page()
        {
            // Arrange
            _routeData = new RouteData();
            _routeData.Values.Add("controller", "Apply");
            _routeData.Values.Add("action", "Forbidden");
            _controller.ControllerContext = new ControllerContext
            {
                RouteData = _routeData
            };

            _context = new ActionExecutingContext(
                new ActionContext(_httpContext,
                    _routeData,
                    new ActionDescriptor(),
                    new ModelStateDictionary()
                ),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                _controller);

            _sut = new ServiceClosedFilterAttribute();

            // Act
            _sut.OnActionExecuting(_context);

            // Assert
            Assert.That(_context.Result, Is.Null);
            Assert.That(_context.HttpContext.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }
    }
}
