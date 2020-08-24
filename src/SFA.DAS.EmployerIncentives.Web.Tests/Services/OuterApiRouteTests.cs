using FluentAssertions;
using NUnit.Framework;
using System;
using System.IO;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Services
{
    public class OuterApiRouteTests
    {
        [Test]
        public void NoOuterApiRoutesArePrefixedWithSlash()
        {
            var pathToOuterApiRoutesFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\SFA.DAS.EmployerIncentives.Web\Services\OuterApiRoutes.cs");

            var text = File.ReadAllText(pathToOuterApiRoutesFile);

            text.Should().NotContainAny("\"/", "$\"/");
        }
    }
}
