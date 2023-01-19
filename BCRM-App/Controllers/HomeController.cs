using BCRM.Common.Configs;
using BCRM.Logging.Extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace BCRM_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;                

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.Log(LogLevel.Information, tag: "bcrm-app", message: "bcrm thanachart", args: null);

            string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            string Brand_Ref = BCRM_Config.Platform.Brand_Ref;
            string App_Id = BCRM_Config.Platform.App.App_Id;
            string content = $"BCRM-App | Brand {Brand_Ref} | App {App_Id} | version {assemblyVersion})";

            return Content(content);
        }
    }
}
