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
    public class ProjectsController : BaseController
    {
        public ProjectsController(ApplicationDbContext context) : base(context)
        {
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            var projects = FilterByCurrentCompany(_context.Projects);
            return View(await projects.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            if (id == null)
            {
                return NotFound();
            }

            var projectEntity = await FilterByCurrentCompany(_context.Projects)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectEntity == null)
            {
                return NotFound();
            }

            return View(projectEntity);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            return View();
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectEntity projectEntity)
        {
            // Ensure company is selected and set it
            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de crear un proyecto.";
                return RedirectToAction("SwitchCompany", "Home");
            }
            projectEntity.CompanyId = currentCompanyId.Value;

            if (ModelState.IsValid)
            {
                _context.Add(projectEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(projectEntity);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            if (id == null)
            {
                return NotFound();
            }

            var projectEntity = await FilterByCurrentCompany(_context.Projects).FirstOrDefaultAsync(m => m.Id == id);
            if (projectEntity == null)
            {
                return NotFound();
            }
            return View(projectEntity);
        }

        // POST: Projects/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProjectEntity projectEntity)
        {
            if (id != projectEntity.Id)
            {
                return NotFound();
            }

            // Ensure company is selected and set it
            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de editar un proyecto.";
                return RedirectToAction("SwitchCompany", "Home");
            }
            projectEntity.CompanyId = currentCompanyId.Value;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(projectEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectEntityExists(projectEntity.Id))
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
            return View(projectEntity);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            if (id == null)
            {
                return NotFound();
            }

            var projectEntity = await FilterByCurrentCompany(_context.Projects)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectEntity == null)
            {
                return NotFound();
            }

            return View(projectEntity);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projectEntity = await FilterByCurrentCompany(_context.Projects).FirstOrDefaultAsync(m => m.Id == id);
            if (projectEntity != null)
            {
                _context.Projects.Remove(projectEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectEntityExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
