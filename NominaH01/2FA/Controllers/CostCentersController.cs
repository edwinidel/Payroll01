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
    public class CostCentersController : BaseController
    {
        public CostCentersController(ApplicationDbContext context) : base(context)
        {
        }

        // GET: CostCenters
        public async Task<IActionResult> Index()
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            var costCenters = FilterByCurrentCompany(_context.CostCenters);
            return View(await costCenters.ToListAsync());
        }

        // GET: CostCenters/Details/5
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

            var costCenterEntity = await FilterByCurrentCompany(_context.CostCenters)
                .Include(c => c.CostCenterPaymentConceptAccounts)
                    .ThenInclude(ccpca => ccpca.PaymentConcept)
                .Include(c => c.CostCenterPaymentConceptAccounts)
                    .ThenInclude(ccpca => ccpca.Account)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (costCenterEntity == null)
            {
                return NotFound();
            }

            return View(costCenterEntity);
        }

        // GET: CostCenters/Create
        public IActionResult Create()
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            return View();
        }

        // POST: CostCenters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CostCenterEntity costCenterEntity)
        {
            // Ensure company is selected and set it
            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de crear un centro de costo.";
                return RedirectToAction("SwitchCompany", "Home");
            }
            costCenterEntity.CompanyId = currentCompanyId.Value;

            ModelState.Remove("Created");
            ModelState.Remove("CreatedBy");

            if (ModelState.IsValid)
            {
                costCenterEntity.Created = DateTime.UtcNow;
                costCenterEntity.CreatedBy = User.Identity.Name ?? string.Empty;
 
                _context.Add(costCenterEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(costCenterEntity);
        }

        // GET: CostCenters/Edit/5
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

            var costCenterEntity = await FilterByCurrentCompany(_context.CostCenters).FirstOrDefaultAsync(m => m.Id == id);
            var departmentsQuery = FilterByCurrentCompany(_context.Departments)
                        .OrderBy(d => d.Name)
                        .AsNoTracking();
            ViewBag.DepartmentId = new SelectList(departmentsQuery, "Id", "Name");


            if (costCenterEntity == null)
            {
                return NotFound();
            }
            return View(costCenterEntity);
        }

        // POST: CostCenters/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CostCenterEntity costCenterEntity)
        {
            if (id != costCenterEntity.Id)
            {
                return NotFound();
            }

            // Ensure company is selected and set it
            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de editar un centro de costo.";
                return RedirectToAction("SwitchCompany", "Home");
            }
            costCenterEntity.CompanyId = currentCompanyId.Value;

            ModelState.Remove("Modified");
            ModelState.Remove("ModifiedBy");

            if (ModelState.IsValid)
            {
                try
                {
                    costCenterEntity.Modified = DateTime.UtcNow;
                    costCenterEntity.ModifiedBy = User.Identity.Name ?? string.Empty;
                    _context.Update(costCenterEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CostCenterEntityExists(costCenterEntity.Id))
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
            return View(costCenterEntity);
        }

        // GET: CostCenters/Delete/5
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

            var costCenterEntity = await FilterByCurrentCompany(_context.CostCenters)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (costCenterEntity == null)
            {
                return NotFound();
            }

            return View(costCenterEntity);
        }

        // POST: CostCenters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var costCenterEntity = await FilterByCurrentCompany(_context.CostCenters).FirstOrDefaultAsync(m => m.Id == id);
            if (costCenterEntity != null)
            {
                costCenterEntity.Deleted = DateTime.UtcNow;
                costCenterEntity.DeletedBy = User.Identity.Name ?? string.Empty;
                _context.CostCenters.Remove(costCenterEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CostCenterEntityExists(int id)
        {
            return _context.CostCenters.Any(e => e.Id == id);
        }
    }
}
