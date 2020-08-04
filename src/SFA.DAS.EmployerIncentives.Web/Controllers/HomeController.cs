using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerIncentives.Web.ViewModels.Home;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{    
    public class HomeController : Controller
    {
        [Route("{accountId}")]
        public IActionResult Home(string accountId)
        {            
            return View(new HomeViewModel(accountId));
        }
    }
}