using Microsoft.AspNetCore.Mvc;
using furnet.Services;

namespace furnet.Controllers
{
    public class BaseController : Controller
    {
        protected readonly TestingModeService _testingModeService;

        public BaseController(TestingModeService testingModeService)
        {
            _testingModeService = testingModeService;
            ViewBag.IsTestingMode = testingModeService.IsTestingMode;
        }
    }
}
