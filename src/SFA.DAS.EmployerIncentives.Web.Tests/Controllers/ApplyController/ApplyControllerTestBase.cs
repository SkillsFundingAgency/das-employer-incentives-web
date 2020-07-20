using AutoFixture;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Services;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities.Types;
using SFA.DAS.HashingService;
using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController
{
    public abstract class ApplyControllerTestBase
    {
        protected Mock<IOptions<EmployerIncentivesWebConfiguration>> ConfigurationMock;
        protected Mock<IDataService> ServiceMock;
        protected Web.Controllers.ApplyController Sut;
        protected Mock<ILegalEntitiesService> _legalEntitiesService;
        protected Mock<IApprenticesService> _apprenticesService;
        protected Mock<IHashingService> _hashingService;


        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();

            ConfigurationMock = new Mock<IOptions<EmployerIncentivesWebConfiguration>>();

            _legalEntitiesService = new Mock<ILegalEntitiesService>();
            _legalEntitiesService
                .Setup(m => m.Get(It.IsAny<long>()))
                .ReturnsAsync(new List<LegalEntityDto>() { _fixture.Create<LegalEntityDto>() });

            _apprenticesService = new Mock<IApprenticesService>();
            _apprenticesService
                .Setup(m => m.Get(It.IsAny<ApprenticesQuery>()))
                .ReturnsAsync(_fixture.Create<List<ApprenticeDto>>());

            _hashingService = new Mock<IHashingService>();


            ConfigurationMock.Setup(x => x.Value.CommitmentsBaseUrl).Returns("");
            Sut = new Web.Controllers.ApplyController(
                ConfigurationMock.Object,
                _legalEntitiesService.Object,
                _apprenticesService.Object,
                _hashingService.Object);

            _legalEntitiesService = new Mock<ILegalEntitiesService>();
            _legalEntitiesService
                .Setup(m => m.Get(It.IsAny<long>()))
                .ReturnsAsync(new List<LegalEntityDto>() { _fixture.Create<LegalEntityDto>() });

            _apprenticesService = new Mock<IApprenticesService>();
            _apprenticesService
                .Setup(m => m.Get(It.IsAny<ApprenticesQuery>()))
                .ReturnsAsync(_fixture.Create<List<ApprenticeDto>>());

            _hashingService = new Mock<IHashingService>();
            ConfigurationMock.Setup(x => x.Value.CommitmentsBaseUrl).Returns("");
            Sut = new Web.Controllers.ApplyController(
                ConfigurationMock.Object,
                _legalEntitiesService.Object,
                _apprenticesService.Object,
                _hashingService.Object);
        }
    }
}
