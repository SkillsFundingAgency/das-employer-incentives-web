using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    public class ApplicationCompleteController : Controller
    {
        [HttpGet]
        [Route("application-complete")]
        public ViewResult Index()
        {
            return View();
        }
    }
}
