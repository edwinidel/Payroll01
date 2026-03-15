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
    public class PaymentGroupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentGroupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PaymentGroups
        public async Task<IActionResult> Index()
        {
            var paymentGroups = _context.PaymentGroups
                .Include(pg => pg.PaymentFrequency)
                .OrderBy(pg => pg.Name).ToListAsync();
            return View(paymentGroups);
        }

        // GET: PaymentGroups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentGroupEntity = await _context.PaymentGroups
                .Include(pg => pg.PaymentFrequency)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paymentGroupEntity == null)
            {
                return NotFound();
            }

            return View(paymentGroupEntity);
        }

        // GET: PaymentGroups/Create
        public IActionResult Create()
        {
            ViewData["PaymentFrequencyId"] = new SelectList(_context.PaymentFrequencies, "Id", "Name");
            return View();
        }

        // POST: PaymentGroups/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaymentGroupEntity paymentGroupEntity)
        {
            if (ModelState.IsValid)
            {
                paymentGroupEntity.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                paymentGroupEntity.Created = DateTime.UtcNow;
                _context.Add(paymentGroupEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(paymentGroupEntity);
        }

        // GET: PaymentGroups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentGroupEntity = await _context.PaymentGroups.FindAsync(id);
            if (paymentGroupEntity == null)
            {
                return NotFound();
            }

            ViewData["PaymentFrequencyId"] = new SelectList(_context.PaymentFrequencies, "Id", "Name", paymentGroupEntity.PaymentFrequencyId);
            return View(paymentGroupEntity);
        }

        // POST: PaymentGroups/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PaymentGroupEntity paymentGroupEntity)
        {
            if (id != paymentGroupEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paymentGroupEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentGroupEntityExists(paymentGroupEntity.Id))
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

            ViewData["PaymentFrequencyId"] = new SelectList(_context.PaymentFrequencies, "Id", "Name", paymentGroupEntity.PaymentFrequencyId);
            return View(paymentGroupEntity);
        }

        // GET: PaymentGroups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentGroupEntity = await _context.PaymentGroups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paymentGroupEntity == null)
            {
                return NotFound();
            }

            ViewData["PaymentFrequencyId"] = new SelectList(_context.PaymentFrequencies, "Id", "Name", paymentGroupEntity.PaymentFrequencyId);
            return View(paymentGroupEntity);
        }

        // POST: PaymentGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paymentGroupEntity = await _context.PaymentGroups.FindAsync(id);
            if (paymentGroupEntity != null)
            {
                _context.PaymentGroups.Remove(paymentGroupEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentGroupEntityExists(int id)
        {
            return _context.PaymentGroups.Any(e => e.Id == id);
        }
    }
}
