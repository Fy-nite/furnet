using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using furnet.Services;

namespace furnet.Pages
{
    public class PrivacyModel : BasePageModel
    {
        private readonly ILogger<PrivacyModel> _logger;

        public PrivacyModel(ILogger<PrivacyModel> logger, TestingModeService testingModeService) 
            : base(testingModeService)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
