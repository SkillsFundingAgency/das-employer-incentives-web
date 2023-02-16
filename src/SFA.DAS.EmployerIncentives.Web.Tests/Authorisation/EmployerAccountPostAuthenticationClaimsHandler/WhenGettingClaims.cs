using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Authorisation.GovUserEmployerAccount;
using SFA.DAS.EmployerIncentives.Web.Infrastructure;
using SFA.DAS.EmployerIncentives.Web.Services;
using SFA.DAS.EmployerIncentives.Web.Services.Users.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Authorisation.EmployerAccountPostAuthenticationClaimsHandler
{
    public class WhenGettingClaims
    {
        private string _userId;
        private string _email;
        private string _emailNotMatching;
        private Web.Authorisation.GovUserEmployerAccount.EmployerAccountPostAuthenticationClaimsHandler _handler;
        private GetUserAccountsResponse _response;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _response = fixture.Create<GetUserAccountsResponse>();
            _response.UserAccounts.First().Role = UserRole.Owner.ToString();
            _response.UserAccounts.Last().Role = UserRole.Transactor.ToString();
            _userId = fixture.Create<string>();
            _email = fixture.Create<string>();
            _emailNotMatching = fixture.Create<string>();
            
            var response = new HttpResponseMessage
            {
                Content = new StringContent(JsonSerializer.Serialize(_response)),
                StatusCode = HttpStatusCode.OK
            };
            var notFoundResponse = new HttpResponseMessage
            {
                Content = new StringContent(JsonSerializer.Serialize(new GetUserAccountsResponse())),
                StatusCode = HttpStatusCode.OK
            };
            var employerAccountRequest = OuterApiRoutes.UserEmployerAccounts.GetEmployerAccountInfo(_userId, _email);
            var employerAccountRequestNonMatching = OuterApiRoutes.UserEmployerAccounts.GetEmployerAccountInfo(_userId, _emailNotMatching);
            var httpMessageHandler = new Mock<HttpMessageHandler>();
            httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(c =>
                        c.Method.Equals(HttpMethod.Get)
                        && c.RequestUri.Equals(new Uri("https://tempuri.org/"+ employerAccountRequest))
                        ),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) => response);
            httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(c =>
                        c.Method.Equals(HttpMethod.Get)
                        && c.RequestUri.Equals(new Uri("https://tempuri.org/"+ employerAccountRequestNonMatching))
                    ),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) => notFoundResponse);
            
            var client = new HttpClient(httpMessageHandler.Object);
            client.BaseAddress = new Uri("https://tempuri.org");

            _handler = new Web.Authorisation.GovUserEmployerAccount.EmployerAccountPostAuthenticationClaimsHandler(client);

        }
        [Test]
        public async Task Then_The_Claims_Are_Passed_To_The_Api_And_Id_FirstName_LastName_Populated()
        {
            var tokenValidatedContext = ArrangeTokenValidatedContext(_userId, _email);
            
            var actual = (await _handler.GetClaims(tokenValidatedContext)).ToList();

            actual.First(c=>c.Type.Equals(EmployerClaimTypes.UserId)).Value.Should().Be(_response.EmployerUserId);
            actual.First(c=>c.Type.Equals(EmployerClaimTypes.GivenName)).Value.Should().Be(_response.FirstName);
            actual.First(c=>c.Type.Equals(EmployerClaimTypes.FamilyName)).Value.Should().Be(_response.LastName);
        }

        [Test]
        public async Task Then_The_Accounts_Are_Populated_For_Owner_And_Transactor_Accounts()
        {
            var tokenValidatedContext = ArrangeTokenValidatedContext(_userId, _email);
            
            var actual = (await _handler.GetClaims(tokenValidatedContext)).ToList();

            var actualAccountClaims = actual.Where(c => c.Type.Equals(EmployerClaimTypes.Account)).Select(c => c.Value)
                .ToList();
            actualAccountClaims.Count.Should().Be(2);
            actualAccountClaims.Should().BeEquivalentTo(
                _response.UserAccounts
                    .Where(c => c.Role.Equals(UserRole.Owner.ToString()) ||
                                c.Role.Equals(UserRole.Transactor.ToString())).Select(c => c.AccountId).ToList());
        }

        [Test]
        public async Task Then_If_No_Response_From_Api_Null_Returned()
        {
            var tokenValidatedContext = ArrangeTokenValidatedContext(_userId, _emailNotMatching);
            
            var actual = (await _handler.GetClaims(tokenValidatedContext)).ToList();

            actual.Should().BeEmpty();
        }
        
        private TokenValidatedContext ArrangeTokenValidatedContext(string nameIdentifier, string emailAddress)
        {
            var identity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, nameIdentifier),
                new Claim(ClaimTypes.Email, emailAddress)
            });
        
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(identity));
            return new TokenValidatedContext(new DefaultHttpContext(), new AuthenticationScheme(",","", typeof(TestAuthHandler)),
                new OpenIdConnectOptions(), Mock.Of<ClaimsPrincipal>(), new AuthenticationProperties())
            {
                Principal = claimsPrincipal
            };
        }
        private class TestAuthHandler : IAuthenticationHandler
        {
            public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
            {
                throw new NotImplementedException();
            }

            public Task<AuthenticateResult> AuthenticateAsync()
            {
                throw new NotImplementedException();
            }

            public Task ChallengeAsync(AuthenticationProperties properties)
            {
                throw new NotImplementedException();
            }

            public Task ForbidAsync(AuthenticationProperties properties)
            {
                throw new NotImplementedException();
            }
        }
    }
}