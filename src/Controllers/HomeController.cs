using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace SACA.Controllers
{
    public class HomeController
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public HomeController(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        
        [AllowAnonymous]
        [HttpGet("/")]
        public ActionResult<string> Index()
        {
            return "SACA v2.0.3";
        }

        [AllowAnonymous]
        [HttpGet("/env")]
        public ActionResult<dynamic> Env()
        {
            var hostEnvironment = _contextAccessor.HttpContext?.RequestServices.GetRequiredService<IWebHostEnvironment>();
            var env = new
            {
                ApplicationName = hostEnvironment?.ApplicationName,
                Environment = hostEnvironment?.EnvironmentName,
            };

            return env;
        }
    }
}
