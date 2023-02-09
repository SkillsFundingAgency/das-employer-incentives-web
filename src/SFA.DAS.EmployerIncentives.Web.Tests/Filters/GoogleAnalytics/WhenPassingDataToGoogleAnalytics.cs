using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using NUnit.Framework;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;
using SFA.DAS.EmployerIncentives.Web.Filters;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using AutoFixture;
using System;
using Moq;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerIncentives.Web.Services.Security;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Filters
{
    [TestFixture]
    public class WhenPassingDataToGoogleAnalytics
    {
        private RouteData _routeData;
        private ActionExecutingContext _context;
        private DefaultHttpContext _httpContext;
        private GoogleAnalyticsFilterAttribute _sut;
        private TestController _controller;
        private Fixture _fixture;
        private string _userId;
        private string _accountId;
        private long _accountLegalEntityId;
        private Guid _applicationId;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();
            _userId = _fixture.Create<string>();
            _accountId = _fixture.Create<string>();
            _accountLegalEntityId = _fixture.Create<long>();
            _applicationId = Guid.NewGuid();

            _controller = new TestController();
            _routeData = new RouteData();
            _httpContext = new DefaultHttpContext();
            _context = new ActionExecutingContext(
                new ActionContext(_httpContext,
                    _routeData,
                    new ActionDescriptor(),
                     new ModelStateDictionary()
                ),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                _controller);

            _sut = new GoogleAnalyticsFilterAttribute();
        }

        [Test]
        public void Then_the_user_id_is_populated_if_present()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(EmployerClaimTypes.UserId, _userId)
            }, "mockUser"));

            _httpContext.User = user;

            // Act
            _sut.OnActionExecuting(_context);

            // Assert
            Assert.That(_controller.ViewBag.GaData.UserId, Is.EqualTo(_userId));
        }

        [Test]
        public void Then_the_user_id_is_null_if_not_present()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {                
            }, "mockUser"));

            _httpContext.User = user;

            // Act
            _sut.OnActionExecuting(_context);

            // Assert
            Assert.That(_controller.ViewBag.GaData.UserId, Is.Null);
        }

        [Test]
        public void Then_the_account_id_is_populated_if_present()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(EmployerClaimTypes.Account, _accountId)
            }, "mockUser"));

            _httpContext.User = user;

            // Act
            _sut.OnActionExecuting(_context);

            // Assert
            Assert.That(_controller.ViewBag.GaData.Acc, Is.EqualTo(_accountId));
        }

        [Test]
        public void Then_the_account_legal_entity_id_is_populated_if_passed_in_url()
        {
            // Arrange
            _routeData.Values.Add("accountLegalEntityId", _accountLegalEntityId);

            // Act
            _sut.OnActionExecuting(_context);

            // Assert
            Assert.That(_controller.ViewBag.GaData.Ale, Is.EqualTo(_accountLegalEntityId.ToString()));
        }

        [Test]
        public void Then_the_account_legal_entity_id_retrieved_from_the_application_if_application_id_in_the_url()
        {
            // Arrange
            _routeData.Values.Add("accountId", _accountId);
            _routeData.Values.Add("applicationId", _applicationId);

            var unhashedAccountId = _fixture.Create<long>();
            var encodingService = new Mock<IAccountEncodingService>();
            var hashedAccountLegalEntityId = _fixture.Create<string>();
            encodingService.Setup(x => x.Encode(It.IsAny<long>())).Returns(hashedAccountLegalEntityId);

            var applicationService = new Mock<IApplicationService>();
            applicationService.Setup(x => x.GetApplicationLegalEntity(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(_accountLegalEntityId);

            var services = new ServiceCollection();
            services.AddScoped<IAccountEncodingService>(x => encodingService.Object);
            services.AddScoped<IApplicationService>(x => applicationService.Object);

            _httpContext.RequestServices = services.BuildServiceProvider();

            // Act
            _sut.OnActionExecuting(_context);

            // Assert
            Assert.That(_controller.ViewBag.GaData.Ale, Is.EqualTo(hashedAccountLegalEntityId));
        }

    }

    public class TestController : Controller
    {

    }
}