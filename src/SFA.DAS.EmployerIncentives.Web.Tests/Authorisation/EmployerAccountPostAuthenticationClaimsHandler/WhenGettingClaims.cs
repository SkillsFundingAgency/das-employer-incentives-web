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
using SFA.DAS.GovUK.Auth.Employer;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Authorisation.EmployerAccountPostAuthenticationClaimsHandler
{
    public class WhenGettingClaims
    {
        private string _userId;
        private string _email;
        private string _emailNotMatching;
        private string _emailSuspended;
        private Web.Authorisation.GovUserEmployerAccount.GovAuthEmployerAccountService _handler;
        private GetUserAccountsResponse _response;
        private GetUserAccountsResponse _responseSuspended;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            _response = fixture.Create<GetUserAccountsResponse>();
            _response.UserAccounts.First().Role = UserRole.Owner.ToString();
            _response.UserAccounts.Last().Role = UserRole.Transactor.ToString();
            _response.IsSuspended = false;
            _responseSuspended = fixture.Create<GetUserAccountsResponse>();
            _responseSuspended.IsSuspended = true;
            _userId = fixture.Create<string>();
            _email = fixture.Create<string>();
            _emailNotMatching = fixture.Create<string>();
            _emailSuspended = fixture.Create<string>();
            
            var response = new HttpResponseMessage
            {
                Content = new StringContent(JsonSerializer.Serialize(_response)),
                StatusCode = HttpStatusCode.OK
            };
            var employerAccountRequest = OuterApiRoutes.UserEmployerAccounts.GetEmployerAccountInfo(_userId, _email);
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
            
            var client = new HttpClient(httpMessageHandler.Object);
            client.BaseAddress = new Uri("https://tempuri.org");

            _handler = new Web.Authorisation.GovUserEmployerAccount.GovAuthEmployerAccountService(client);

        }

        [Test]
        public async Task Then_The_Accounts_Are_Populated_For_Owner_And_Transactor_Accounts()
        {
            
            var actual = (await _handler.GetUserAccounts(_userId, _email));

            actual.Should().BeEquivalentTo(new
            {
                EmployerAccounts = _response.UserAccounts != null? _response.UserAccounts.Select(c => new EmployerUserAccountItem
                {
                    Role = c.Role,
                    AccountId = c.AccountId,
                    ApprenticeshipEmployerType = Enum.Parse<ApprenticeshipEmployerType>(c.ApprenticeshipEmployerType.ToString()),
                    EmployerName = c.EmployerName,
                }).ToList() : [],
                FirstName = _response.FirstName,
                IsSuspended = _response.IsSuspended,
                LastName = _response.LastName,
                EmployerUserId = _response.EmployerUserId,
            });
        }

    }
}