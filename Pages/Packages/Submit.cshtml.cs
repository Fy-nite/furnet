using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using furnet.Models;
using furnet.Services;

namespace furnet.Pages.Packages
{
    [Authorize]
    public class SubmitModel : BasePageModel
    {
        private readonly IPackageService _packageService;
        private readonly ILogger<SubmitModel> _logger;

        [BindProperty]
        [Required(ErrorMessage = "Package name is required")]
        [Display(Name = "Package Name")]
        public string PackageName { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Version is required")]
        public string Version { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "At least one author is required")]
        [Display(Name = "Authors (comma-separated)")]
        public string Authors { get; set; } = string.Empty;

        [BindProperty]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [BindProperty]
        [Display(Name = "README URL")]
        public string? ReadmeUrl { get; set; }

        [BindProperty]
        [Display(Name = "Homepage URL")]
        public string? Homepage { get; set; }

        [BindProperty]
        [Display(Name = "Issue Tracker URL")]
        public string? IssueTracker { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Git repository URL is required")]
        [Display(Name = "Git Repository URL")]
        public string Git { get; set; } = string.Empty;

        [BindProperty]
        [Display(Name = "Installer Script")]
        public string? Installer { get; set; }

        [BindProperty]
        [Display(Name = "Dependencies (comma-separated, format: name@version)")]
        public string? Dependencies { get; set; }

        [BindProperty]
        [Display(Name = "Keywords (comma-separated)")]
        public string? Keywords { get; set; }

        public string? Message { get; set; }
        public bool IsSuccess { get; set; }

        public SubmitModel(IPackageService packageService, ILogger<SubmitModel> logger, TestingModeService testingModeService)
            : base(testingModeService)
        {
            _packageService = packageService;
            _logger = logger;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Parse dependencies safely
                var dependenciesList = new List<string>();
                if (!string.IsNullOrWhiteSpace(Dependencies))
                {
                    dependenciesList = Dependencies.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(d => d.Trim())
                        .Where(d => !string.IsNullOrEmpty(d))
                        .ToList();
                }

                // Parse keywords safely
                var keywordsList = new List<string>();
                if (!string.IsNullOrWhiteSpace(Keywords))
                {
                    keywordsList = Keywords.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(k => k.Trim())
                        .Where(k => !string.IsNullOrEmpty(k))
                        .ToList();
                }

                // Parse authors safely
                var authorsList = Authors.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(a => a.Trim())
                    .Where(a => !string.IsNullOrEmpty(a))
                    .ToList();

                if (authorsList.Count == 0)
                {
                    ModelState.AddModelError(nameof(Authors), "At least one author is required");
                    return Page();
                }

                var furConfig = new FurConfig
                {
                    Name = PackageName.Trim(),
                    Version = Version.Trim(),
                    Authors = authorsList,
                    Description = Description?.Trim() ?? string.Empty,
                    ReadmeUrl = ReadmeUrl?.Trim() ?? string.Empty,
                    Homepage = Homepage?.Trim() ?? string.Empty,
                    IssueTracker = IssueTracker?.Trim() ?? string.Empty,
                    Git = Git.Trim(),
                    Installer = Installer?.Trim() ?? string.Empty,
                    Dependencies = dependenciesList,
                    Keywords = keywordsList
                };

                // Get current user info for package creation tracking
                var userName = User.Identity?.Name ?? "unknown";
                var userIdClaim = User.FindFirst("UserId");
                int? userId = null;
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var parsedUserId))
                {
                    userId = parsedUserId;
                }
                
                var success = await _packageService.SavePackageAsync(furConfig, userName, userId);

                if (success)
                {
                    Message = "Package submitted successfully!";
                    IsSuccess = true;
                    ModelState.Clear();
                    
                    // Clear form fields
                    PackageName = string.Empty;
                    Version = string.Empty;
                    Authors = string.Empty;
                    Description = string.Empty;
                    ReadmeUrl = null;
                    Homepage = null;
                    IssueTracker = null;
                    Git = string.Empty;
                    Installer = null;
                    Dependencies = null;
                    Keywords = null;
                    
                    // Redirect to package details page
                    return RedirectToPage("/Packages/Details", new { packageName = furConfig.Name });
                }
                else
                {
                    Message = "Failed to submit package. Package may already exist.";
                    IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting package");
                Message = "An error occurred while submitting the package.";
                IsSuccess = false;
            }

            return Page();
        }
    }
}
