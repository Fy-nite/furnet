using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using furnet.Models;
using furnet.Services;

namespace furnet.Pages.Packages
{
    public class BrowseModel : PageModel
    {
        private readonly IFurApiService _furApiService;
        private readonly ILogger<BrowseModel> _logger;

        public List<Package> Packages { get; set; } = new();
        public PackageListResponse? ApiResponse { get; set; }
        public bool IsApiAvailable => _furApiService.IsApiAvailable;
        
        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; } = "name";
        
        [BindProperty(SupportsGet = true)]
        public string SearchQuery { get; set; } = "";

        public BrowseModel(IFurApiService furApiService, ILogger<BrowseModel> logger)
        {
            _furApiService = furApiService;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            try
            {
                // Clear cache if explicitly requested
                if (Request.Query.ContainsKey("refresh"))
                {
                    _furApiService.ClearCache();
                    _logger.LogInformation("Cache cleared due to refresh request");
                }

                ApiResponse = await _furApiService.GetPackagesAsync(SortBy, SearchQuery);
                Packages = await _furApiService.GetPackageDetailsAsync(SortBy, SearchQuery);
                
                _logger.LogInformation("Loaded {PackageCount} packages for browse page", Packages.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading packages");
                Packages = new List<Package>();
            }
        }
    }
}
