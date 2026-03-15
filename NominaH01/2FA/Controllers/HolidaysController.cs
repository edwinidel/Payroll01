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
    public class HolidaysController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HolidaysController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Holidays
        public async Task<IActionResult> Index()
        {
            return View(await _context.HoliDays.ToListAsync());
        }

        // GET: Holidays/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var holiDayEntity = await _context.HoliDays
                .FirstOrDefaultAsync(m => m.Id == id);
            if (holiDayEntity == null)
            {
                return NotFound();
            }

            return View(holiDayEntity);
        }

        // GET: Holidays/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Holidays/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HoliDayEntity holiDayEntity)
        {
            if (ModelState.IsValid)
            {
                holiDayEntity.Created = DateTime.UtcNow;
                _context.Add(holiDayEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(holiDayEntity);
        }

        // GET: Holidays/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var holiDayEntity = await _context.HoliDays.FindAsync(id);
            if (holiDayEntity == null)
            {
                return NotFound();
            }
            return View(holiDayEntity);
        }

        // POST: Holidays/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HoliDayEntity holiDayEntity)
        {
            if (id != holiDayEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    holiDayEntity.Modified = DateTime.UtcNow;
                    _context.Update(holiDayEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoliDayEntityExists(holiDayEntity.Id))
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
            return View(holiDayEntity);
        }

        // GET: Holidays/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var holiDayEntity = await _context.HoliDays
                .FirstOrDefaultAsync(m => m.Id == id);
            if (holiDayEntity == null)
            {
                return NotFound();
            }

            return View(holiDayEntity);
        }

        // POST: Holidays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var holiDayEntity = await _context.HoliDays.FindAsync(id);
            if (holiDayEntity != null)
            {
                holiDayEntity.Deleted = DateTime.UtcNow;
                holiDayEntity.IsActive = false;
                holiDayEntity.IsDeleted = true;
                _context.HoliDays.Update(holiDayEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HoliDayEntityExists(int id)
        {
            return _context.HoliDays.Any(e => e.Id == id);
        }
    }
}
