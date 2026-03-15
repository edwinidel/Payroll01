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
    public class DepartmentsController : BaseController
    {
        public DepartmentsController(ApplicationDbContext context) : base(context)
        {
        }

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            var departments = FilterByCurrentCompany(_context.Departments);
            return View(await departments.ToListAsync());
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departmentEntity = await _context.Departments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (departmentEntity == null)
            {
                return NotFound();
            }

            return View(departmentEntity);
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            return View();
        }

        // POST: Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentEntity departmentEntity)
        {
            // Ensure company is selected and set it
            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de crear un departamento.";
                return RedirectToAction("SwitchCompany", "Home");
            }
            departmentEntity.CompanyId = currentCompanyId.Value;

            ModelState.Remove("Created");
            ModelState.Remove("CreatedBy");

            if (ModelState.IsValid)
            {
                departmentEntity.Created = DateTime.UtcNow;
                departmentEntity.CreatedBy = User.Identity.Name ?? string.Empty;

                _context.Add(departmentEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(departmentEntity);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departmentEntity = await _context.Departments.FindAsync(id);
            if (departmentEntity == null)
            {
                return NotFound();
            }
            return View(departmentEntity);
        }

        // POST: Departments/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DepartmentEntity departmentEntity)
        {
            if (id != departmentEntity.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Modified");
            ModelState.Remove("ModifiedBy");

            if (ModelState.IsValid)
            {
                try
                {

                    departmentEntity.Modified = DateTime.UtcNow;
                    departmentEntity.ModifiedBy = User.Identity.Name ?? string.Empty;

                    _context.Update(departmentEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentEntityExists(departmentEntity.Id))
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
            return View(departmentEntity);
        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departmentEntity = await _context.Departments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (departmentEntity == null)
            {
                return NotFound();
            }

            return View(departmentEntity);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var departmentEntity = await _context.Departments.FindAsync(id);
            if (departmentEntity != null)
            {
                _context.Departments.Remove(departmentEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentEntityExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }
}
