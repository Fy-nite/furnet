using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using furnet.Models;
using furnet.Services;

namespace furnet.Pages.Packages
{
    public class DetailsModel : PageModel
    {
        private readonly IFurApiService _furApiService;
        private readonly ILogger<DetailsModel> _logger;

        public Package? Package { get; set; }
        public FurConfig? FurConfig { get; set; }
        public bool IsApiAvailable => _furApiService.IsApiAvailable;
        public string? ErrorMessage { get; set; }

        public DetailsModel(IFurApiService furApiService, ILogger<DetailsModel> logger)
        {
            _furApiService = furApiService;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string packageName, string? version = null)
        {
            if (string.IsNullOrEmpty(packageName))
            {
                return NotFound();
            }

            try
            {
                FurConfig = await _furApiService.GetPackageAsync(packageName, version);
                
                if (FurConfig == null)
                {
                    ErrorMessage = $"Package '{packageName}' not found.";
                    return NotFound();
                }

                Package = new Package
                {
                    Name = FurConfig.Name,
                    Version = FurConfig.Version,
                    Authors = FurConfig.Authors,
                    SupportedPlatforms = FurConfig.SupportedPlatforms,
                    Description = FurConfig.Description,
                    LongDescription = FurConfig.LongDescription,
                    License = FurConfig.License,
                    LicenseUrl = FurConfig.LicenseUrl,
                    Keywords = FurConfig.Keywords,
                    Tags = FurConfig.Tags,
                    Homepage = FurConfig.Homepage,
                    IssueTracker = FurConfig.IssueTracker,
                    Git = FurConfig.Git,
                    Installer = FurConfig.Installer,
                    InstallCommand = $"fur install {FurConfig.Name}",
                    Dependencies = FurConfig.Dependencies,
                    Downloads = Random.Shared.Next(100, 50000),
                    LastUpdated = DateTime.Now.AddDays(-Random.Shared.Next(1, 365))
                };

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading package details for {PackageName}", packageName);
                ErrorMessage = "An error occurred while loading the package details.";
                return NotFound();
            }
        }
    }
}
