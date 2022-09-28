using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SACA.Controllers
{
    public class HomeController
    {
        [AllowAnonymous]
        [HttpGet("/")]
        public ActionResult<string> Index()
        {
            return "SACA v2.0.3";
        }
    }
}
