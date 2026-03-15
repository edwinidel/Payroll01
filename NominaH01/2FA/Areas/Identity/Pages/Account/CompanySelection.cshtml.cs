using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using _2FA.Data;
using _2FA.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace _2FA.Areas.Identity.Pages.Account
{
    public class CompanySelectionModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CompanySelectionModel(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<CompanyEntity> UserCompanies { get; set; } = new List<CompanyEntity>();
        public string ReturnUrl { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("Login");
            }

            UserCompanies = await _context.UserCompanies
                .Where(uc => uc.UserId == user.Id)
                .Include(uc => uc.Company)
                .Select(uc => uc.Company)
                .ToListAsync();

            // Check if user is administrator
            var isAdmin = await _userManager.IsInRoleAsync(user, "Administrator");

            if (UserCompanies.Count == 1 && !isAdmin)
            {
                // Auto-select for regular users with single company
                HttpContext.Session.SetInt32("SelectedCompanyId", UserCompanies.First().Id);
                return LocalRedirect(ReturnUrl);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int selectedCompanyId, string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            HttpContext.Session.SetInt32("SelectedCompanyId", selectedCompanyId);

            return LocalRedirect(ReturnUrl);
        }
    }
}