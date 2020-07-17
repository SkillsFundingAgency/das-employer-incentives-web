using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController
{
    public abstract class ApplyControllerTestBase
    {
        protected Mock<IOptions<EmployerIncentivesWebConfiguration>> ConfigurationMock;
        protected Mock<IDataService> ServiceMock;
        protected Web.Controllers.ApplyController Sut;

        [SetUp]
        public void SetUp()
        {
            ConfigurationMock = new Mock<IOptions<EmployerIncentivesWebConfiguration>>();
            ServiceMock = new Mock<IDataService>();
            ConfigurationMock.Setup(x => x.Value.CommitmentsBaseUrl).Returns("");
            Sut = new Web.Controllers.ApplyController(ConfigurationMock.Object, ServiceMock.Object);
        }
    }
}
