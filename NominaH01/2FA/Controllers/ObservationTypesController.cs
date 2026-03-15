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
    public class ObservationTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ObservationTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ObservationTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.ObservationTypes.ToListAsync());
        }

        // GET: ObservationTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var observationTypeEntity = await _context.ObservationTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (observationTypeEntity == null)
            {
                return NotFound();
            }

            return View(observationTypeEntity);
        }

        // GET: ObservationTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ObservationTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ObservationTypeEntity observationTypeEntity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(observationTypeEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(observationTypeEntity);
        }

        // GET: ObservationTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var observationTypeEntity = await _context.ObservationTypes.FindAsync(id);
            if (observationTypeEntity == null)
            {
                return NotFound();
            }
            return View(observationTypeEntity);
        }

        // POST: ObservationTypes/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ObservationTypeEntity observationTypeEntity)
        {
            if (id != observationTypeEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(observationTypeEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ObservationTypeEntityExists(observationTypeEntity.Id))
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
            return View(observationTypeEntity);
        }

        // GET: ObservationTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var observationTypeEntity = await _context.ObservationTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (observationTypeEntity == null)
            {
                return NotFound();
            }

            return View(observationTypeEntity);
        }

        // POST: ObservationTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var observationTypeEntity = await _context.ObservationTypes.FindAsync(id);
            if (observationTypeEntity != null)
            {
                _context.ObservationTypes.Remove(observationTypeEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ObservationTypeEntityExists(int id)
        {
            return _context.ObservationTypes.Any(e => e.Id == id);
        }
    }
}
