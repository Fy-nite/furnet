using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using furnet.Models;
using furnet.Services;

namespace furnet.Pages.Packages
{
    public class SubmitModel : PageModel
    {
        private readonly IFurApiService _furApiService;
        private readonly ILogger<SubmitModel> _logger;

        [BindProperty]
        [Required]
        [Display(Name = "Package Name")]
        public string PackageName { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        public string Version { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [Display(Name = "Authors (comma-separated)")]
        public string Authors { get; set; } = string.Empty;

        [BindProperty]
        [Display(Name = "Homepage URL")]
        public string? Homepage { get; set; }

        [BindProperty]
        [Display(Name = "Issue Tracker URL")]
        public string? IssueTracker { get; set; }

        [BindProperty]
        [Required]
        [Display(Name = "Git Repository URL")]
        public string Git { get; set; } = string.Empty;

        [BindProperty]
        [Display(Name = "Installer Script")]
        public string? Installer { get; set; }

        [BindProperty]
        [Display(Name = "Dependencies (comma-separated, format: name@version)")]
        public string Dependencies { get; set; } = string.Empty;

        public string? Message { get; set; }
        public bool IsSuccess { get; set; }

        public SubmitModel(IFurApiService furApiService, ILogger<SubmitModel> logger)
        {
            _furApiService = furApiService;
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
                var furConfig = new FurConfig
                {
                    Name = PackageName,
                    Version = Version,
                    Authors = Authors.Split(',').Select(a => a.Trim()).ToList(),
                    Homepage = Homepage ?? string.Empty,
                    IssueTracker = IssueTracker ?? string.Empty,
                    Git = Git,
                    Installer = Installer ?? string.Empty,
                    Dependencies = string.IsNullOrEmpty(Dependencies) ? new() : 
                        Dependencies.Split(',').Select(d => d.Trim()).ToList()
                };

                var success = await _furApiService.UploadPackageAsync(furConfig);

                if (success)
                {
                    Message = "Package submitted successfully!";
                    IsSuccess = true;
                    ModelState.Clear();
                }
                else
                {
                    Message = "Failed to submit package to the API.";
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
