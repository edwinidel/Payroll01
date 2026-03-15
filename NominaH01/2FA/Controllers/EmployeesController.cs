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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace _2FA.Controllers
{
    public class EmployeesController : BaseController
    {
        private readonly IWebHostEnvironment _env;

        public EmployeesController(ApplicationDbContext context, IWebHostEnvironment env) : base(context)
        {
            _env = env;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Administración", Url = "/Home/Administration" },
                new BreadcrumbItem { Text = "Administración de Empleados", Url = "/Home/Employees"  },
                new BreadcrumbItem { Text = "Listado de Empleados" }
            };

            var applicationDbContext = FilterByCurrentCompany(_context.Employees
                .Include(e => e.Bank)
                .Include(e => e.Company)
                .Include(e => e.EmployeeObservation)
                .Include(e => e.EmployeeType)
                .Include(e => e.IdentityDocumentType)
                .Include(e => e.Position)
                .Include(e => e.Schedule)
                .Include(e => e.TypeOfWorker));

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Employees/Details/5
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

            var employeeEntity = await _context.Employees
                .Include(e => e.Bank)
                .Include(e => e.Company)
                .Include(e => e.Branch)
                .Include(e => e.Department)
                .Include(e => e.Section)
                .Include(e => e.CostCenter)
                .Include(e => e.Division)
                .Include(e => e.Project)
                .Include(e => e.Phase)
                .Include(e => e.Activity)
                .Include(e => e.EmployeeObservation)
                .Include(e => e.EmployeeType)
                .Include(e => e.IdentityDocumentType)
                .Include(e => e.Position)
                .Include(e => e.Schedule)
                .Include(e => e.TypeOfWorker)
                .Include(e => e.PieceworkUnitType)
                .Include(e => e.OriginCountry)
                .Include(e => e.PaymentGroup)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeeEntity == null)
            {
                return NotFound();
            }

            // Verify employee belongs to current company
            var currentCompanyId = GetCurrentCompanyId();
            if (employeeEntity.CompanyId != currentCompanyId)
            {
                TempData["Error"] = "No tiene permisos para ver este empleado.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Administración", Url = "/Home/Administration" },
                new BreadcrumbItem { Text = "Administración de Empleados", Url = "/Home/Employees"  },
                new BreadcrumbItem { Text = "Listado de Empleados", Url = "/Employees/Index" },
                new BreadcrumbItem { Text = $"Detalle de {employeeEntity.FullName }"  }
            };

            return View(employeeEntity);
        }

        // GET: Employees/Create
        public async Task<IActionResult> Create()
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Administración", Url = "/Home/Administration" },
                new BreadcrumbItem { Text = "Administración de Empleados", Url = "/Home/Employees"  },
                new BreadcrumbItem { Text = "Listado de Empleados", Url = "/Employees/Index" },
                new BreadcrumbItem { Text = "Nuevo Empleado" }
            };

            await PopulateDropdownsAsync();
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeEntity employeeEntity, IFormFile? photoFile)
        {
            // Ensure company is selected and set it
            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de crear un empleado.";
                return RedirectToAction("SwitchCompany", "Home");
            }
            employeeEntity.CompanyId = currentCompanyId.Value;

            // Validate BusinessGroup MaxEmployees limit (defensive: consider active/not deleted entities)
            var currentCompany = await _context.Companies
                .Include(c => c.BusinessGroup)
                .FirstOrDefaultAsync(c => c.Id == currentCompanyId.Value);

            var isDestajo = NormalizeSalaryType(employeeEntity);
            await SyncPieceworkUnitTypeAsync(employeeEntity, isDestajo);

            // Preserve or set photo path
            var photoPath = await SavePhotoAsync(photoFile, employeeEntity.PhotoPath);
            if (!string.IsNullOrEmpty(photoPath))
            {
                employeeEntity.PhotoPath = photoPath;
            }

            if (currentCompany != null)
            {
                // First try the navigation property (works when EF populated it in tests)
                int? maxEmployees = currentCompany.BusinessGroup?.MaxEmployees;
                var groupId = currentCompany.BusinessGroupId;

                // If navigation didn't contain the value, try to resolve from DB
                if (!maxEmployees.HasValue && groupId == 0)
                {
                    groupId = await _context.Companies
                        .Where(c => c.Id == currentCompany.Id)
                        .Select(c => c.BusinessGroupId)
                        .FirstOrDefaultAsync();
                }

                if (!maxEmployees.HasValue && groupId != 0)
                {
                    maxEmployees = await _context.BusinessGroups
                        .Where(bg => bg.Id == groupId)
                        .Select(bg => bg.MaxEmployees)
                        .FirstOrDefaultAsync();
                }

                if (maxEmployees.HasValue && groupId != 0)
                {
                    var companiesInGroup = await _context.Companies
                        .Where(c => c.BusinessGroupId == groupId && c.IsActive && !c.IsDeleted)
                        .Select(c => c.Id)
                        .ToListAsync();

                    var activeEmployeeCount = await _context.Employees
                        .Where(e => companiesInGroup.Contains(e.CompanyId) && !e.IsDeleted &&
                                    !string.Equals(e.Status, "cesante", StringComparison.OrdinalIgnoreCase))
                        .CountAsync();

                    if (activeEmployeeCount >= maxEmployees.Value)
                    {
                        TempData["Error"] = $"No se puede crear el empleado. El grupo de negocio ha alcanzado el límite máximo de empleados ({maxEmployees.Value}).";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            // Final check before saving: ensure group limits haven't been exceeded
            {
                var companyGroupId = await _context.Companies
                    .Where(c => c.Id == employeeEntity.CompanyId)
                    .Select(c => c.BusinessGroupId)
                    .FirstOrDefaultAsync();

                if (companyGroupId != 0)
                {
                    var maxEmployeesFinal = await _context.BusinessGroups
                        .Where(bg => bg.Id == companyGroupId)
                        .Select(bg => bg.MaxEmployees)
                        .FirstOrDefaultAsync();

                    if (maxEmployeesFinal.HasValue)
                    {
                        var companiesInGroupFinal = await _context.Companies
                            .Where(c => c.BusinessGroupId == companyGroupId && c.IsActive && !c.IsDeleted)
                            .Select(c => c.Id)
                            .ToListAsync();

                        var activeEmployeeCountFinal = await _context.Employees
                            .Where(e => companiesInGroupFinal.Contains(e.CompanyId) && !e.IsDeleted &&
                                        !string.Equals(e.Status, "cesante", StringComparison.OrdinalIgnoreCase))
                            .CountAsync();

                        if (activeEmployeeCountFinal >= maxEmployeesFinal.Value)
                        {
                            TempData["Error"] = $"No se puede crear el empleado. El grupo de negocio ha alcanzado el límite máximo de empleados ({maxEmployeesFinal.Value}).";
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(employeeEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Filter company-specific dropdowns by current company
            await PopulateDropdownsAsync(employeeEntity);
            return View(employeeEntity);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Administración", Url = "/Home/Administration" },
                new BreadcrumbItem { Text = "Administración de Empleados", Url = "/Home/Employees"  },
                new BreadcrumbItem { Text = "Listado de Empleados", Url = "/Employees/Index" },
                new BreadcrumbItem { Text = "Editar Empleado" }
            };

            if (id == null)
            {
                return NotFound();
            }

            var employeeEntity = await _context.Employees.FindAsync(id);
            if (employeeEntity == null)
            {
                return NotFound();
            }

            // Verify employee belongs to current company
            var currentCompanyId = GetCurrentCompanyId();
            if (employeeEntity.CompanyId != currentCompanyId)
            {
                TempData["Error"] = "No tiene permisos para editar este empleado.";
                return RedirectToAction(nameof(Index));
            }

            // Filter company-specific dropdowns by current company
            await PopulateDropdownsAsync(employeeEntity);
            return View(employeeEntity);
        }

        // POST: Employees/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeEntity employeeEntity, IFormFile? photoFile)
        {
            if (id != employeeEntity.Id)
            {
                return NotFound();
            }

            // Ensure company is selected and set it
            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de editar un empleado.";
                return RedirectToAction("SwitchCompany", "Home");
            }
            employeeEntity.CompanyId = currentCompanyId.Value;

            var existingEmployee = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if (existingEmployee == null)
            {
                return NotFound();
            }

            var isDestajo = NormalizeSalaryType(employeeEntity);
            await SyncPieceworkUnitTypeAsync(employeeEntity, isDestajo);

            if (isDestajo && !employeeEntity.PieceworkUnitTypeId.HasValue)
            {
                ModelState.AddModelError("PieceworkUnitTypeId", "El tipo de unidad es obligatorio para empleados a destajo.");
            }

            if (!isDestajo)
            {
                ModelState.Remove(nameof(employeeEntity.PieceworkUnitTypeId));
            }

            ModelState.Remove(nameof(employeeEntity.UnitType));
            ModelState.Remove(nameof(employeeEntity.UnitValue));

            var photoPath = await SavePhotoAsync(photoFile, existingEmployee.PhotoPath);
            if (!string.IsNullOrEmpty(photoPath))
            {
                employeeEntity.PhotoPath = photoPath;
            }
            else
            {
                employeeEntity.PhotoPath = existingEmployee.PhotoPath;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employeeEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeEntityExists(employeeEntity.Id))
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

            // Filter company-specific dropdowns by current company
            ViewData["BranchId"] = new SelectList(FilterByCurrentCompany(_context.Branches), "Id", "Name", employeeEntity.BranchId);
            ViewData["CostCenterId"] = new SelectList(FilterByCurrentCompany(_context.CostCenters), "Id", "Name", employeeEntity.CostCenterId);
            ViewData["DepartmentId"] = new SelectList(FilterByCurrentCompany(_context.Departments), "Id", "Name", employeeEntity.DepartmentId);
            ViewData["SectionId"] = new SelectList(FilterByCurrentCompany(_context.Sections), "Id", "Name", employeeEntity.SectionId);
            ViewData["DivisionId"] = new SelectList(FilterByCurrentCompany(_context.Divisions), "Id", "Name", employeeEntity.DivisionId);
            ViewData["ProjectId"] = new SelectList(FilterByCurrentCompany(_context.Projects), "Id", "Name", employeeEntity.ProjectId);
            ViewData["PhaseId"] = new SelectList(FilterByCurrentCompany(_context.Phases), "Id", "Name", employeeEntity.PhaseId);
            ViewData["ActivityId"] = new SelectList(_context.Activities, "Id", "Name", employeeEntity.ActivityId);

            // Show only current company
            var currentCompany = await GetCurrentCompanyAsync();
            ViewData["CompanyId"] = new SelectList(new[] { currentCompany }, "Id", "Name", employeeEntity.CompanyId);

            ViewData["OriginCountryId"] = new SelectList(_context.Countries, "Id", "Name_es", employeeEntity.OriginCountryId);
            ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Name", employeeEntity.BankId);
            ViewData["PayingBankId"] = new SelectList(_context.Banks, "Id", "Name", employeeEntity.PayingBankId);
            ViewData["EmployeeObservationId"] = new SelectList(_context.EmployeeObservations, "Id", "Name", employeeEntity.EmployeeObservationId);
            ViewData["EmployeeTypeId"] = new SelectList(_context.EmployeeTypes, "Id", "Name", employeeEntity.EmployeeTypeId);
            ViewData["IdentityDocumentTypeId"] = new SelectList(_context.IdentityDocumentTypes, "Id", "Name", employeeEntity.IdentityDocumentTypeId);
            ViewData["PositionId"] = new SelectList(_context.Positions, "Id", "Name", employeeEntity.PositionId);
            ViewData["ScheduleId"] = new SelectList(_context.Schedules, "Id", "Name", employeeEntity.ScheduleId);
            ViewData["TypeOfWorkerId"] = new SelectList(_context.TypeOfWorkers, "Id", "Name", employeeEntity.TypeOfWorkerId);
            ViewData["PaymentGroupId"] = new SelectList(_context.PaymentGroups, "Id", "Name", employeeEntity.PaymentGroupId);
            return View(employeeEntity);
        }

        // GET: Employees/Delete/5
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

            var employeeEntity = await _context.Employees
                .Include(e => e.Bank)
                .Include(e => e.Company)
                .Include(e => e.EmployeeObservation)
                .Include(e => e.EmployeeType)
                .Include(e => e.IdentityDocumentType)
                .Include(e => e.Position)
                .Include(e => e.Schedule)
                .Include(e => e.TypeOfWorker)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeeEntity == null)
            {
                return NotFound();
            }

            // Verify employee belongs to current company
            var currentCompanyId = GetCurrentCompanyId();
            if (employeeEntity.CompanyId != currentCompanyId)
            {
                TempData["Error"] = "No tiene permisos para eliminar este empleado.";
                return RedirectToAction(nameof(Index));
            }

            return View(employeeEntity);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employeeEntity = await _context.Employees.FindAsync(id);
            if (employeeEntity != null)
            {
                // Verify employee belongs to current company
                var currentCompanyId = GetCurrentCompanyId();
                if (employeeEntity.CompanyId != currentCompanyId)
                {
                    TempData["Error"] = "No tiene permisos para eliminar este empleado.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Employees.Remove(employeeEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeEntityExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }

        private static bool NormalizeSalaryType(EmployeeEntity employeeEntity)
        {
            var salaryType = (employeeEntity.SalaryType ?? string.Empty).Trim();
            employeeEntity.SalaryType = salaryType;
            return string.Equals(salaryType, "Destajo", StringComparison.OrdinalIgnoreCase);
        }

        private async Task SyncPieceworkUnitTypeAsync(EmployeeEntity employeeEntity, bool isDestajo)
        {
            if (isDestajo && employeeEntity.PieceworkUnitTypeId.HasValue)
            {
                var unitName = await FilterByCurrentCompany(_context.PieceworkUnitTypes)
                    .Where(p => p.Id == employeeEntity.PieceworkUnitTypeId.Value)
                    .Select(p => p.Name)
                    .FirstOrDefaultAsync();

                employeeEntity.UnitType = unitName ?? string.Empty;
            }
            else
            {
                var noAplicaId = await _context.PieceworkUnitTypes
                    .Where(p => p.CompanyId == employeeEntity.CompanyId && p.Name == "No Aplica")
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync();

                employeeEntity.PieceworkUnitTypeId = noAplicaId == 0 ? null : noAplicaId;
                employeeEntity.UnitType = noAplicaId == 0 ? string.Empty : "No Aplica";
            }
        }

        private async Task PopulateDropdownsAsync(EmployeeEntity? employeeEntity = null)
        {
            var selectedBranch = employeeEntity?.BranchId;
            var selectedCostCenter = employeeEntity?.CostCenterId;
            var selectedDepartment = employeeEntity?.DepartmentId;
            var selectedSection = employeeEntity?.SectionId;
            var selectedDivision = employeeEntity?.DivisionId;
            var selectedProject = employeeEntity?.ProjectId;
            var selectedPhase = employeeEntity?.PhaseId;
            var selectedPieceworkUnitType = employeeEntity?.PieceworkUnitTypeId;
            var selectedActivity = employeeEntity?.ActivityId;

            ViewData["BranchId"] = await BuildCompanySelectListAsync(_context.Branches, "Id", "Name", selectedBranch);
            ViewData["CostCenterId"] = await BuildCompanySelectListAsync(_context.CostCenters, "Id", "Name", selectedCostCenter);
            ViewData["DepartmentId"] = await BuildCompanySelectListAsync(_context.Departments, "Id", "Name", selectedDepartment);
            ViewData["SectionId"] = await BuildCompanySelectListAsync(_context.Sections, "Id", "Name", selectedSection);
            ViewData["DivisionId"] = await BuildCompanySelectListAsync(_context.Divisions, "Id", "Name", selectedDivision);
            ViewData["ProjectId"] = await BuildCompanySelectListAsync(_context.Projects, "Id", "Name", selectedProject);
            ViewData["PhaseId"] = await BuildCompanySelectListAsync(_context.Phases, "Id", "Name", selectedPhase);
            ViewData["PieceworkUnitTypeId"] = await BuildCompanySelectListAsync(_context.PieceworkUnitTypes, "Id", "Name", selectedPieceworkUnitType);

            ViewData["ActivityId"] = new SelectList(await _context.Activities.ToListAsync(), "Id", "Name", selectedActivity);
            ViewData["BankId"] = new SelectList(await _context.Banks.ToListAsync(), "Id", "Name", employeeEntity?.BankId);
            ViewData["PayingBankId"] = new SelectList(await _context.Banks.ToListAsync(), "Id", "Name", employeeEntity?.PayingBankId);
            ViewData["EmployeeObservationId"] = new SelectList(await _context.EmployeeObservations.ToListAsync(), "Id", "Name", employeeEntity?.EmployeeObservationId);
            ViewData["EmployeeTypeId"] = new SelectList(await _context.EmployeeTypes.ToListAsync(), "Id", "Name", employeeEntity?.EmployeeTypeId);
            ViewData["IdentityDocumentTypeId"] = new SelectList(await _context.IdentityDocumentTypes.ToListAsync(), "Id", "Name", employeeEntity?.IdentityDocumentTypeId);
            ViewData["PositionId"] = new SelectList(await _context.Positions.ToListAsync(), "Id", "Name", employeeEntity?.PositionId);
            ViewData["ScheduleId"] = new SelectList(await _context.Schedules.ToListAsync(), "Id", "Name", employeeEntity?.ScheduleId);
            ViewData["TypeOfWorkerId"] = new SelectList(await _context.TypeOfWorkers.ToListAsync(), "Id", "Name", employeeEntity?.TypeOfWorkerId);
            ViewData["OriginCountryId"] = new SelectList(await _context.Countries.ToListAsync(), "Id", "Name_es", employeeEntity?.OriginCountryId);
            ViewData["PaymentGroupId"] = new SelectList(await _context.PaymentGroups.ToListAsync(), "Id", "Name", employeeEntity?.PaymentGroupId);

            var currentCompany = await GetCurrentCompanyAsync();
            if (currentCompany != null)
            {
                ViewData["CompanyId"] = new SelectList(new[] { currentCompany }, "Id", "Name", employeeEntity?.CompanyId ?? currentCompany.Id);
            }
        }

        private async Task<SelectList> BuildCompanySelectListAsync<T>(IQueryable<T> query, string valueField, string textField, object? selected = null) where T : class
        {
            var filtered = FilterByCurrentCompany(query);
            var items = await filtered.ToListAsync();

            if (!items.Any())
            {
                items = await query.ToListAsync();
            }

            return new SelectList(items, valueField, textField, selected);
        }

        private async Task<string?> SavePhotoAsync(IFormFile? photoFile, string? existingPath)
        {
            if (photoFile == null || photoFile.Length == 0)
                return existingPath;

            var uploadsRoot = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "employees");
            if (!Directory.Exists(uploadsRoot))
            {
                Directory.CreateDirectory(uploadsRoot);
            }

            var fileName = $"emp_{Guid.NewGuid():N}{Path.GetExtension(photoFile.FileName)}";
            var filePath = Path.Combine(uploadsRoot, fileName);

            await using (var stream = System.IO.File.Create(filePath))
            {
                await photoFile.CopyToAsync(stream);
            }

            return Path.Combine("uploads", "employees", fileName).Replace("\\", "/");
        }
    }
}
