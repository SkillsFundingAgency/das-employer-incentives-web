using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Authorisation;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Authorisation.IsAuthenticatedAuthorizationHandlerTests
{
    [TestFixture]
    public class WhenHandleRequirementAsync
    {
        private IsAuthenticatedAuthorizationHandler _sut;
        private List<IAuthorizationRequirement> _requirements;
        private ClaimsPrincipal _user;
        private ClaimsIdentity _identity;
        private object _resource;
        private ActionContext _actionContext;
        
        [SetUp]
        public void SetUp()
        {
            _requirements = new List<IAuthorizationRequirement>
            {
                new IsAuthenticatedRequirement()
            };
            _actionContext = new ActionContext
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            };

            _resource = new AuthorizationFilterContext(_actionContext, new List<IFilterMetadata>());
         
            _identity = new ClaimsIdentity("Cookie");
            _user = new ClaimsPrincipal(_identity);

            _sut = new IsAuthenticatedAuthorizationHandler();
        }

        [Test]
        public async Task Then_the_requirement_does_not_succeed_if_the_user_has_no_identity()
        {
            // arrange  
            _user = new ClaimsPrincipal();
            var context = new AuthorizationHandlerContext(_requirements, _user, _resource);

            // act
            await _sut.HandleAsync(context);

            // assert
            context.HasSucceeded.Should().BeFalse();
        }

        [Test]
        public async Task Then_the_requirement_does_not_succeed_if_the_user_has_no_identities_that_are_authenticated()
        {
            // arrange                    
            _identity = new ClaimsIdentity();
            _user = new ClaimsPrincipal(_identity);
            var context = new AuthorizationHandlerContext(_requirements, _user, new object());

            // act
            await _sut.HandleAsync(context);

            // assert
            context.HasSucceeded.Should().BeFalse();
        }


        [Test]
        public async Task Then_the_requirement_succeeds_if_the_user_has_an_identity_that_is_authenticated()
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
