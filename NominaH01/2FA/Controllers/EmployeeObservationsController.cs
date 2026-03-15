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
    public class EmployeeObservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeObservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EmployeeObservations
        public async Task<IActionResult> Index()
        {
            var observations = _context.EmployeeObservations
                .Include(o => o.Employee)
                .Include(o => o.ObservationType)
                .OrderByDescending(o => o.Created);

            return View(observations);
        }

        // GET: EmployeeObservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeObservationEntity = await _context.EmployeeObservations
                .Include(o => o.Employee)
                .Include(o => o.ObservationType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeeObservationEntity == null)
            {
                return NotFound();
            }

            return View(employeeObservationEntity);
        }

        // GET: EmployeeObservations/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            ViewData["ObservationTypeId"] = new SelectList(_context.ObservationTypes, "Id", "Name");
            return View();
        }

        // POST: EmployeeObservations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeObservationEntity employeeObservationEntity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employeeObservationEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", employeeObservationEntity.EmployeeId);
            ViewData["ObservationTypeId"] = new SelectList(_context.ObservationTypes, "Id", "Name", employeeObservationEntity.ObservationTypeId);
            return View(employeeObservationEntity);
        }

        // GET: EmployeeObservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeObservationEntity = await _context.EmployeeObservations.FindAsync(id);
            if (employeeObservationEntity == null)
            {
                return NotFound();
            }

            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", employeeObservationEntity.EmployeeId);
            ViewData["ObservationTypeId"] = new SelectList(_context.ObservationTypes, "Id", "Name", employeeObservationEntity.ObservationTypeId);
            return View(employeeObservationEntity);
        }

        // POST: EmployeeObservations/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeObservationEntity employeeObservationEntity)
        {
            if (id != employeeObservationEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employeeObservationEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeObservationEntityExists(employeeObservationEntity.Id))
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

            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", employeeObservationEntity.EmployeeId);
            ViewData["ObservationTypeId"] = new SelectList(_context.ObservationTypes, "Id", "Name", employeeObservationEntity.ObservationTypeId);
            return View(employeeObservationEntity);
        }

        // GET: EmployeeObservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeObservationEntity = await _context.EmployeeObservations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeeObservationEntity == null)
            {
                return NotFound();
            }

            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", employeeObservationEntity.EmployeeId);
            ViewData["ObservationTypeId"] = new SelectList(_context.ObservationTypes, "Id", "Name", employeeObservationEntity.ObservationTypeId);
            return View(employeeObservationEntity);
        }

        // POST: EmployeeObservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employeeObservationEntity = await _context.EmployeeObservations.FindAsync(id);
            if (employeeObservationEntity != null)
            {
                _context.EmployeeObservations.Remove(employeeObservationEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeObservationEntityExists(int id)
        {
            return _context.EmployeeObservations.Any(e => e.Id == id);
        }
    }
}
