using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using furnet.Data;

namespace furnet.Pages.Admin
{
    [Authorize]
    public class SetupModel : PageModel
    {
        private readonly FurDbContext _context;
        private readonly IConfiguration _configuration;

        public SetupModel(FurDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Only allow if no admins exist yet
            var hasAdmins = await _context.Users.AnyAsync(u => u.IsAdmin);
            if (hasAdmins)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // don't allow at all.

            return Page();
        }
    }
}
