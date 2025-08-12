using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Purrnet.Services;
using Purrnet.Models;

namespace Purrnet.Pages
{
    public class IndexModel : BasePageModel
    {
        private readonly IPackageService _packageService;
        private readonly ILogger<IndexModel> _logger;

        public List<Package> Packages { get; set; } = new();
        public PackageStatistics? Statistics { get; set; }
        public string? ErrorMessage { get; set; }
        public List<Package> TopDownloadedPackages { get; set; } = new();

        public IndexModel(IPackageService packageService, ILogger<IndexModel> logger, TestingModeService testingModeService)
            : base(testingModeService)
        {
            _packageService = packageService;
            _logger = logger;
        }

        public async Task OnGetAsync(string? sort = null, string? search = null)
        {
            try
            {
                var searchResult = await _packageService.SearchPackagesAsync(search, sort, 1, 50);
                Packages = searchResult.Packages;
                Statistics = await _packageService.GetStatisticsAsync();
                TopDownloadedPackages = Statistics?.MostDownloaded?.Take(5).ToList() ?? new List<Package>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading packages");
                ErrorMessage = "Unable to load packages at this time.";
            }
        }
    }
}
