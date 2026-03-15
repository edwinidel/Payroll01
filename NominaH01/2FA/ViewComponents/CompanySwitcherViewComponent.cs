using Microsoft.AspNetCore.Mvc;
using _2FA.Data;
using _2FA.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace _2FA.ViewComponents
{
    public class CompanySwitcherViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CompanySwitcherViewComponent(
            ApplicationDbContext context,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return View(new List<CompanyEntity>());
            }

            var userCompanies = await _context.UserCompanies
                .Where(uc => uc.UserId == user.Id)
                .Include(uc => uc.Company)
                .Select(uc => uc.Company)
                .ToListAsync();

            var currentCompanyId = HttpContext.Session.GetInt32("SelectedCompanyId");
            ViewBag.CurrentCompanyId = currentCompanyId;

            return View(userCompanies);
        }
    }
}