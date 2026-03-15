using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _2FA.Data;
using _2FA.Data.Entities;
using System.Runtime.CompilerServices;
using _2FA.Models;
using Microsoft.Identity.Client;

namespace _2FA.Controllers
{
    public class PayrollHeadersController : BaseController
    {
        public PayrollHeadersController(ApplicationDbContext context) : base(context)
        {
        }

        // GET: PayrollHeaders
        public async Task<IActionResult> Index()
        {
            var redirect = EnsureCompanySelected();
            if (redirect != null) return redirect;

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Administración", Url = "/Home/Administration" },
                new BreadcrumbItem { Text = "Listado de Planillas" }
            };

            var applicationDbContext = FilterByCurrentCompany(_context.PayrollHeaders.Include(p => p.PaymentGroup));
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PayrollHeaders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var redirect = EnsureCompanySelected();
            if (redirect != null) return redirect;

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Administración", Url = "/Home/Administration" },
                new BreadcrumbItem { Text = "Listado de Planillas", Url = "/PayrollTmpHeaders/Index"  },
                new BreadcrumbItem { Text = "Detalle de Planillas" }
            };

            if (id == null)
            {
                return NotFound();
            }

            var payrrollHeaderEntity = await FilterByCurrentCompany(_context.PayrollHeaders
                .Include(p => p.PaymentGroup))
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payrrollHeaderEntity == null)
            {
                return NotFound();
            }

            return View(payrrollHeaderEntity);
        }

        // GET: PayrollHeaders/Create
        public IActionResult Create()
        {
            var redirect = EnsureCompanySelected();
            if (redirect != null) return redirect;

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Administración", Url = "/Home/Administration" },
                new BreadcrumbItem { Text = "Listado de Planillas", Url = "/PayrollTmpHeaders/Index"  },
                new BreadcrumbItem { Text = "Nueva planilla" }
            };

            var payrollHeader = new PayrollHeaderViewModel
            {
                Status = "Draft",
                Created = DateTime.UtcNow
            };

            ViewData["PaymentGroupId"] = new SelectList(FilterByCurrentCompany(_context.PaymentGroups), "Id", "Name");
            return View(payrollHeader);
        }

        // POST: PayrollHeaders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PayrollHeaderEntity payrrollHeaderEntity)
        {
            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de continuar.";
                return RedirectToAction("SwitchCompany", "Home");
            }

            if (ModelState.IsValid)
            {
                payrrollHeaderEntity.Created = DateTime.UtcNow;
                payrrollHeaderEntity.CompanyId = currentCompanyId.Value;
                _context.Add(payrrollHeaderEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PaymentGroupId"] = new SelectList(FilterByCurrentCompany(_context.PaymentGroups), "Id", "Name", payrrollHeaderEntity.PaymentGroupId);

            return View(payrrollHeaderEntity);
        }

        // GET: PayrollHeaders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var redirect = EnsureCompanySelected();
            if (redirect != null) return redirect;

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Administración", Url = "/Home/Administration" },
                new BreadcrumbItem { Text = "Listado de Planillas", Url = "/PayrollTmpHeaders/Index"  },
                new BreadcrumbItem { Text = "Editar Planilla" }
            };

            if (id == null)
            {
                return NotFound();
            }

            var payrrollHeaderEntity = await FilterByCurrentCompany(_context.PayrollHeaders).FirstOrDefaultAsync(e => e.Id == id);
            if (payrrollHeaderEntity == null)
            {
                return NotFound();
            }

            ViewData["PaymentGroupId"] = new SelectList(FilterByCurrentCompany(_context.PaymentGroups), "Id", "Name", payrrollHeaderEntity.PaymentGroupId);
            return View(payrrollHeaderEntity);
        }

        // POST: PayrollHeaders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PayrollHeaderEntity payrrollHeaderEntity)
        {
            if (id != payrrollHeaderEntity.Id)
            {
                return NotFound();
            }

            // Verify the entity belongs to the current company
            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue || payrrollHeaderEntity.CompanyId != currentCompanyId.Value)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payrrollHeaderEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PayrrollHeaderEntityExists(payrrollHeaderEntity.Id))
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
            ViewData["PaymentGroupId"] = new SelectList(FilterByCurrentCompany(_context.PaymentGroups), "Id", "Name", payrrollHeaderEntity.PaymentGroupId);
            return View(payrrollHeaderEntity);
        }

        // GET: PayrollHeaders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var redirect = EnsureCompanySelected();
            if (redirect != null) return redirect;

            if (id == null)
            {
                return NotFound();
            }

            var payrrollHeaderEntity = await FilterByCurrentCompany(_context.PayrollHeaders
                .Include(p => p.PaymentGroup))
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payrrollHeaderEntity == null)
            {
                return NotFound();
            }

            return View(payrrollHeaderEntity);
        }

        // POST: PayrollHeaders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payrrollHeaderEntity = await FilterByCurrentCompany(_context.PayrollHeaders).FirstOrDefaultAsync(e => e.Id == id);
            if (payrrollHeaderEntity != null)
            {
                _context.PayrollHeaders.Remove(payrrollHeaderEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PayrrollHeaderEntityExists(int id)
        {
            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
                return false;

            return _context.PayrollHeaders.Any(e => e.Id == id && e.CompanyId == currentCompanyId.Value);
        }

        [HttpGet]
        public IActionResult GetEmployeesByGroupId(int groupId, bool isContractor = false)
        {
            if (groupId <= 0)
                return NotFound();

            if (_context == null)
                return StatusCode(500, "Database context is not available");

            // Lista de empleados del grupo (filter by isContractor parameter)
            var employees = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Branch)
                .Where(e => e.PaymentGroupId == groupId && e.IsContractor == isContractor)
                .Select(e => new
                {
                    id = e.Id,
                    code = e.Code,
                    idDocument = e.IdDocument,
                    fullName = e.FullName,
                    branchName = e.Branch != null ? e.Branch.Name : "",
                    departmentName = e.Department != null ? e.Department.Name : ""
                })
                .ToList();

            if (employees.Count == 0)
            {
                return Json(new { 
                    success = false, 
                    message = "No hay empleados asignados al grupo de pago seleccionado",
                    employees = new List<object>()
                });
            }

            // Datos del grupo de pago
            var paymentGroup = _context.PaymentGroups
                .Where(pg => pg.Id == groupId)
                .Select(pg => new
                {
                    id = pg.Id,
                    lastPayDate = pg.LastPayDate != null ? pg.LastPayDate.Value.ToString("yyyy-MM-dd") : "",
                    lastAbsensestDate = pg.LastAbsensestDate != null ? pg.LastAbsensestDate.Value.ToString("yyyy-MM-dd") : "",
                    extraTimeDate = pg.ExtraTimeDate != null ? pg.ExtraTimeDate.Value.ToString("yyyy-MM-dd") : "",
                    quantityOfDays = pg.QuantityOfDays
                })
                .FirstOrDefault();

            if (employees.Count == 0 || paymentGroup == null)
                return Json(new { 
                    success = false, 
                    message = "No se encontró información para este grupo de pago",
                    employees = new List<object>()
                });

            return new JsonResult(new
            {
                success = true,
                employees,
                paymentGroup
            });
        }



    }
}
