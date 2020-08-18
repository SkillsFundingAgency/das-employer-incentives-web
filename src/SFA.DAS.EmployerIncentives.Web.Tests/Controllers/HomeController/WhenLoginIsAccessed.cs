using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.HomeController
{
    [TestFixture]
    public class WhenLoginIsAccessed
    {
        private Web.Controllers.HomeController _sut;
        private ClaimsIdentity _claimsIdentity;

        [SetUp]
        public void SetUp()
        {            
            _claimsIdentity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(_claimsIdentity);

            _sut = new Web.Controllers.HomeController
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = user
                    }
                }
            };
        }

        [Test]
        public async Task Then_the_caller_is_redirected_to_home_if_the_user_has_an_account_claim()
        {
            // arrange
            var claimValue = Guid.NewGuid().ToString();
            _claimsIdentity.AddClaim(new Claim(EmployerClaimTypes.Account, claimValue));

            // act
            var result = await _sut.Login() as RedirectToActionResult;

            // assert
            result.ActionName.Should().Be("Home");
            result.RouteValues.ContainsKey("accountId");
            result.RouteValues["accountId"].Should().Be(claimValue);
        }

        [Test]
        public async Task Then_the_caller_is_redirected_to_forbid_if_the_user_has_no_claim()
        {
            // act
            var result = await _sut.Login() as ForbidResult;

            // assert
            result.Should().NotBeNull();
        }
    }
}
