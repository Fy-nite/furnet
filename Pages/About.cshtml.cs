using Microsoft.AspNetCore.Mvc.RazorPages;
using furnet.Services;

namespace furnet.Pages
{
    public class AboutModel : BasePageModel
    {
        private readonly ILogger<AboutModel> _logger;

        public AboutModel(ILogger<AboutModel> logger, TestingModeService testingModeService) 
            : base(testingModeService)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
