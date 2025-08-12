using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Purrnet.Models;
using Purrnet.Services;

namespace Purrnet.Pages.Account
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly ILogger<ProfileModel> _logger;

        public User? CurrentUser { get; set; }
        public List<Package> OwnedPackages { get; set; } = new();
        public List<Package> MaintainedPackages { get; set; } = new();
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }

        public ProfileModel(IUserService userService, ILogger<ProfileModel> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                var gitHubId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(gitHubId))
                {
                    CurrentUser = await _userService.GetUserByGitHubIdAsync(gitHubId);
                }

                if (CurrentUser != null)
                {
                    OwnedPackages = await _userService.GetUserPackagesAsync(CurrentUser.Id);
                    MaintainedPackages = await _userService.GetUserMaintainedPackagesAsync(CurrentUser.Id);
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user profile for user ID {UserId}", userId);
                Message = "An error occurred while loading your profile.";
                IsSuccess = false;
                return Page();
            }
        }
    }
}
