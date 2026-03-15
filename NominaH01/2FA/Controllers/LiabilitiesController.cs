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
    public class LiabilitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LiabilitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Liabilities
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Liabilities
                .Include(l => l.Creditor)
                .Include(l => l.Employee);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Liabilities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var liabilityEntity = await _context.Liabilities
                .Include(l => l.Creditor)
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (liabilityEntity == null)
            {
                return NotFound();
            }

            return View(liabilityEntity);
        }

        // GET: Liabilities/Create
        public IActionResult Create()
        {
            ViewData["CreditorId"] = new SelectList(_context.Creditors, "Id", "Name");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            return View();
        }

        // POST: Liabilities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LiabilityEntity liabilityEntity)
        {
            if (ModelState.IsValid)
            {
                liabilityEntity.Created = DateTime.UtcNow;
                _context.Add(liabilityEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreditorId"] = new SelectList(_context.Creditors, "Id", "Name", liabilityEntity.CreditorId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", liabilityEntity.EmployeeId);
            return View(liabilityEntity);
        }

        // GET: Liabilities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var liabilityEntity = await _context.Liabilities.FindAsync(id);
            if (liabilityEntity == null)
            {
                return NotFound();
            }
            ViewData["CreditorId"] = new SelectList(_context.Creditors, "Id", "Name", liabilityEntity.CreditorId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", liabilityEntity.EmployeeId);
            return View(liabilityEntity);
        }

        // POST: Liabilities/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LiabilityEntity liabilityEntity)
        {
            if (id != liabilityEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    liabilityEntity.Modified = DateTime.UtcNow;
                    _context.Update(liabilityEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LiabilityEntityExists(liabilityEntity.Id))
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
            ViewData["CreditorId"] = new SelectList(_context.Creditors, "Id", "Name", liabilityEntity.CreditorId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", liabilityEntity.EmployeeId);
            return View(liabilityEntity);
        }

        // GET: Liabilities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var liabilityEntity = await _context.Liabilities
                .Include(l => l.Creditor)
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (liabilityEntity == null)
            {
                return NotFound();
            }

            return View(liabilityEntity);
        }

        // POST: Liabilities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var liabilityEntity = await _context.Liabilities.FindAsync(id);
            if (liabilityEntity != null)
            {
                liabilityEntity.Deleted = DateTime.UtcNow;
                liabilityEntity.IsDeleted = true;
                _context.Liabilities.Remove(liabilityEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LiabilityEntityExists(int id)
        {
            return _context.Liabilities.Any(e => e.Id == id);
        }
    }
}
