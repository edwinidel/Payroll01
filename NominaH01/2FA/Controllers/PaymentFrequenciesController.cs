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
    public class PaymentFrequenciesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentFrequenciesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PaymentFrequencies
        public async Task<IActionResult> Index()
        {
            return View(await _context.PaymentFrequencies.ToListAsync());
        }

        // GET: PaymentFrequencies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentFrequencyEntity = await _context.PaymentFrequencies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paymentFrequencyEntity == null)
            {
                return NotFound();
            }

            return View(paymentFrequencyEntity);
        }

        // GET: PaymentFrequencies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PaymentFrequencies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaymentFrequencyEntity paymentFrequencyEntity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(paymentFrequencyEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(paymentFrequencyEntity);
        }

        // GET: PaymentFrequencies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentFrequencyEntity = await _context.PaymentFrequencies.FindAsync(id);
            if (paymentFrequencyEntity == null)
            {
                return NotFound();
            }
            return View(paymentFrequencyEntity);
        }

        // POST: PaymentFrequencies/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PaymentFrequencyEntity paymentFrequencyEntity)
        {
            if (id != paymentFrequencyEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paymentFrequencyEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentFrequencyEntityExists(paymentFrequencyEntity.Id))
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
            return View(paymentFrequencyEntity);
        }

        // GET: PaymentFrequencies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentFrequencyEntity = await _context.PaymentFrequencies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paymentFrequencyEntity == null)
            {
                return NotFound();
            }

            return View(paymentFrequencyEntity);
        }

        // POST: PaymentFrequencies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paymentFrequencyEntity = await _context.PaymentFrequencies.FindAsync(id);
            if (paymentFrequencyEntity != null)
            {
                _context.PaymentFrequencies.Remove(paymentFrequencyEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentFrequencyEntityExists(int id)
        {
            return _context.PaymentFrequencies.Any(e => e.Id == id);
        }
    }
}
