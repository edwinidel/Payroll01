using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _2FA.Data;
using _2FA.Data.Entities;
using System.Security.Claims;

namespace _2FA.Controllers
{
    public class DivisionsController : BaseController
    {
        public DivisionsController(ApplicationDbContext context) : base(context)
        {
        }

        // GET: Divisions
        public async Task<IActionResult> Index()
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            var divisions = FilterByCurrentCompany(_context.Divisions);
            return View(await divisions.ToListAsync());
        }

        // GET: Divisions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            if (id == null)
            {
                return NotFound();
            }

            var divisionEntity = await FilterByCurrentCompany(_context.Divisions)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (divisionEntity == null)
            {
                return NotFound();
            }

            return View(divisionEntity);
        }

        // GET: Divisions/Create
        public IActionResult Create()
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            return View();
        }

        // POST: Divisions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DivisionEntity divisionEntity)
        {
            // Ensure company is selected and set it
            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de crear una división.";
                return RedirectToAction("SwitchCompany", "Home");
            }
            divisionEntity.CompanyId = currentCompanyId.Value;

            if (ModelState.IsValid)
            {
                _context.Add(divisionEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(divisionEntity);
        }

        // GET: Divisions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            if (id == null)
            {
                return NotFound();
            }

            var divisionEntity = await FilterByCurrentCompany(_context.Divisions).FirstOrDefaultAsync(m => m.Id == id);
            if (divisionEntity == null)
            {
                return NotFound();
            }
            return View(divisionEntity);
        }

        // POST: Divisions/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DivisionEntity divisionEntity)
        {
            if (id != divisionEntity.Id)
            {
                return NotFound();
            }

            // Ensure company is selected and set it
            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de editar una división.";
                return RedirectToAction("SwitchCompany", "Home");
            }
            divisionEntity.CompanyId = currentCompanyId.Value;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(divisionEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DivisionEntityExists(divisionEntity.Id))
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
            return View(divisionEntity);
        }

        // GET: Divisions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            if (id == null)
            {
                return NotFound();
            }

            var divisionEntity = await FilterByCurrentCompany(_context.Divisions)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (divisionEntity == null)
            {
                return NotFound();
            }

            return View(divisionEntity);
        }

        // POST: Divisions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var divisionEntity = await FilterByCurrentCompany(_context.Divisions).FirstOrDefaultAsync(m => m.Id == id);
            if (divisionEntity != null)
            {
                _context.Divisions.Remove(divisionEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DivisionEntityExists(int id)
        {
            return _context.Divisions.Any(e => e.Id == id);
        }
    }
}
