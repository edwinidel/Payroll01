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
    public class BranchesController : BaseController
    {
        public BranchesController(ApplicationDbContext context) : base(context)
        {
        }

        // GET: Branches
        public async Task<IActionResult> Index()
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            var branches = FilterByCurrentCompany(_context.Branches);
            return View(await branches.ToListAsync());
        }

        // GET: Branches/Details/5
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

            var branchEntity = await FilterByCurrentCompany(_context.Branches)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (branchEntity == null)
            {
                return NotFound();
            }

            return View(branchEntity);
        }

        // GET: Branches/Create
        public IActionResult Create()
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            return View();
        }

        // POST: Branches/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BranchEntity branchEntity)
        {
            // Ensure company is selected and set it
            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de crear una sucursal.";
                return RedirectToAction("SwitchCompany", "Home");
            }
            branchEntity.CompanyId = currentCompanyId.Value;

            if (ModelState.IsValid)
            {
                branchEntity.Created = DateTime.UtcNow;
                branchEntity.CreatedBy = User.Identity.Name ?? string.Empty;
                _context.Add(branchEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(branchEntity);
        }

        // GET: Branches/Edit/5
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

            var branchEntity = await FilterByCurrentCompany(_context.Branches).FirstOrDefaultAsync(m => m.Id == id);
            if (branchEntity == null)
            {
                return NotFound();
            }
            return View(branchEntity);
        }

        // POST: Branches/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BranchEntity branchEntity)
        {
            if (id != branchEntity.Id)
            {
                return NotFound();
            }

            // Ensure company is selected and set it
            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de editar una sucursal.";
                return RedirectToAction("SwitchCompany", "Home");
            }
            branchEntity.CompanyId = currentCompanyId.Value;

            if (ModelState.IsValid)
            {
                try
                {
                    branchEntity.Modified = DateTime.UtcNow;
                    branchEntity.ModifiedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    branchEntity.CreatedBy = User.Identity.Name ?? string.Empty;
                    _context.Update(branchEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BranchEntityExists(branchEntity.Id))
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
            return View(branchEntity);
        }

        // GET: Branches/Delete/5
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

            var branchEntity = await FilterByCurrentCompany(_context.Branches)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (branchEntity == null)
            {
                return NotFound();
            }

            return View(branchEntity);
        }

        // POST: Branches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var branchEntity = await FilterByCurrentCompany(_context.Branches).FirstOrDefaultAsync(m => m.Id == id);
            if (branchEntity != null)
            {
                branchEntity.Deleted = DateTime.UtcNow;
                branchEntity.DeletedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                branchEntity.IsDeleted = true;
                branchEntity.IsActive = false;
                _context.Branches.Update(branchEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BranchEntityExists(int id)
        {
            return _context.Branches.Any(e => e.Id == id);
        }
    }
}
