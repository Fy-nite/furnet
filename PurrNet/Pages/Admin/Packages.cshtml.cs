using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Purrnet.Models;
using Purrnet.Services;

namespace Purrnet.Pages.Admin
{
    [Authorize]
    public class PackagesModel : BasePageModel
    {
        private readonly IPackageService _packageService;

        public List<Package> Packages { get; set; } = new();
        
        [BindProperty(SupportsGet = true)]
        public string Filter { get; set; } = "all";
        
        [BindProperty(SupportsGet = true)]
        public string SearchQuery { get; set; } = "";
        
        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; } = "newest";

        // Count properties for the filter buttons
        public int AllCount { get; set; }
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }

        public PackagesModel(IPackageService packageService, TestingModeService testingModeService) 
            : base(testingModeService)
        {
            _packageService = packageService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.HasClaim("IsAdmin", "True"))
            {
                return Forbid();
            }

            // Get all packages for counting and filtering
            var allPackages = await _packageService.GetAllPackagesAsync();
            
            // Calculate counts (for now, treat all packages as approved since we don't have approval status yet)
            AllCount = allPackages.Count;
            PendingCount = 0; // Will implement when we add approval workflow
            ApprovedCount = allPackages.Count(p => p.IsActive);
            RejectedCount = allPackages.Count(p => !p.IsActive);

            // Filter packages based on the selected filter
            Packages = Filter switch
            {
                "pending" => new List<Package>(), // Empty for now
                "approved" => allPackages.Where(p => p.IsActive).ToList(),
                "rejected" => allPackages.Where(p => !p.IsActive).ToList(),
                _ => allPackages
            };

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(SearchQuery))
            {
                Packages = Packages.Where(p => 
                    p.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    p.Description.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    p.Authors.Any(a => a.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            // Apply sorting
            Packages = SortBy switch
            {
                "oldest" => Packages.OrderBy(p => p.CreatedAt).ToList(),
                "name" => Packages.OrderBy(p => p.Name).ToList(),
                "downloads" => Packages.OrderByDescending(p => p.Downloads).ToList(),
                _ => Packages.OrderByDescending(p => p.CreatedAt).ToList()
            };

            return Page();
        }
    }
}
