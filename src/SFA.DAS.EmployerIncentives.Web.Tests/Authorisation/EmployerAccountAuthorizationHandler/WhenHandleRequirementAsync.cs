using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Authorisation;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.RouteValues;
using SFA.DAS.EmployerIncentives.Web.Services.Users;
using SFA.DAS.HashingService;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Authorisation.EmployerAccountAuthorizationHandlerTests
{
    [TestFixture]
    public class WhenHandleRequirementAsync
    {
        private EmployerAccountAuthorizationHandler _sut;
        private List<IAuthorizationRequirement> _requirements;
        private ClaimsPrincipal _user;
        private ClaimsIdentity _identity;
        private object _resource;
        private ActionContext _actionContext;
        private Guid _userId;
        private List<Claim> _claims;
        private string _accountClaimValue;

        [SetUp]
        public void SetUp()
        {
            _userId = Guid.NewGuid();
            _accountClaimValue = Guid.NewGuid().ToString();

            _requirements = new List<IAuthorizationRequirement>
            {
                new EmployerAccountRequirement()
            };
            _actionContext = new ActionContext
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            };

            _actionContext.RouteData.Values.Add("controller", "TestController");
            _actionContext.RouteData.Values.Add("action", "TestAction");
            _actionContext.RouteData.Values.Add(RouteValueKeys.AccountHashedId, _accountClaimValue);

            _resource = new AuthorizationFilterContext(_actionContext, new List<IFilterMetadata>());
            var mvcContext = _resource as AuthorizationFilterContext;
            mvcContext.HttpContext.Request.Host = new HostString("https://employer-incentives.gov.uk");

            _claims = new List<Claim>
            {
                new Claim(EmployerClaimTypes.UserId, _userId.ToString()),
                new Claim(EmployerClaimTypes.Account, _accountClaimValue)
            };
            _identity = new ClaimsIdentity(_claims);
            _user = new ClaimsPrincipal(_identity);

            _sut = new EmployerAccountAuthorizationHandler();
        }

        [Test]
        public async Task Then_the_requirement_does_not_succeed_if_the_resource_is_not_a_AuthorizationFilterContext()
        {
            // arrange            
            var context = new AuthorizationHandlerContext(_requirements, _user, new object());

            // act
            await _sut.HandleAsync(context);

            // assert
            context.HasSucceeded.Should().BeFalse();
        }

        [Test]
        public async Task Then_the_requirement_succeeds_if_the_route_is_the_home_controller_and_is_not_the_login_action()
        {
            // arrange            
            _actionContext.RouteData.Values["controller"] = "Home";
            var context = new AuthorizationHandlerContext(_requirements, _user, _resource);

            // act
            await _sut.HandleAsync(context);

            // assert
            context.HasSucceeded.Should().BeTrue();
        }

        [Test]
        public async Task Then_the_requirement_succeeds_is_the_home_controller_and_is_the_login_action()
        {
            // arrange            
            _actionContext.RouteData.Values["controller"] = "Home";
            _actionContext.RouteData.Values["action"] = "Login";
            var context = new AuthorizationHandlerContext(_requirements, _user, _resource);

            // act
            await _sut.HandleAsync(context);

            // assert
            context.HasSucceeded.Should().BeTrue();
        }

        [Test]
        public async Task Then_the_requirement_does_not_succeed_if_the_route_does_not_contain_the_accountId()
        {
            // arrange            
            _actionContext.RouteData.Values.Remove(RouteValueKeys.AccountHashedId);
            var context = new AuthorizationHandlerContext(_requirements, _user, _resource);

            // act
            await _sut.HandleAsync(context);

            // assert
            context.HasSucceeded.Should().BeFalse();
        }

        [Test]
        public async Task Then_the_requirement_does_not_succeed_if_the_user_does_not_have_a_userId_claim()
        {
            // arrange            
            _identity = new ClaimsIdentity(new List<Claim>());
            _user = new ClaimsPrincipal(_identity);
            var context = new AuthorizationHandlerContext(_requirements, _user, _resource);

            // act
            await _sut.HandleAsync(context);

            // assert
            context.HasSucceeded.Should().BeFalse();
        }

        [Test]
        public async Task Then_the_requirement_does_not_succeed_if_the_userid_claim_is_not_a_guid()
        {
            // arrange            
            _claims = new List<Claim>
            {
                new Claim(EmployerClaimTypes.UserId, "TEST")
            };
            _identity = new ClaimsIdentity(_claims);
            _user = new ClaimsPrincipal(_identity);
            var context = new AuthorizationHandlerContext(_requirements, _user, _resource);

            // act
            await _sut.HandleAsync(context);

            // assert
            context.HasSucceeded.Should().BeFalse();
        }

        [Test]
        public async Task Then_the_requirement_does_not_succeed_if_the_accountId_claim_does_not_exist()
        {
            // arrange            
            _claims.Remove(_claims.Find(c => c.Type == EmployerClaimTypes.Account));            
            _identity = new ClaimsIdentity(_claims);
            _user = new ClaimsPrincipal(_identity);
            var context = new AuthorizationHandlerContext(_requirements, _user, _resource);

            // act
            await _sut.HandleAsync(context);

            // assert
            context.HasSucceeded.Should().BeFalse();
        }

        [Test]
        public async Task Then_the_requirement_does_not_succeed_if_the_accountId_claim_does_not_match_the_route_data()
        {
            // arrange            
            _claims.Remove(_claims.Find(c => c.Type == EmployerClaimTypes.Account));
            _claims.Add(new Claim(EmployerClaimTypes.Account, Guid.NewGuid().ToString()));
            _identity = new ClaimsIdentity(_claims);
            _user = new ClaimsPrincipal(_identity);
            var context = new AuthorizationHandlerContext(_requirements, _user, _resource);

            // act
            await _sut.HandleAsync(context);

            // assert
            context.HasSucceeded.Should().BeFalse();
        }

        [Test]
        public async Task Then_the_requirement_succeeds_if_the_route_accountid_matches_the_account_claim()
        {
            // arrange            
            var context = new AuthorizationHandlerContext(_requirements, _user, _resource);

            // act
            await _sut.HandleAsync(context);

            // assert
            context.HasSucceeded.Should().BeTrue();
        }
    }
}
