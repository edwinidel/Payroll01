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
    public class TypeOfWorkersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TypeOfWorkersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TypeOfWorkers
        public async Task<IActionResult> Index()
        {
            return View(await _context.TypeOfWorkers.ToListAsync());
        }

        // GET: TypeOfWorkers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeOfWorkerEntity = await _context.TypeOfWorkers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (typeOfWorkerEntity == null)
            {
                return NotFound();
            }

            return View(typeOfWorkerEntity);
        }

        // GET: TypeOfWorkers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TypeOfWorkers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TypeOfWorkerEntity typeOfWorkerEntity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(typeOfWorkerEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(typeOfWorkerEntity);
        }

        // GET: TypeOfWorkers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeOfWorkerEntity = await _context.TypeOfWorkers.FindAsync(id);
            if (typeOfWorkerEntity == null)
            {
                return NotFound();
            }
            return View(typeOfWorkerEntity);
        }

        // POST: TypeOfWorkers/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TypeOfWorkerEntity typeOfWorkerEntity)
        {
            if (id != typeOfWorkerEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(typeOfWorkerEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TypeOfWorkerEntityExists(typeOfWorkerEntity.Id))
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
            return View(typeOfWorkerEntity);
        }

        // GET: TypeOfWorkers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeOfWorkerEntity = await _context.TypeOfWorkers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (typeOfWorkerEntity == null)
            {
                return NotFound();
            }

            return View(typeOfWorkerEntity);
        }

        // POST: TypeOfWorkers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var typeOfWorkerEntity = await _context.TypeOfWorkers.FindAsync(id);
            if (typeOfWorkerEntity != null)
            {
                _context.TypeOfWorkers.Remove(typeOfWorkerEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TypeOfWorkerEntityExists(int id)
        {
            return _context.TypeOfWorkers.Any(e => e.Id == id);
        }
    }
}
