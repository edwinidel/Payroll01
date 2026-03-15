using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _2FA.Data;
using _2FA.Data.Entities;

namespace _2FA.Controllers
{
    public class CostCenterPaymentConceptAccountsController : BaseController
    {
        public CostCenterPaymentConceptAccountsController(ApplicationDbContext context) : base(context)
        {
        }

        // GET: CostCenterPaymentConceptAccounts
        public async Task<IActionResult> Index()
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            var applicationDbContext = _context.CostCenterPaymentConceptAccounts
                .Include(c => c.CostCenter)
                .Include(c => c.PaymentConcept)
                .Include(c => c.Account)
                .Where(c => c.CostCenter.CompanyId == GetCurrentCompanyId());

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: CostCenterPaymentConceptAccounts/Details/5
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

            var costCenterPaymentConceptAccountEntity = await _context.CostCenterPaymentConceptAccounts
                .Include(c => c.CostCenter)
                .Include(c => c.PaymentConcept)
                .Include(c => c.Account)
                .FirstOrDefaultAsync(m => m.Id == id && m.CostCenter.CompanyId == GetCurrentCompanyId());
            if (costCenterPaymentConceptAccountEntity == null)
            {
                return NotFound();
            }

            return View(costCenterPaymentConceptAccountEntity);
        }

        // GET: CostCenterPaymentConceptAccounts/Create
        public IActionResult Create()
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            ViewData["CostCenterId"] = new SelectList(FilterByCurrentCompany(_context.CostCenters), "Id", "Name");
            ViewData["PaymentConceptId"] = new SelectList(_context.PaymentConcepts.Where(pc => pc.Country.Name_es == "Panamá"), "Id", "Name");
            ViewData["AccountId"] = new SelectList(FilterByCurrentCompany(_context.Accounts).Where(a => a.IsActive), "Id", "AccountNumber");
            return View();
        }

        // POST: CostCenterPaymentConceptAccounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CostCenterPaymentConceptAccountEntity costCenterPaymentConceptAccountEntity)
        {
            ModelState.Remove("Created");
            ModelState.Remove("CreatedBy");

            if (ModelState.IsValid)
            {
                costCenterPaymentConceptAccountEntity.Created = DateTime.Now;
                costCenterPaymentConceptAccountEntity.CreatedBy = User.Identity.Name ?? string.Empty;
                _context.Add(costCenterPaymentConceptAccountEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CostCenterId"] = new SelectList(FilterByCurrentCompany(_context.CostCenters), "Id", "Name", costCenterPaymentConceptAccountEntity.CostCenterId);
            ViewData["PaymentConceptId"] = new SelectList(_context.PaymentConcepts.Where(pc => pc.Country.Name_es == "Panamá"), "Id", "Name", costCenterPaymentConceptAccountEntity.PaymentConceptId);
            ViewData["AccountId"] = new SelectList(FilterByCurrentCompany(_context.Accounts).Where(a => a.IsActive), "Id", "AccountNumber", costCenterPaymentConceptAccountEntity.AccountId);
            return View(costCenterPaymentConceptAccountEntity);
        }

        // GET: CostCenterPaymentConceptAccounts/Edit/5
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

            var costCenterPaymentConceptAccountEntity = await _context.CostCenterPaymentConceptAccounts
                .Include(c => c.CostCenter)
                .Include(c => c.Account)
                .FirstOrDefaultAsync(c => c.Id == id && c.CostCenter.CompanyId == GetCurrentCompanyId());
            if (costCenterPaymentConceptAccountEntity == null)
            {
                return NotFound();
            }
            ViewData["CostCenterId"] = new SelectList(FilterByCurrentCompany(_context.CostCenters), "Id", "Name", costCenterPaymentConceptAccountEntity.CostCenterId);
            ViewData["PaymentConceptId"] = new SelectList(_context.PaymentConcepts.Where(pc => pc.Country.Name_es == "Panamá"), "Id", "Name", costCenterPaymentConceptAccountEntity.PaymentConceptId);
            ViewData["AccountId"] = new SelectList(FilterByCurrentCompany(_context.Accounts).Where(a => a.IsActive), "Id", "AccountNumber", costCenterPaymentConceptAccountEntity.AccountId);
            return View(costCenterPaymentConceptAccountEntity);
        }

        // POST: CostCenterPaymentConceptAccounts/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  CostCenterPaymentConceptAccountEntity costCenterPaymentConceptAccountEntity)
        {
            if (id != costCenterPaymentConceptAccountEntity.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Modified");
            ModelState.Remove("ModifiedBy");

            if (ModelState.IsValid)
            {
                try
                {
                    costCenterPaymentConceptAccountEntity.Modified = DateTime.Now;
                    costCenterPaymentConceptAccountEntity.ModifiedBy = User.Identity.Name ?? string.Empty;
                    _context.Update(costCenterPaymentConceptAccountEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CostCenterPaymentConceptAccountEntityExists(costCenterPaymentConceptAccountEntity.Id))
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
            ViewData["CostCenterId"] = new SelectList(FilterByCurrentCompany(_context.CostCenters), "Id", "Name", costCenterPaymentConceptAccountEntity.CostCenterId);
            ViewData["PaymentConceptId"] = new SelectList(_context.PaymentConcepts.Where(pc => pc.Country.Name_es == "Panamá"), "Id", "Name", costCenterPaymentConceptAccountEntity.PaymentConceptId);
            ViewData["AccountId"] = new SelectList(FilterByCurrentCompany(_context.Accounts).Where(a => a.IsActive), "Id", "AccountNumber", costCenterPaymentConceptAccountEntity.AccountId);
            return View(costCenterPaymentConceptAccountEntity);
        }

        // GET: CostCenterPaymentConceptAccounts/Delete/5
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

            var costCenterPaymentConceptAccountEntity = await _context.CostCenterPaymentConceptAccounts
                .Include(c => c.CostCenter)
                .Include(c => c.PaymentConcept)
                .Include(c => c.Account)
                .FirstOrDefaultAsync(m => m.Id == id && m.CostCenter.CompanyId == GetCurrentCompanyId());
            if (costCenterPaymentConceptAccountEntity == null)
            {
                return NotFound();
            }

            return View(costCenterPaymentConceptAccountEntity);
        }

        // POST: CostCenterPaymentConceptAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var costCenterPaymentConceptAccountEntity = await _context.CostCenterPaymentConceptAccounts.FindAsync(id);
            if (costCenterPaymentConceptAccountEntity != null)
            {
                _context.CostCenterPaymentConceptAccounts.Remove(costCenterPaymentConceptAccountEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CostCenterPaymentConceptAccountEntityExists(int id)
        {
            return _context.CostCenterPaymentConceptAccounts.Any(e => e.Id == id);
        }
    }
}
