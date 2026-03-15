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
    public class CompaniesController(
        ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // GET: Companies
        public async Task<IActionResult> Index()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Configuración", Url = "/Home/Configurations" },
                new BreadcrumbItem { Text = "Listado de Compañías" }
            };

            var applicationDbContext = _context.Companies
                .Include(c => c.BusinessGroup)
                .Include(c => c.Country)
                    .ThenInclude(country => country!.States!)
                        .ThenInclude(state => state.Cities!);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Configuración", Url = "/Home/Configurations" },
                new BreadcrumbItem { Text = "Listado de Compañías", Url = "/Companies/Index" },
                new BreadcrumbItem { Text = "Detalle de Compañìa" }
            };

            if (id == null)
            {
                return NotFound();
            }

            var companyEntity = await _context.Companies
                .Include(c => c.BusinessGroup)
                .Include(c => c.Country)
                .ThenInclude(country => country!.States!)
                .ThenInclude(state => state.Cities!)
                .Include(c => c.PaymentBank)
                .Include(c => c.PayrollVoucherFormats!)
                .ThenInclude(v => v.PayrollType!)
                .Include(c => c.CompanyPayrollVouchers)
                .ThenInclude(cpv => cpv.PayrollVoucherFormat!)
                .ThenInclude(pvf => pvf.PayrollType!)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (companyEntity == null)
            {
                return NotFound();
            }

            var vouchersCompany = _context.PayrollVoucherFormats
                .Where(c => c.CompanyId == companyEntity.Id)
                .Include(c => c.PayrollType)
                .ToList();

            if (vouchersCompany != null && vouchersCompany.Count > 0)
            {
                companyEntity.PayrollVoucherFormats = vouchersCompany;
            }

            // Get employee statistics grouped by status
            var employeeStats = await _context.Employees
                .Where(e => e.CompanyId == id && !e.IsDeleted)
                .GroupBy(e => e.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            ViewData["EmployeeStats"] = employeeStats;

            return View(companyEntity);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Configuración", Url = "/Home/Configurations" },
                new BreadcrumbItem { Text = "Listado de Compañías", Url = "/Companies/Index" },
                new BreadcrumbItem { Text = "Nueva Compañìa" }
            };

            var company = new CompanyEntity
            {
                IsActive = true
            };

            ViewData["BusinessGroupId"] = new SelectList(_context.BusinessGroups, "Id", "Name");
            ViewData["CountriesId"] = new SelectList(_context.Countries, "Id", "Name_es");
            ViewData["StatesId"] = new SelectList(_context.States, "Id", "Name");
            ViewData["CitiesId"] = new SelectList(_context.Cities, "Id", "Name");

            return View(company);
        }

        // POST: Companies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyEntity companyEntity)
        {
            if (ModelState.IsValid)
            {
                // Validate MaxCompanies limit for the BusinessGroup
                var businessGroup = await _context.BusinessGroups.FindAsync(companyEntity.BusinessGroupId);
                if (businessGroup != null && businessGroup.MaxCompanies.HasValue)
                {
                    var activeCompaniesCount = await _context.Companies
                        .Where(c => c.BusinessGroupId == companyEntity.BusinessGroupId && c.IsActive && !c.IsDeleted)
                        .CountAsync();
                    if (activeCompaniesCount >= businessGroup.MaxCompanies.Value)
                    {
                        ModelState.AddModelError("", $"No se puede crear la compañía. El grupo de negocio '{businessGroup.Name}' ha alcanzado el límite máximo de {businessGroup.MaxCompanies.Value} compañías activas.");
                    }
                }

                if (ModelState.IsValid)
                {
                    companyEntity.Created = DateTime.Now;
                    companyEntity.CreatedBy = User.Identity.Name ?? string.Empty;
                    _context.Add(companyEntity);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["BusinessGroupId"] = new SelectList(_context.BusinessGroups, "Id", "Name", companyEntity.BusinessGroupId);
            ViewData["CountriesId"] = new SelectList(_context.Countries.OrderBy(c => c.Name_es), "Id", "Name_es", companyEntity.CountryId);
            ViewData["StatesId"] = new SelectList(_context.States.OrderBy(c => c.Name), "Id", "Name", companyEntity.StateId);
            ViewData["CitiesId"] = new SelectList(_context.Cities.OrderBy(c => c.Name), "Id", "Name", companyEntity.CityId);
            ViewData["PaymentBankId"] = new SelectList(_context.Banks, "Id", "Name", companyEntity.PaymentBankId);
            ViewData["PayrollTypes"] = _context.PayrollTypes.Where(pt => pt.IsActive).ToList();
            ViewData["VoucherFormats"] = _context.PayrollVoucherFormats.Where(v => v.CompanyId == companyEntity.Id && v.IsActive).ToList();
            return View(companyEntity);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Configuración", Url = "/Home/Configurations" },
                new BreadcrumbItem { Text = "Listado de Compañías", Url = "/Companies/Index" },
                new BreadcrumbItem { Text = "Editar Compañìa" }
            };

            if (id == null)
            {
                return NotFound();
            }

            var companyEntity = await _context.Companies
                .Include(c => c.PayrollVoucherFormats!)
                .ThenInclude(v => v.PayrollType)
                .Include(c => c.VoucherFormat)
                .Include(c => c.CompanyPayrollVouchers)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (companyEntity == null)
            {
                return NotFound();
            }

            ViewData["BusinessGroupId"] = new SelectList(_context.BusinessGroups, "Id", "Name", companyEntity.BusinessGroupId);
            ViewData["CountriesId"] = new SelectList(_context.Countries.OrderBy(c => c.Name_es), "Id", "Name_es", companyEntity.CountryId);
            ViewData["StatesId"] = new SelectList(_context.States.OrderBy(c => c.Name), "Id", "Name", companyEntity.StateId);
            ViewData["CitiesId"] = new SelectList(_context.Cities.OrderBy(c => c.Name), "Id", "Name", companyEntity.CityId);
            ViewData["PaymentBankId"] = new SelectList(_context.Banks, "Id", "Name", companyEntity.PaymentBankId);

            var payrollTypes = new List<PayrollTypeEntity>();

            foreach (var pt in _context.PayrollTypes.Where(pt => pt.IsActive))
            {
                payrollTypes.Add(pt);
            }

            ViewData["PayrollTypes"] = payrollTypes;

            var voucherFormats = new List<PayrollVoucherFormatEntity>();

            foreach (var vf in _context.PayrollVoucherFormats.Where(v => v.CompanyId == id && v.IsActive))
            {
                voucherFormats.Add(vf);
            }

            ViewData["VoucherFormats"] = voucherFormats;

            return View(companyEntity);
        }

        // POST: Companies/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CompanyEntity companyEntity, string[] VoucherFormats)
        {
            if (id != companyEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    companyEntity.Modified = DateTime.Now;
                    companyEntity.ModifiedBy = User.Identity.Name ?? string.Empty;

                    // Handle voucher format configurations
                    if (VoucherFormats != null && VoucherFormats.Length > 0)
                    {
                        // Remove existing voucher formats for this company
                        var existingFormats = _context.PayrollVoucherFormats.Where(v => v.CompanyId == id);
                        _context.PayrollVoucherFormats.RemoveRange(existingFormats);

                        // Add new voucher formats
                        foreach (var voucherFormatId in VoucherFormats)
                        {
                            if (int.TryParse(voucherFormatId, out int formatId))
                            {
                                var voucherFormat = await _context.PayrollVoucherFormats.FindAsync(formatId);
                                if (voucherFormat != null)
                                {
                                    // Create a new instance to avoid conflicts
                                    var newVoucherFormat = new PayrollVoucherFormatEntity
                                    {
                                        CompanyId = id,
                                        PayrollTypeId = voucherFormat.PayrollTypeId,
                                        Name = voucherFormat.Name,
                                        FormatTemplate = voucherFormat.FormatTemplate,
                                        IsActive = true,
                                        Created = DateTime.Now,
                                        CreatedBy = companyEntity.ModifiedBy
                                    };
                                    _context.PayrollVoucherFormats.Add(newVoucherFormat);
                                }
                            }
                        }
                    }

                    _context.Update(companyEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyEntityExists(companyEntity.Id))
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

            ViewData["BusinessGroupId"] = new SelectList(_context.BusinessGroups, "Id", "Name", companyEntity.BusinessGroupId);
            ViewData["CountriesId"] = new SelectList(_context.Countries.OrderBy(c => c.Name_es), "Id", "Name_es", companyEntity.CountryId);
            ViewData["StatesId"] = new SelectList(_context.States.OrderBy(c => c.Name), "Id", "Name", companyEntity.StateId);
            ViewData["CitiesId"] = new SelectList(_context.Cities.OrderBy(c => c.Name), "Id", "Name", companyEntity.CityId);
            ViewData["PaymentBankId"] = new SelectList(_context.Banks, "Id", "Name", companyEntity.PaymentBankId);

            var payrollTypes = new List<PayrollTypeEntity>();

            foreach (var pt in _context.PayrollTypes.Where(pt => pt.IsActive))
            {
                payrollTypes.Add(pt);
            }

            ViewData["PayrollTypes"] = payrollTypes;

            var voucherFormats = new List<PayrollVoucherFormatEntity>();

            foreach (var vf in _context.PayrollVoucherFormats.Where(v => v.CompanyId == companyEntity.Id && v.IsActive))
            {
                voucherFormats.Add(vf);
            }

            ViewData["VoucherFormats"] = voucherFormats;

            return View(companyEntity);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyEntity = await _context.Companies
                .Include(c => c.BusinessGroup)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (companyEntity == null)
            {
                return NotFound();
            }

            return View(companyEntity);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var companyEntity = await _context.Companies.FindAsync(id);
            if (companyEntity != null)
            {
                companyEntity.Deleted = DateTime.Now;
                companyEntity.DeletedBy = User.Identity.Name ?? string.Empty;
                companyEntity.IsDeleted = true;
                companyEntity.IsActive = false;

                _context.Companies.Remove(companyEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> SaveVoucherConfigurations(int companyId, List<string> configurations)
        {
            try
            {
                // Remove existing configurations for this company
                var existingConfigs = _context.CompanyPayrollVouchers.Where(cpv => cpv.CompanyId == companyId);
                _context.CompanyPayrollVouchers.RemoveRange(existingConfigs);

                // Add new configurations
                foreach (var config in configurations)
                {
                    var parts = config.Split(':');
                    if (parts.Length == 2 &&
                        int.TryParse(parts[0], out int payrollTypeId) &&
                        int.TryParse(parts[1], out int voucherFormatId))
                    {
                        var newConfig = new CompanyPayrollVoucherEntity
                        {
                            CompanyId = companyId,
                            PayrollTypeId = payrollTypeId,
                            PayrollVoucherFormatId = voucherFormatId,
                            IsActive = true,
                            Created = DateTime.Now,
                            CreatedBy = User.Identity.Name ?? "System"
                        };
                        _context.CompanyPayrollVouchers.Add(newConfig);
                    }
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Configuraciones guardadas exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al guardar configuraciones: {ex.Message}" });
            }
        }

        private bool CompanyEntityExists(int id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }
    }
}
