using _2FA.Data;
using _2FA.Data.Entities;
using _2FA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace _2FA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context,
            UserManager<AppUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio" }
            };

            return View();
        }

        // GET: Company Switcher
        public async Task<IActionResult> SwitchCompany()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var userCompanies = await _context.UserCompanies
                .Where(uc => uc.UserId == user.Id)
                .Include(uc => uc.Company)
                .Select(uc => uc.Company)
                .ToListAsync();

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Cambiar Compañía" }
            };

            return View(userCompanies);
        }

        // POST: Switch Company
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SwitchCompany(int selectedCompanyId, string returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Verify the user has access to the selected company
            var hasAccess = await _context.UserCompanies
                .AnyAsync(uc => uc.UserId == user.Id && uc.CompanyId == selectedCompanyId);

            if (!hasAccess)
            {
                TempData["Error"] = "No tiene acceso a la compañía seleccionada.";
                return RedirectToAction("SwitchCompany");
            }

            // Update session with new company
            HttpContext.Session.SetInt32("SelectedCompanyId", selectedCompanyId);

            TempData["Success"] = "Compañía cambiada exitosamente.";

            // Always redirect to Home/Index to close open views
            return RedirectToAction("Index");
        }

        // GET: Get Current Company Info (for AJAX)
        [HttpGet]
        public async Task<IActionResult> GetCurrentCompany()
        {
            var companyId = HttpContext.Session.GetInt32("SelectedCompanyId");
            if (companyId.HasValue)
            {
                var company = await _context.Companies.FindAsync(companyId.Value);
                if (company != null)
                {
                    return Json(new
                    {
                        success = true,
                        companyId = company.Id,
                        companyName = company.Name
                    });
                }
            }

            return Json(new { success = false, message = "No company selected" });
        }

        // POST: Switch Company via AJAX
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SwitchCompanyAjax(int selectedCompanyId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Usuario no autenticado" });
            }

            // Verify the user has access to the selected company
            var hasAccess = await _context.UserCompanies
                .AnyAsync(uc => uc.UserId == user.Id && uc.CompanyId == selectedCompanyId);

            if (!hasAccess)
            {
                return Json(new { success = false, message = "No tiene acceso a la compañía seleccionada" });
            }

            // Update session with new company
            HttpContext.Session.SetInt32("SelectedCompanyId", selectedCompanyId);

            // Get company name for response
            var company = await _context.Companies.FindAsync(selectedCompanyId);

            return Json(new
            {
                success = true,
                message = "Compañía cambiada exitosamente",
                companyId = selectedCompanyId,
                companyName = company?.Name ?? "Compañía"
            });
        }

        public IActionResult Privacy()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = Url.Action("Index", "Home") },
                new BreadcrumbItem { Text = "Política de Privacidad", Url = null }
            };

            return View();
        }

        public IActionResult Administration()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Administración" }
            };

            return View();
        }

        public IActionResult Accounting()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Configuración", Url = "/Home/Configuration" },
                new BreadcrumbItem { Text = "Contabilidad" }
            };

            return View();
        }

        public IActionResult Clasifications()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Administración", Url = "/Home/Administration" },
                new BreadcrumbItem { Text = "Clasificaciones" }
            };

            return View();
        }

        public IActionResult Payrolls()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Administración", Url = "/Home/Administration" },
                new BreadcrumbItem { Text = "Planillas" }
            };

            return View();
        }

        public IActionResult Employees()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Administración", Url = "/Home/Administration" },
                new BreadcrumbItem { Text = "Administración de Empleados" }
            };

            return View();
        }

        public IActionResult Configurations()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Configuración" }
            };

            return View();
        }

        public IActionResult Banks()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Configuración", Url = "/Home/Configurations" },
                new BreadcrumbItem { Text = "Configuración de Bancos" }
            };

            return View();
        }

        public IActionResult Correspondence()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Administración" }
            };

            return View();
        }

        public IActionResult OtherClasifications()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Configuración", Url = "/Home/Configurations" },
                new BreadcrumbItem { Text = "Otras Clasificaciones" }
            };

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}