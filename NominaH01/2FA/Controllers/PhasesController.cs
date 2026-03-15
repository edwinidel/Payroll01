using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _2FA.Data;
using _2FA.Data.Entities;

namespace _2FA.Controllers
{
    public class PhasesController : BaseController
    {
        public PhasesController(ApplicationDbContext context) : base(context)
        {
        }

        // GET: Phases
        public async Task<IActionResult> Index()
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            var phases = FilterByCurrentCompany(_context.Phases);
            return View(await phases.ToListAsync());
        }

        // GET: Phases/Details/5
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

            var phaseEntity = await FilterByCurrentCompany(_context.Phases)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (phaseEntity == null)
            {
                return NotFound();
            }

            return View(phaseEntity);
        }

        // GET: Phases/Create
        public IActionResult Create()
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            return View();
        }

        // POST: Phases/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhaseEntity phaseEntity)
        {
            // Ensure company is selected and set it
            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de crear una fase.";
                return RedirectToAction("SwitchCompany", "Home");
            }
            phaseEntity.CompanyId = currentCompanyId.Value;

            if (ModelState.IsValid)
            {
                _context.Add(phaseEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(phaseEntity);
        }

        // GET: Phases/Edit/5
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

            var phaseEntity = await FilterByCurrentCompany(_context.Phases).FirstOrDefaultAsync(m => m.Id == id);
            if (phaseEntity == null)
            {
                return NotFound();
            }
            return View(phaseEntity);
        }

        // POST: Phases/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PhaseEntity phaseEntity)
        {
            if (id != phaseEntity.Id)
            {
                return NotFound();
            }

            // Ensure company is selected and set it
            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de editar una fase.";
                return RedirectToAction("SwitchCompany", "Home");
            }
            phaseEntity.CompanyId = currentCompanyId.Value;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phaseEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhaseEntityExists(phaseEntity.Id))
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
            return View(phaseEntity);
        }

        // GET: Phases/Delete/5
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

            var phaseEntity = await FilterByCurrentCompany(_context.Phases)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (phaseEntity == null)
            {
                return NotFound();
            }

            return View(phaseEntity);
        }

        // POST: Phases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var phaseEntity = await FilterByCurrentCompany(_context.Phases).FirstOrDefaultAsync(m => m.Id == id);
            if (phaseEntity != null)
            {
                _context.Phases.Remove(phaseEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhaseEntityExists(int id)
        {
            return _context.Phases.Any(e => e.Id == id);
        }
    }
}
