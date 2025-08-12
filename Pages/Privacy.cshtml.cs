using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Purrnet.Services;

namespace Purrnet.Pages
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
