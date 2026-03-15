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
    public class IdentityDocumentTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IdentityDocumentTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: IdentityDocumentTypes
        public async Task<IActionResult> Index()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Administración", Url = "/Home/Administration" },
                new BreadcrumbItem { Text = "Administración de Empleados", Url = "/Home/Employees"  },
                new BreadcrumbItem { Text = "Tipos de Documentos de Identidad" }
            };
            return View(await _context.IdentityDocumentTypes.ToListAsync());
        }

        // GET: IdentityDocumentTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Administración", Url = "/Home/Administration" },
                new BreadcrumbItem { Text = "Administración de Empleados", Url = "/Home/Employees"  },
                new BreadcrumbItem { Text = "Tipos de Documentos de Identidad", Url = "/IdentityDocumentTypes/Index"  },
                new BreadcrumbItem { Text = "Detalle del Tipo de Documento de Identidad" }
            };

            var identityDocumentTypeEntity = await _context.IdentityDocumentTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (identityDocumentTypeEntity == null)
            {
                return NotFound();
            }

            return View(identityDocumentTypeEntity);
        }

        // GET: IdentityDocumentTypes/Create
        public IActionResult Create()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Administración", Url = "/Home/Administration" },
                new BreadcrumbItem { Text = "Administración de Empleados", Url = "/Home/Employees"  },
                new BreadcrumbItem { Text = "Tipos de Documentos de Identidad", Url = "/IdentityDocumentTypes/Index"  },
                new BreadcrumbItem { Text = "Nuevo Tipo de Documento de Identidad" }
            };

            return View();
        }

        // POST: IdentityDocumentTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdentityDocumentTypeEntity identityDocumentTypeEntity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(identityDocumentTypeEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(identityDocumentTypeEntity);
        }

        // GET: IdentityDocumentTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Administración", Url = "/Home/Administration" },
                new BreadcrumbItem { Text = "Administración de Empleados", Url = "/Home/Employees"  },
                new BreadcrumbItem { Text = "Tipos de Documentos de Identidad", Url = "/IdentityDocumentTypes/Index"  },
                new BreadcrumbItem { Text = "Editar Tipo de Documento de Identidad" }
            };

            var identityDocumentTypeEntity = await _context.IdentityDocumentTypes.FindAsync(id);
            if (identityDocumentTypeEntity == null)
            {
                return NotFound();
            }
            return View(identityDocumentTypeEntity);
        }

        // POST: IdentityDocumentTypes/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IdentityDocumentTypeEntity identityDocumentTypeEntity)
        {
            if (id != identityDocumentTypeEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(identityDocumentTypeEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IdentityDocumentTypeEntityExists(identityDocumentTypeEntity.Id))
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
            return View(identityDocumentTypeEntity);
        }

        // GET: IdentityDocumentTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var identityDocumentTypeEntity = await _context.IdentityDocumentTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (identityDocumentTypeEntity == null)
            {
                return NotFound();
            }

            return View(identityDocumentTypeEntity);
        }

        // POST: IdentityDocumentTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var identityDocumentTypeEntity = await _context.IdentityDocumentTypes.FindAsync(id);
            if (identityDocumentTypeEntity != null)
            {
                _context.IdentityDocumentTypes.Remove(identityDocumentTypeEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IdentityDocumentTypeEntityExists(int id)
        {
            return _context.IdentityDocumentTypes.Any(e => e.Id == id);
        }
    }
}
