using Microsoft.AspNetCore.Mvc.RazorPages;
using Purrnet.Services;

namespace Purrnet.Pages
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
