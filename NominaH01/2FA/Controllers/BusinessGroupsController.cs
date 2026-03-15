using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _2FA.Data;
using _2FA.Data.Entities;
using _2FA.Models;

namespace _2FA.Controllers
{
    public class BusinessGroupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BusinessGroupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BusinessGroups
        public async Task<IActionResult> Index()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Configuración", Url = "/Home/Configurations" },
                new BreadcrumbItem { Text = "Grupo de Negocios" }
            };

            return View(await _context.BusinessGroups.ToListAsync());
        }

        // GET: BusinessGroups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Configuración", Url = "/Home/Configurations" },
                new BreadcrumbItem { Text = "Grupo de Negocios", Url = "/BusinessGroups/Index" },
                new BreadcrumbItem { Text = "Detalle de Grupo de Negocios" }
            };

            if (id == null)
            {
                return NotFound();
            }

            var businessGroupEntity = await _context.BusinessGroups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (businessGroupEntity == null)
            {
                return NotFound();
            }

            // Calculate usage statistics
            var currentCompanies = await _context.Companies
                .Where(c => c.BusinessGroupId == id && c.IsActive && !c.IsDeleted)
                .CountAsync();

            var companiesInGroup = await _context.Companies
                .Where(c => c.BusinessGroupId == id)
                .Select(c => c.Id)
                .ToListAsync();

            var currentEmployees = await _context.Employees
                .Where(e => companiesInGroup.Contains(e.CompanyId) && e.Status != "cesante")
                .CountAsync();

            // Set ViewData for usage statistics
            ViewData["CurrentCompanies"] = currentCompanies;
            ViewData["CurrentEmployees"] = currentEmployees;
            ViewData["AvailableCompanies"] = businessGroupEntity.MaxCompanies.HasValue
                ? Math.Max(0, businessGroupEntity.MaxCompanies.Value - currentCompanies)
                : (int?)null;
            ViewData["AvailableEmployees"] = businessGroupEntity.MaxEmployees.HasValue
                ? Math.Max(0, businessGroupEntity.MaxEmployees.Value - currentEmployees)
                : (int?)null;

            return View(businessGroupEntity);
        }

        // GET: BusinessGroups/Create
        public IActionResult Create()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Configuración", Url = "/Home/Configurations" },
                new BreadcrumbItem { Text = "Grupo de Negocios", Url = "/BusinessGroups/Index" },
                new BreadcrumbItem { Text = "Nuevo Grupo de Negocios" }
            };

            return View();
        }

        // POST: BusinessGroups/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BusinessGroupEntity businessGroupEntity)
        {
            if (ModelState.IsValid)
            {
                businessGroupEntity.Created = DateTime.UtcNow;
                businessGroupEntity.CreatedBy = User.Identity.Name ?? string.Empty;
                _context.Add(businessGroupEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(businessGroupEntity);
        }

        // GET: BusinessGroups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Configuración", Url = "/Home/Configurations" },
                new BreadcrumbItem { Text = "Grupo de Negocios", Url = "/BusinessGroups/Index" },
                new BreadcrumbItem { Text = "Editar Grupo de Negocios" }
            };

            if (id == null)
            {
                return NotFound();
            }

            var businessGroupEntity = await _context.BusinessGroups.FindAsync(id);
            if (businessGroupEntity == null)
            {
                return NotFound();
            }
            return View(businessGroupEntity);
        }

        // POST: BusinessGroups/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BusinessGroupEntity businessGroupEntity)
        {
            if (id != businessGroupEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    businessGroupEntity.Modified = DateTime.UtcNow;
                    businessGroupEntity.ModifiedBy = User.Identity.Name ?? string.Empty;
                    _context.Update(businessGroupEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BusinessGroupEntityExists(businessGroupEntity.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(businessGroupEntity);
        }

        // GET: BusinessGroups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var businessGroupEntity = await _context.BusinessGroups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (businessGroupEntity == null)
            {
                return NotFound();
            }

            return View(businessGroupEntity);
        }

        // POST: BusinessGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var businessGroupEntity = await _context.BusinessGroups.FindAsync(id);
            if (businessGroupEntity != null)
            {
                _context.BusinessGroups.Remove(businessGroupEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BusinessGroupEntityExists(int id)
        {
            return _context.BusinessGroups.Any(e => e.Id == id);
        }
    }
}
