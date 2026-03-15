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
    public class SectionsController : BaseController
    {
        public SectionsController(ApplicationDbContext context) : base(context)
        {
        }

        // GET: Sections
        public async Task<IActionResult> Index()
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            var sections = FilterByCurrentCompany(_context.Sections);
            return View(await sections.ToListAsync());
        }

        // GET: Sections/Details/5
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

            var sectionEntity = await FilterByCurrentCompany(_context.Sections)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sectionEntity == null)
            {
                return NotFound();
            }

            return View(sectionEntity);
        }

        // GET: Sections/Create
        public IActionResult Create()
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            return View();
        }

        // POST: Sections/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SectionEntity sectionEntity)
        {
            // Ensure company is selected and set it
            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de crear una sección.";
                return RedirectToAction("SwitchCompany", "Home");
            }
            sectionEntity.CompanyId = currentCompanyId.Value;

            if (ModelState.IsValid)
            {
                _context.Add(sectionEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sectionEntity);
        }

        // GET: Sections/Edit/5
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

            var sectionEntity = await FilterByCurrentCompany(_context.Sections).FirstOrDefaultAsync(m => m.Id == id);
            if (sectionEntity == null)
            {
                return NotFound();
            }
            return View(sectionEntity);
        }

        // POST: Sections/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SectionEntity sectionEntity)
        {
            if (id != sectionEntity.Id)
            {
                return NotFound();
            }

            // Ensure company is selected and set it
            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de editar una sección.";
                return RedirectToAction("SwitchCompany", "Home");
            }
            sectionEntity.CompanyId = currentCompanyId.Value;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sectionEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SectionEntityExists(sectionEntity.Id))
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
            return View(sectionEntity);
        }

        // GET: Sections/Delete/5
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

            var sectionEntity = await FilterByCurrentCompany(_context.Sections)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sectionEntity == null)
            {
                return NotFound();
            }

            return View(sectionEntity);
        }

        // POST: Sections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sectionEntity = await FilterByCurrentCompany(_context.Sections).FirstOrDefaultAsync(m => m.Id == id);
            if (sectionEntity != null)
            {
                _context.Sections.Remove(sectionEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SectionEntityExists(int id)
        {
            return _context.Sections.Any(e => e.Id == id);
        }
    }
}
