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
    public class PieceworkUnitTypesController : BaseController
    {
        public PieceworkUnitTypesController(ApplicationDbContext context) : base(context)
        {
        }

        // GET: PieceworkUnitTypes
        public async Task<IActionResult> Index()
        {
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Configuración", Url = "/Home/Configurations" },
                new BreadcrumbItem { Text = "Otras Clasificaciones", Url = "/Home/OtherClasifications" },
                new BreadcrumbItem { Text = "Unidades de Destajo" }
            };

            var units = FilterByCurrentCompany(_context.PieceworkUnitTypes)
                .Include(p => p.Company);

            return View(await units.ToListAsync());
        }

        // GET: PieceworkUnitTypes/Create
        public IActionResult Create()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Configuración", Url = "/Home/Configurations" },
                new BreadcrumbItem { Text = "Otras Clasificaciones", Url = "/Home/OtherClasifications" },
                new BreadcrumbItem { Text = "Unidades de Destajo", Url = "/PieceworkUnitTypes/Index" },
                new BreadcrumbItem { Text = "Nueva Unidad de Destajo" }
            };

            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            var currentCompany = GetCurrentCompanyId();
            ViewData["CompanyId"] = new SelectList(FilterByCurrentCompany(_context.Companies), "Id", "Name", currentCompany);
            return View(new PieceworkUnitTypeEntity { CompanyId = currentCompany ?? 0 });
        }

        // POST: PieceworkUnitTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PieceworkUnitTypeEntity pieceworkUnitType)
        {
            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de crear unidades de destajo.";
                return RedirectToAction("SwitchCompany", "Home");
            }

            pieceworkUnitType.CompanyId = currentCompanyId.Value;

            if (ModelState.IsValid)
            {
                _context.Add(pieceworkUnitType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CompanyId"] = new SelectList(FilterByCurrentCompany(_context.Companies), "Id", "Name", pieceworkUnitType.CompanyId);
            return View(pieceworkUnitType);
        }

        // GET: PieceworkUnitTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Configuración", Url = "/Home/Configurations" },
                new BreadcrumbItem { Text = "Otras Clasificaciones", Url = "/Home/OtherClasifications" },
                new BreadcrumbItem { Text = "Unidades de Destajo", Url = "/PieceworkUnitTypes/Index" },
                new BreadcrumbItem { Text = "Editar Unidad de Destajo" }
            };

            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            if (id == null)
            {
                return NotFound();
            }

            var pieceworkUnitType = await _context.PieceworkUnitTypes.FindAsync(id);
            if (pieceworkUnitType == null)
            {
                return NotFound();
            }

            if (pieceworkUnitType.CompanyId != GetCurrentCompanyId())
            {
                TempData["Error"] = "No tiene permisos para editar esta unidad.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["CompanyId"] = new SelectList(FilterByCurrentCompany(_context.Companies), "Id", "Name", pieceworkUnitType.CompanyId);
            return View(pieceworkUnitType);
        }

        // POST: PieceworkUnitTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PieceworkUnitTypeEntity pieceworkUnitType)
        {
            if (id != pieceworkUnitType.Id)
            {
                return NotFound();
            }

            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de editar unidades de destajo.";
                return RedirectToAction("SwitchCompany", "Home");
            }

            pieceworkUnitType.CompanyId = currentCompanyId.Value;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pieceworkUnitType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PieceworkUnitTypeExists(pieceworkUnitType.Id))
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

            ViewData["CompanyId"] = new SelectList(FilterByCurrentCompany(_context.Companies), "Id", "Name", pieceworkUnitType.CompanyId);
            return View(pieceworkUnitType);
        }

        // GET: PieceworkUnitTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            if (id == null)
            {
                return NotFound();
            }

            var pieceworkUnitType = await _context.PieceworkUnitTypes
                .Include(p => p.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pieceworkUnitType == null)
            {
                return NotFound();
            }

            if (pieceworkUnitType.CompanyId != GetCurrentCompanyId())
            {
                TempData["Error"] = "No tiene permisos para eliminar esta unidad.";
                return RedirectToAction(nameof(Index));
            }

            return View(pieceworkUnitType);
        }

        // POST: PieceworkUnitTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pieceworkUnitType = await _context.PieceworkUnitTypes.FindAsync(id);
            if (pieceworkUnitType != null)
            {
                if (pieceworkUnitType.CompanyId != GetCurrentCompanyId())
                {
                    TempData["Error"] = "No tiene permisos para eliminar esta unidad.";
                    return RedirectToAction(nameof(Index));
                }

                _context.PieceworkUnitTypes.Remove(pieceworkUnitType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PieceworkUnitTypeExists(int id)
        {
            return _context.PieceworkUnitTypes.Any(e => e.Id == id);
        }
    }
}
