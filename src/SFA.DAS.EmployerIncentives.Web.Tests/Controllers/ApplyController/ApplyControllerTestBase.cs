using AutoFixture;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.EmployerIncentives.Web.Services.LegalEntities;
using System.Collections.Generic;
using SFA.DAS.EmployerIncentives.Web.Services.Applications;
using SFA.DAS.EmployerIncentives.Web.Services.Apprentices;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Controllers.ApplyController
{
    public abstract class ApplyControllerTestBase
    {
        protected Mock<IOptions<WebConfigurationOptions>> ConfigurationMock;
        protected Mock<ILegalEntitiesService> LegalEntitiesService;
        protected Mock<IApprenticesService> ApprenticesServiceMock;        
        protected Mock<IApplicationService> ApplicationServiceMock;        

        protected Web.Controllers.ApplyController Sut;

        protected Fixture Fixture;

        [SetUp]
        public void SetUp()
        {
            Fixture = new Fixture();

            ConfigurationMock = new Mock<IOptions<WebConfigurationOptions>>();
            LegalEntitiesService = new Mock<ILegalEntitiesService>();
            LegalEntitiesService
                .Setup(m => m.Get(It.IsAny<string>()))
                .ReturnsAsync(new List<LegalEntityModel>() { Fixture.Create<LegalEntityModel>() });

            ApprenticesServiceMock = new Mock<IApprenticesService>();
            ApplicationServiceMock = new Mock<IApplicationService>();

            ConfigurationMock.Setup(x => x.Value.CommitmentsBaseUrl).Returns("");

            Sut = new Web.Controllers.ApplyController(
                ConfigurationMock.Object,
                LegalEntitiesService.Object,
                ApprenticesServiceMock.Object,
                ApplicationServiceMock.Object);
        }
    }
}
