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
    public class ScheduleEntitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScheduleEntitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ScheduleEntities
        public async Task<IActionResult> Index()
        {
            return View(await _context.Schedules.ToListAsync());
        }

        // GET: ScheduleEntities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleEntity = await _context.Schedules
                .FirstOrDefaultAsync(m => m.Id == id);
            if (scheduleEntity == null)
            {
                return NotFound();
            }

            return View(scheduleEntity);
        }

        // GET: ScheduleEntities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ScheduleEntities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ScheduleEntity scheduleEntity)
        {
            ModelState.Remove("Id");
            ModelState.Remove("Created");
            ModelState.Remove("CreatedBy");
            if (ModelState.IsValid)
            {
                scheduleEntity.Created = DateTime.UtcNow;
                scheduleEntity.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Add(scheduleEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(scheduleEntity);
        }

        // GET: ScheduleEntities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleEntity = await _context.Schedules.FindAsync(id);
            if (scheduleEntity == null)
            {
                return NotFound();
            }
            return View(scheduleEntity);
        }

        // POST: ScheduleEntities/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ScheduleEntity scheduleEntity)
        {
            if (id != scheduleEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(scheduleEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleEntityExists(scheduleEntity.Id))
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
            return View(scheduleEntity);
        }

        // GET: ScheduleEntities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleEntity = await _context.Schedules
                .FirstOrDefaultAsync(m => m.Id == id);
            if (scheduleEntity == null)
            {
                return NotFound();
            }

            return View(scheduleEntity);
        }

        // POST: ScheduleEntities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var scheduleEntity = await _context.Schedules.FindAsync(id);
            if (scheduleEntity != null)
            {
                _context.Schedules.Remove(scheduleEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScheduleEntityExists(int id)
        {
            return _context.Schedules.Any(e => e.Id == id);
        }
    }
}
