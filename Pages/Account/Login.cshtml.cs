using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace furnet.Pages.Account
{
    public class LoginModel : PageModel
    {
        public string? ReturnUrl { get; set; }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? "/";
        }

        public IActionResult OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= "/";

            return Challenge(new AuthenticationProperties
            {
                RedirectUri = returnUrl
            }, "GitHub");
        }
    }
}
