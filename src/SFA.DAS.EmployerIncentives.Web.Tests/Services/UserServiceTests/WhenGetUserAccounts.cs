using AutoFixture;
using FluentAssertions;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.ReadStore;
using SFA.DAS.EmployerIncentives.Web.Services.Users;
using SFA.DAS.EmployerIncentives.Web.Services.Users.Types;
using SFA.DAS.HashingService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Services.ApplicationTests
{
    [TestFixture]
    public class WhenGetUserAccounts
    {
        private UserService _sut;
        private Fixture _fixture;
        private Mock<IAccountUsersReadOnlyRepository> _mockAccountUsersReadOnlyRepository;
        private Mock<IHashingService> _mockHashingService;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();

            _mockAccountUsersReadOnlyRepository = new Mock<IAccountUsersReadOnlyRepository>();
            _mockHashingService = new Mock<IHashingService>();

            _sut = new UserService(_mockAccountUsersReadOnlyRepository.Object, _mockHashingService.Object);
        }

        [Test]
        public async Task Then_The_AccountModels_Are_Returned()
        {
            // arrange
            var userRef = _fixture.Create<Guid>();
            var accountUsers = _fixture.Create<List<AccountUsers>>();

            int count = 1;
            var expected = new List<UserModel>();
            accountUsers.ForEach(a =>
            {
                a.userRef = userRef;
                a.removed = null;
                a.role = (UserRole)count;
                var hashedValue = $"HASH{a.accountId}";

                _mockHashingService.Setup(m => m.HashValue(a.accountId)).Returns(hashedValue);
                if (a.role == UserRole.Owner || a.role == UserRole.Transactor)
                {
                    expected.Add(new UserModel { UserRef = userRef, AccountId = hashedValue });
                }
                count++;
            });

            var documentQuery = new FakeDocumentQuery<AccountUsers>(accountUsers);

            GetUserRequest request = new GetUserRequest
            {
                UserRef = userRef,
                Roles = new List<UserRole>() { UserRole.Owner, UserRole.Transactor }
            };

            _mockAccountUsersReadOnlyRepository.Setup(m =>
            m.CreateQuery(It.IsAny<FeedOptions>()))
            .Returns(documentQuery);
            
            // act
            var result = await _sut.Get(request);

            // assert
            result.Count().Should().Be(2);
            result.Should().BeEquivalentTo(expected);
        }

#pragma warning disable S3881 // "IDisposable" should be implemented correctly
        private class FakeDocumentQuery<T> : IDocumentQuery<T>, IOrderedQueryable<T>
#pragma warning restore S3881 // "IDisposable" should be implemented correctly
        {
            public Expression Expression => _query.Expression;
            public Type ElementType => _query.ElementType;
            public bool HasMoreResults => ++_page <= _pages;
            public IQueryProvider Provider => new FakeDocumentQueryProvider<T>(_query.Provider);

            private readonly IQueryable<T> _query;
            private readonly int _pages = 1;
            private int _page = 0;

            public FakeDocumentQuery(IEnumerable<T> data)
            {
                _query = data.AsQueryable();
            }

            public Task<FeedResponse<TResult>> ExecuteNextAsync<TResult>(CancellationToken token = default)
            {
                return Task.FromResult(new FeedResponse<TResult>(this.Cast<TResult>()));
            }

            public Task<FeedResponse<dynamic>> ExecuteNextAsync(CancellationToken token = default)
            {
                return Task.FromResult(new FeedResponse<dynamic>(this.Cast<dynamic>()));
            }

            public IEnumerator<T> GetEnumerator()
            {
                return _query.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

#pragma warning disable S1186 // Methods should not be empty
            public void Dispose()
#pragma warning restore S1186 // Methods should not be empty
            {
            }
        }

        private class FakeDocumentQueryProvider<T> : IQueryProvider
        {
            private readonly IQueryProvider _queryProvider;

            public FakeDocumentQueryProvider(IQueryProvider queryProvider)
            {
                _queryProvider = queryProvider;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new FakeDocumentQuery<T>(_queryProvider.CreateQuery<T>(expression));
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new FakeDocumentQuery<TElement>(_queryProvider.CreateQuery<TElement>(expression));
            }

            public object Execute(Expression expression)
            {
                return _queryProvider.Execute(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return _queryProvider.Execute<TResult>(expression);
            }
        }
    }
}
