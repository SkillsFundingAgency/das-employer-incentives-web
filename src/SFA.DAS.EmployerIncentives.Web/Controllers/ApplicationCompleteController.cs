using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.ViewModels.ApplicationComplete;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    public class ApplicationCompleteController : Controller
    {
        private readonly WebConfigurationOptions _configuration;

        public ApplicationCompleteController(IOptions<WebConfigurationOptions> configuration)
        {
            _configuration = configuration.Value;
        }

        [HttpGet]
        [Route("application-complete")]
        public ViewResult Confirmation()
        {
            var model = new ConfirmationViewModel(_configuration.AccountsBaseUrl);
            return View(model);
        }
    }
}
