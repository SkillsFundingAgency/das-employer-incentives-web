using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Controllers
{
    public class HubController : Controller
    {
        [Route("{accountId}/{accountLegalEntityId}/hire-new-apprentice-payment")]
        [HttpGet]
        public async Task<IActionResult> Index(string accountId, string accountLegalEntityId)
        {
            return View();
        }
    }
}
