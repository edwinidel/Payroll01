#nullable disable
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _2FA.Data;
using _2FA.Data.Entities;

namespace _2FA.Areas.Identity.Pages.Account
{
    [Authorize]
    public class PasswordExpiryModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;

        public PasswordExpiryModel(UserManager<AppUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [BindProperty]
        public string ReturnUrl { get; set; }

        public bool RequirePasswordChangeOnExpiry { get; set; }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            RequirePasswordChangeOnExpiry = user.RequirePasswordChangeOnExpiry;
            return Page();
        }

        public async Task<IActionResult> OnPostKeepAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            user.PasswordLastChanged = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return LocalRedirect(ReturnUrl);
        }

        public IActionResult OnPostChange(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            return RedirectToPage("/Account/Manage/ChangePassword", new { area = "Identity", returnUrl = ReturnUrl });
        }
    }
}
