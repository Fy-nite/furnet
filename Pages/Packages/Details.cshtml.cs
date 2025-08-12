using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Purrnet.Models;
using Purrnet.Services;

namespace Purrnet.Pages.Packages
{
    public class DetailsModel : PageModel
    {
        private readonly IPackageService _packageService;
        private readonly ILogger<DetailsModel> _logger;

        public Package? Package { get; set; }
        public string? ErrorMessage { get; set; }

        public DetailsModel(IPackageService packageService, ILogger<DetailsModel> logger)
        {
            _packageService = packageService;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string packageName, string? version = null)
        {
            if (string.IsNullOrEmpty(packageName))
            {
                ErrorMessage = "Package name is required.";
                return Page();
            }

            try
            {
                Package = await _packageService.GetPackageAsync(packageName, version);
                
                if (Package == null)
                {
                    ErrorMessage = $"Package '{packageName}' not found.";
                    return Page();
                }

                // Increment view count
                await _packageService.IncrementViewCountAsync(Package.Id);
                
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading package details for {PackageName}", packageName);
                ErrorMessage = "An error occurred while loading the package details.";
                return Page();
            }
        }
    }
}
