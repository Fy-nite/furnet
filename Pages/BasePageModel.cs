using Microsoft.AspNetCore.Mvc.RazorPages;
using Purrnet.Services;

namespace Purrnet.Pages
{
    public class BasePageModel : PageModel
    {
        protected readonly TestingModeService _testingModeService;

        public BasePageModel(TestingModeService testingModeService)
        {
            _testingModeService = testingModeService;
        }

        public override void OnPageHandlerExecuting(Microsoft.AspNetCore.Mvc.Filters.PageHandlerExecutingContext context)
        {
            ViewData["IsTestingMode"] = _testingModeService.IsTestingMode;
            base.OnPageHandlerExecuting(context);
        }
    }
}
