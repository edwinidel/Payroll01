using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _2FA.Data;
using _2FA.Data.Entities;
using _2FA.Models;

namespace _2FA.Controllers
{
    public class CountriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CountriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Countries
        public async Task<IActionResult> Index()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Configuración", Url = "/Home/Configurations" },
                new BreadcrumbItem { Text = "Listdo de Paises" }
            };

            var applicationDbContext = _context.Countries.OrderBy(c => c.Name_es);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Countries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Configuración", Url = "/Home/Configurations" },
                new BreadcrumbItem { Text = "Listdo de Paises", Url = "/Countries/Index" },
                new BreadcrumbItem { Text = "Detalle de Paises" }
            };

            if (id == null || _context.Countries == null)
            {
                return NotFound();
            }

            var countryEntity = await _context.Countries
                .Include(c => c.CountryTimeZones.Where(c => c.CountryId == id))
                .Include(c => c.States)
                .ThenInclude(c => c.Cities)
                .OrderBy(c => c.Name_es)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (countryEntity == null)
            {
                return NotFound();
            }

            return View(countryEntity);
        }

        // GET: Countries/Create
        public IActionResult Create()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Configuración", Url = "/Home/Configurations" },
                new BreadcrumbItem { Text = "Listdo de Paises", Url = "/Countries/Index" },
                new BreadcrumbItem { Text = "Nuevo País" }
            };

            return View();
        }

        // POST: Countries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CountryEntity countryEntity)
        {
            if (ModelState.IsValid)
            {
                countryEntity.Created   = DateTime.UtcNow;
                countryEntity.CreatedBy = User.Identity.Name ?? string.Empty;
                _context.Add(countryEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(countryEntity);
        }

        // GET: Countries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Configuración", Url = "/Home/Configurations" },
                new BreadcrumbItem { Text = "Listdo de Paises", Url = "/Countries/Index" },
                new BreadcrumbItem { Text = "Editar País" }
            };

            if (id == null || _context.Countries == null)
            {
                return NotFound();
            }

            var countryEntity = await _context.Countries.FindAsync(id);
            if (countryEntity == null)
            {
                return NotFound();
            }
            return View(countryEntity);
        }

        // POST: Countries/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CountryEntity countryEntity)
        {
            if (id != countryEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    countryEntity.Modified = DateTime.UtcNow;
                    countryEntity.ModifiedBy = User.Identity.Name ?? string.Empty;

                    _context.Update(countryEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountryEntityExists(countryEntity.Id))
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
            return View(countryEntity);
        }

        // GET: Countries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Countries == null)
            {
                return NotFound();
            }

            var countryEntity = await _context.Countries
                .Include(c => c.TimeZone)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (countryEntity == null)
            {
                return NotFound();
            }

            return View(countryEntity);
        }

        // POST: Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Countries == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Countries'  is null.");
            }
            var countryEntity = await _context.Countries.FindAsync(id);
            if (countryEntity != null)
            {
                _context.Countries.Remove(countryEntity);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CountryEntityExists(int id)
        {
          return (_context.Countries?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
