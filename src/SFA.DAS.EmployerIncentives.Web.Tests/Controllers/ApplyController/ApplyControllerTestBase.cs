using AutoFixture;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using SFA.DAS.HashingService;
using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController
{
    public abstract class ApplyControllerTestBase
    {
        protected Mock<IOptions<WebConfigurationOptions>> ConfigurationMock;
        protected Mock<ILegalEntitiesService> LegalEntitiesService;
        protected Mock<IApprenticesService> ApprenticesServiceMock;
        protected Mock<IHashingService> HashingService;

        protected Web.Controllers.ApplyController Sut;

        protected Fixture Fixture;

        [SetUp]
        public void SetUp()
        {
            Fixture = new Fixture();

            ConfigurationMock = new Mock<IOptions<WebConfigurationOptions>>();
            LegalEntitiesService = new Mock<ILegalEntitiesService>();
            LegalEntitiesService
                .Setup(m => m.Get(It.IsAny<long>()))
                .ReturnsAsync(new List<LegalEntityDto>() { Fixture.Create<LegalEntityDto>() });

            ApprenticesServiceMock = new Mock<IApprenticesService>();

            HashingService = new Mock<IHashingService>();
            ConfigurationMock.Setup(x => x.Value.CommitmentsBaseUrl).Returns("");

            Sut = new Web.Controllers.ApplyController(
                ConfigurationMock.Object,
                LegalEntitiesService.Object,
                ApprenticesServiceMock.Object,
                HashingService.Object);
        }
    }
}
