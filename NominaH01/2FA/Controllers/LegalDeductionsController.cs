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
    public class LegalDeductionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LegalDeductionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task LoadLookupDataAsync(object? selectedPayrollTypeId = null, object? selectedCountryId = null)
        {
            ViewData["PayrollTypeId"] = new SelectList(
                await _context.PayrollTypes.OrderBy(p => p.Name).ToListAsync(),
                "Id",
                "Name",
                selectedPayrollTypeId);

            ViewData["CountryId"] = new SelectList(
                await _context.Countries.OrderBy(c => c.Name_es).ToListAsync(),
                "Id",
                "Name_es",
                selectedCountryId);
        }

        // GET: LegalDeductions
        public async Task<IActionResult> Index()
        {
            var items = await _context.LegalDeductionEntity
                .Include(l => l.Country)
                .Include(l => l.PayrollType)
                .Where(l => !l.IsDeleted)
                .OrderBy(l => l.Country!.Name_es)
                .ThenBy(l => l.Name)
                .ToListAsync();

            return View(items);
        }

        // GET: LegalDeductions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var legalDeductionEntity = await _context.LegalDeductionEntity
                .Include(l => l.Country)
                .Include(l => l.PayrollType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (legalDeductionEntity == null)
            {
                return NotFound();
            }

            return View(legalDeductionEntity);
        }

        // GET: LegalDeductions/Create
        public async Task<IActionResult> Create()
        {
            await LoadLookupDataAsync();
            return View();
        }

        // POST: LegalDeductions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LegalDeductionEntity legalDeductionEntity)
        {
            if (ModelState.IsValid)
            {
                legalDeductionEntity.Created = DateTime.UtcNow;
                legalDeductionEntity.CreatedBy = User?.Identity?.Name ?? "system";
                _context.Add(legalDeductionEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await LoadLookupDataAsync(legalDeductionEntity.PayrollTypeId, legalDeductionEntity.CountryId);
            return View(legalDeductionEntity);
        }

        // GET: LegalDeductions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var legalDeductionEntity = await _context.LegalDeductionEntity.FindAsync(id);
            if (legalDeductionEntity == null)
            {
                return NotFound();
            }

            await LoadLookupDataAsync(legalDeductionEntity.PayrollTypeId, legalDeductionEntity.CountryId);
            return View(legalDeductionEntity);
        }

        // POST: LegalDeductions/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LegalDeductionEntity legalDeductionEntity)
        {
            if (id != legalDeductionEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    legalDeductionEntity.Modified = DateTime.UtcNow;
                    legalDeductionEntity.ModifiedBy = User?.Identity?.Name ?? "system";
                    _context.Update(legalDeductionEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LegalDeductionEntityExists(legalDeductionEntity.Id))
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

            await LoadLookupDataAsync(legalDeductionEntity.PayrollTypeId, legalDeductionEntity.CountryId);
            return View(legalDeductionEntity);
        }

        // GET: LegalDeductions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var legalDeductionEntity = await _context.LegalDeductionEntity
                .Include(l => l.Country)
                .Include(l => l.PayrollType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (legalDeductionEntity == null)
            {
                return NotFound();
            }

            return View(legalDeductionEntity);
        }

        // POST: LegalDeductions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var legalDeductionEntity = await _context.LegalDeductionEntity.FindAsync(id);
            if (legalDeductionEntity != null)
            {
                legalDeductionEntity.Deleted = DateTime.UtcNow;
                legalDeductionEntity.IsDeleted = true;
                _context.LegalDeductionEntity.Update(legalDeductionEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LegalDeductionEntityExists(int id)
        {
            return _context.LegalDeductionEntity.Any(e => e.Id == id);
        }
    }
}
