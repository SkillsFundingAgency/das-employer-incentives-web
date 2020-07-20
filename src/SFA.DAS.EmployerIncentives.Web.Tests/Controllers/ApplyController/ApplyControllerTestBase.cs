using AutoFixture;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using SFA.DAS.HashingService;
using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController
{
    public abstract class ApplyControllerTestBase
    {
        protected Mock<IOptions<WebConfigurationOptions>> ConfigurationMock;
        protected Mock<ILegalEntitiesService> _legalEntitiesService;
        protected Mock<IApprenticesService> ApprenticesServiceMock;
        protected Mock<IHashingService> _hashingService;

        protected Web.Controllers.ApplyController Sut;

        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();

            ConfigurationMock = new Mock<IOptions<WebConfigurationOptions>>();
            _legalEntitiesService = new Mock<ILegalEntitiesService>();
            _legalEntitiesService
                .Setup(m => m.Get(It.IsAny<long>()))
                .ReturnsAsync(new List<LegalEntityDto>() { _fixture.Create<LegalEntityDto>() });

            ApprenticesServiceMock = new Mock<IApprenticesService>();
            ApprenticesServiceMock
                .Setup(m => m.Get(It.IsAny<ApprenticesQuery>()))
                .ReturnsAsync(_fixture.Create<List<ApprenticeDto>>());

            _hashingService = new Mock<IHashingService>();
            ConfigurationMock.Setup(x => x.Value.CommitmentsBaseUrl).Returns("");
            Sut = new Web.Controllers.ApplyController(
                ConfigurationMock.Object,
                _legalEntitiesService.Object,
                ApprenticesServiceMock.Object,
                _hashingService.Object);
        }
    }
}
