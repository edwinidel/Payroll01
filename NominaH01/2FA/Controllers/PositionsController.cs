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
using _2FA.Models;

namespace _2FA.Controllers
{
    public class PositionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PositionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Positions
        public async Task<IActionResult> Index()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Administración", Url = "/Home/Administration" },
                new BreadcrumbItem { Text = "Listado de Cargos" }
            };

            return View(await _context.Positions.ToListAsync());
        }

        // GET: Positions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var positionEntity = await _context.Positions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (positionEntity == null)
            {
                return NotFound();
            }

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Administración", Url = "/Home/Administration" },
                new BreadcrumbItem { Text = "Listado de Cargos", Url = "/Position/Index"  },
                new BreadcrumbItem { Text = $"Detalle de  {positionEntity.Name }" }
            };

            return View(positionEntity);
        }

        // GET: Positions/Create
        public IActionResult Create()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Administración", Url = "/Home/Administration" },
                new BreadcrumbItem { Text = "Listado de Cargos", Url = "/Position/Index"  },
                new BreadcrumbItem { Text = "Nuevo Cargo" }
            };

            return View();
        }

        // POST: Positions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PositionEntity positionEntity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(positionEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(positionEntity);
        }

        // GET: Positions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var positionEntity = await _context.Positions.FindAsync(id);
            if (positionEntity == null)
            {
                return NotFound();
            }

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Administración", Url = "/Home/Administration" },
                new BreadcrumbItem { Text = "Listado de Cargos", Url = "/Position/Index"  },
                new BreadcrumbItem { Text = $"Editar  {positionEntity.Name }" }
            };

            return View(positionEntity);
        }

        // POST: Positions/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PositionEntity positionEntity)
        {
            if (id != positionEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(positionEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PositionEntityExists(positionEntity.Id))
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
            return View(positionEntity);
        }

        // GET: Positions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var positionEntity = await _context.Positions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (positionEntity == null)
            {
                return NotFound();
            }

            return View(positionEntity);
        }

        // POST: Positions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var positionEntity = await _context.Positions.FindAsync(id);
            if (positionEntity != null)
            {
                _context.Positions.Remove(positionEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PositionEntityExists(int id)
        {
            return _context.Positions.Any(e => e.Id == id);
        }
    }
}
