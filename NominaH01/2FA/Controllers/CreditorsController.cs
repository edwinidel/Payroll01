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
    public class CreditorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CreditorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Creditors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Creditors.ToListAsync());
        }

        // GET: Creditors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var creditorEntity = await _context.Creditors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (creditorEntity == null)
            {
                return NotFound();
            }

            return View(creditorEntity);
        }

        // GET: Creditors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Creditors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreditorEntity creditorEntity)
        {
            if (ModelState.IsValid)
            {
                creditorEntity.Created = DateTime.UtcNow;
                creditorEntity.CreatedBy = User.Identity.Name ?? string.Empty;
                _context.Add(creditorEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(creditorEntity);
        }

        // GET: Creditors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var creditorEntity = await _context.Creditors.FindAsync(id);
            if (creditorEntity == null)
            {
                return NotFound();
            }
            return View(creditorEntity);
        }

        // POST: Creditors/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreditorEntity creditorEntity)
        {
            if (id != creditorEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    creditorEntity.Modified = DateTime.UtcNow;
                    creditorEntity.ModifiedBy = User.Identity.Name ?? string.Empty;
                    _context.Update(creditorEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CreditorEntityExists(creditorEntity.Id))
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
            return View(creditorEntity);
        }

        // GET: Creditors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var creditorEntity = await _context.Creditors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (creditorEntity == null)
            {
                return NotFound();
            }

            return View(creditorEntity);
        }

        // POST: Creditors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var creditorEntity = await _context.Creditors.FindAsync(id);
            if (creditorEntity != null)
            {
                creditorEntity.Deleted = DateTime.UtcNow;
                creditorEntity.IsDeleted = true;

                _context.Creditors.Remove(creditorEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CreditorEntityExists(int id)
        {
            return _context.Creditors.Any(e => e.Id == id);
        }
    }
}
