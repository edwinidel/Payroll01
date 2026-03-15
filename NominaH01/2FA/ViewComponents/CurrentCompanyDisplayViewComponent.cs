using Microsoft.AspNetCore.Mvc;
using _2FA.Data;
using _2FA.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace _2FA.ViewComponents
{
    public class CurrentCompanyDisplayViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CurrentCompanyDisplayViewComponent(
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
                return Content("");
            }

            var currentCompanyId = HttpContext.Session.GetInt32("SelectedCompanyId");
            string companyName = "Sin Compañía Seleccionada";

            if (currentCompanyId.HasValue)
            {
                var company = await _context.Companies
                    .FirstOrDefaultAsync(c => c.Id == currentCompanyId.Value);

                if (company != null)
                {
                    companyName = company.Name;
                }
            }

            ViewBag.CompanyName = companyName;
            return View();
        }
    }
}