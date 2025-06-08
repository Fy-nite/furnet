using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace furnet.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(ILogger<LogoutModel> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            return await LogoutAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            return await LogoutAsync();
        }

        private async Task<IActionResult> LogoutAsync()
        {
            try
            {
                var userName = User.Identity?.Name ?? "unknown";
                _logger.LogInformation("Starting logout for user: {UserName}", userName);
                
                // Clear session first
                HttpContext.Session.Clear();
                
                // Sign out from cookie authentication
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                
                // Set headers to prevent caching
                Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
                Response.Headers.Add("Pragma", "no-cache");
                Response.Headers.Add("Expires", "0");
                
                _logger.LogInformation("Logout completed for user: {UserName}", userName);
                
                // Simple redirect to home page
                return LocalRedirect("~/");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return LocalRedirect("~/");
            }
        }
    }
}
