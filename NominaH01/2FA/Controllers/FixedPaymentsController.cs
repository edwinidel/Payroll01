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
    public class FixedPaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FixedPaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FixedPayments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.FixedPayments
                .Include(f => f.Employee)
                .Include(f => f.PaymentConcept);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: FixedPayments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fixedPaymentEntity = await _context.FixedPayments
                .Include(f => f.Employee)
                .Include(f => f.PaymentConcept)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fixedPaymentEntity == null)
            {
                return NotFound();
            }

            return View(fixedPaymentEntity);
        }

        // GET: FixedPayments/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName");
            ViewData["PaymentConceptId"] = new SelectList(_context.PaymentConcepts.Where(pc => pc.RecurrentPayment), "Id", "Name");
            return View();
        }

        // POST: FixedPayments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FixedPaymentEntity fixedPaymentEntity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fixedPaymentEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName", fixedPaymentEntity.EmployeeId);
            ViewData["PaymentConceptId"] = new SelectList(_context.PaymentConcepts.Where(pc => pc.RecurrentPayment), "Id", "Name", fixedPaymentEntity.PaymentConceptId);
            return View(fixedPaymentEntity);
        }

        // GET: FixedPayments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fixedPaymentEntity = await _context.FixedPayments.FindAsync(id);
            if (fixedPaymentEntity == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName", fixedPaymentEntity.EmployeeId);
            ViewData["PaymentConceptId"] = new SelectList(_context.PaymentConcepts.Where(pc => pc.RecurrentPayment), "Id", "Name", fixedPaymentEntity.PaymentConceptId);
            return View(fixedPaymentEntity);
        }

        // POST: FixedPayments/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FixedPaymentEntity fixedPaymentEntity)
        {
            if (id != fixedPaymentEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fixedPaymentEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FixedPaymentEntityExists(fixedPaymentEntity.Id))
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
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName", fixedPaymentEntity.EmployeeId);
            ViewData["PaymentConceptId"] = new SelectList(_context.PaymentConcepts.Where(pc => pc.RecurrentPayment), "Id", "Name", fixedPaymentEntity.PaymentConceptId);
            return View(fixedPaymentEntity);
        }

        // GET: FixedPayments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fixedPaymentEntity = await _context.FixedPayments
                .Include(f => f.Employee)
                .Include(f => f.PaymentConcept)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fixedPaymentEntity == null)
            {
                return NotFound();
            }

            return View(fixedPaymentEntity);
        }

        // POST: FixedPayments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fixedPaymentEntity = await _context.FixedPayments.FindAsync(id);
            if (fixedPaymentEntity != null)
            {
                _context.FixedPayments.Remove(fixedPaymentEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FixedPaymentEntityExists(int id)
        {
            return _context.FixedPayments.Any(e => e.Id == id);
        }
    }
}
