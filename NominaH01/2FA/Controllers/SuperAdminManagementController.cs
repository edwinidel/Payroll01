using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _2FA.Data;
using _2FA.Data.Entities;
using _2FA.Models;

namespace _2FA.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class SuperAdminManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SuperAdminManagementController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SuperAdminManagement
        public async Task<IActionResult> Index()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Administración SuperAdmin" }
            };

            var businessGroups = await _context.BusinessGroups
                .Include(bg => bg.Companies)
                .ToListAsync();

            var viewModels = businessGroups.Select(bg => new BusinessGroupLimitsViewModel
            {
                Id = bg.Id,
                Name = bg.Name,
                MaxCompanies = bg.MaxCompanies,
                MaxEmployees = bg.MaxEmployees,
                CurrentCompanies = bg.Companies?.Count(c => c.IsActive && !c.IsDeleted) ?? 0,
                CurrentEmployees = bg.Companies?
                    .Where(c => c.IsActive && !c.IsDeleted)
                    .Sum(c => _context.Employees.Count(e => e.CompanyId == c.Id && e.Status != "cesante")) ?? 0
            }).ToList();

            return View(viewModels);
        }

        // GET: SuperAdminManagement/ConfigureLimits/5
        public async Task<IActionResult> ConfigureLimits(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var businessGroup = await _context.BusinessGroups
                .Include(bg => bg.Companies)
                .FirstOrDefaultAsync(bg => bg.Id == id);

            if (businessGroup == null)
            {
                return NotFound();
            }

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Administración SuperAdmin", Url = "/SuperAdminManagement/Index" },
                new BreadcrumbItem { Text = $"Configurar Límites - {businessGroup.Name}" }
            };

            var viewModel = new BusinessGroupLimitsViewModel
            {
                Id = businessGroup.Id,
                Name = businessGroup.Name,
                MaxCompanies = businessGroup.MaxCompanies,
                MaxEmployees = businessGroup.MaxEmployees,
                CurrentCompanies = businessGroup.Companies?.Count(c => c.IsActive && !c.IsDeleted) ?? 0,
                CurrentEmployees = businessGroup.Companies?
                    .Where(c => c.IsActive && !c.IsDeleted)
                    .Sum(c => _context.Employees.Count(e => e.CompanyId == c.Id && e.Status != "cesante")) ?? 0
            };

            return View(viewModel);
        }

        // POST: SuperAdminManagement/ConfigureLimits
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfigureLimits(int id, BusinessGroupLimitsViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            var businessGroup = await _context.BusinessGroups.FindAsync(id);
            if (businessGroup == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Validate limits against current usage
                var currentCompanies = await _context.Companies
                    .CountAsync(c => c.BusinessGroupId == id && c.IsActive && !c.IsDeleted);

                var companiesInGroup = await _context.Companies
                    .Where(c => c.BusinessGroupId == id && c.IsActive && !c.IsDeleted)
                    .Select(c => c.Id)
                    .ToListAsync();

                var currentEmployees = await _context.Employees
                    .CountAsync(e => companiesInGroup.Contains(e.CompanyId) && e.Status != "cesante");

                // Check if new limits are below current usage
                if (viewModel.MaxCompanies.HasValue && viewModel.MaxCompanies.Value < currentCompanies)
                {
                    ModelState.AddModelError("MaxCompanies", $"No puede establecer un límite de empresas menor al número actual de empresas activas ({currentCompanies}).");
                }

                if (viewModel.MaxEmployees.HasValue && viewModel.MaxEmployees.Value < currentEmployees)
                {
                    ModelState.AddModelError("MaxEmployees", $"No puede establecer un límite de empleados menor al número actual de empleados activos ({currentEmployees}).");
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        businessGroup.MaxCompanies = viewModel.MaxCompanies;
                        businessGroup.MaxEmployees = viewModel.MaxEmployees;
                        businessGroup.Modified = DateTime.UtcNow;
                        businessGroup.ModifiedBy = User.Identity?.Name ?? string.Empty;

                        _context.Update(businessGroup);
                        await _context.SaveChangesAsync();

                        TempData["Success"] = "Límites actualizados exitosamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!BusinessGroupExists(businessGroup.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }

            // Reload current usage data for the view
            viewModel.CurrentCompanies = await _context.Companies
                .CountAsync(c => c.BusinessGroupId == id && c.IsActive && !c.IsDeleted);

            var companiesInGroupForView = await _context.Companies
                .Where(c => c.BusinessGroupId == id && c.IsActive && !c.IsDeleted)
                .Select(c => c.Id)
                .ToListAsync();

            viewModel.CurrentEmployees = await _context.Employees
                .CountAsync(e => companiesInGroupForView.Contains(e.CompanyId) && e.Status != "cesante");

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Administración SuperAdmin", Url = "/SuperAdminManagement/Index" },
                new BreadcrumbItem { Text = $"Configurar Límites - {businessGroup.Name}" }
            };

            return View(viewModel);
        }

        // GET: SuperAdminManagement/UsageReport
        public async Task<IActionResult> UsageReport()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Administración SuperAdmin", Url = "/SuperAdminManagement/Index" },
                new BreadcrumbItem { Text = "Reporte de Uso" }
            };

            var businessGroups = await _context.BusinessGroups
                .Include(bg => bg.Companies)
                .ToListAsync();

            var viewModels = businessGroups.Select(bg => new BusinessGroupLimitsViewModel
            {
                Id = bg.Id,
                Name = bg.Name,
                MaxCompanies = bg.MaxCompanies,
                MaxEmployees = bg.MaxEmployees,
                CurrentCompanies = bg.Companies?.Count(c => c.IsActive && !c.IsDeleted) ?? 0,
                CurrentEmployees = bg.Companies?
                    .Where(c => c.IsActive && !c.IsDeleted)
                    .Sum(c => _context.Employees.Count(e => e.CompanyId == c.Id && e.Status != "cesante")) ?? 0
            }).ToList();

            return View(viewModels);
        }

        private bool BusinessGroupExists(int id)
        {
            return _context.BusinessGroups.Any(e => e.Id == id);
        }
    }
}