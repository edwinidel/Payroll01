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
    public class EmployeeTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EmployeeTypes
        public async Task<IActionResult> Index()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Administración", Url = "/Home/Administration" },
                new BreadcrumbItem { Text = "Administración de Empleados", Url = "/Home/Employees"  },
                new BreadcrumbItem { Text = "Tipos de Empleados" }
            };
            return View(await _context.EmployeeTypes.ToListAsync());
        }

        // GET: EmployeeTypes/Details/5
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
                new BreadcrumbItem { Text = "Tipos de Empleados", Url = "/EmployeeTypes/Index"  },
                new BreadcrumbItem { Text = "Detalles de Tipos de Empleados" }
            };
            var employeeTypeEntity = await _context.EmployeeTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeeTypeEntity == null)
            {
                return NotFound();
            }

            return View(employeeTypeEntity);
        }

        // GET: EmployeeTypes/Create
        public IActionResult Create()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Administración", Url = "/Home/Administration" },
                new BreadcrumbItem { Text = "Administración de Empleados", Url = "/Home/Employees"  },
                new BreadcrumbItem { Text = "Tipos de Empleados", Url = "/EmployeeTypes/Index"  },
                new BreadcrumbItem { Text = "Nuevo Tipo de Empleado" }
            };

            return View();
        }

        // POST: EmployeeTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeTypeEntity employeeTypeEntity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employeeTypeEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employeeTypeEntity);
        }

        // GET: EmployeeTypes/Edit/5
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
                new BreadcrumbItem { Text = "Tipos de Empleados", Url = "/EmployeeTypes/Index"  },
                new BreadcrumbItem { Text = "Editar Tipo de Empleado" }
            };

            var employeeTypeEntity = await _context.EmployeeTypes.FindAsync(id);
            if (employeeTypeEntity == null)
            {
                return NotFound();
            }
            return View(employeeTypeEntity);
        }

        // POST: EmployeeTypes/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeTypeEntity employeeTypeEntity)
        {
            if (id != employeeTypeEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employeeTypeEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeTypeEntityExists(employeeTypeEntity.Id))
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
            return View(employeeTypeEntity);
        }

        // GET: EmployeeTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeTypeEntity = await _context.EmployeeTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeeTypeEntity == null)
            {
                return NotFound();
            }

            return View(employeeTypeEntity);
        }

        // POST: EmployeeTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employeeTypeEntity = await _context.EmployeeTypes.FindAsync(id);
            if (employeeTypeEntity != null)
            {
                _context.EmployeeTypes.Remove(employeeTypeEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeTypeEntityExists(int id)
        {
            return _context.EmployeeTypes.Any(e => e.Id == id);
        }
    }
}
