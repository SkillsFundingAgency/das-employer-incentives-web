using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController
{
    public abstract class ApplyControllerTestBase
    {
        protected Mock<IOptions<EmployerIncentivesWebConfiguration>> _configuration;
        protected Web.Controllers.ApplyController _sut;

        [SetUp]
        public void SetUp()
        {
            _configuration = new Mock<IOptions<EmployerIncentivesWebConfiguration>>();
            _configuration.Setup(x => x.Value.CommitmentsBaseUrl).Returns("");
            _sut = new Web.Controllers.ApplyController(_configuration.Object);
        }
    }
}
