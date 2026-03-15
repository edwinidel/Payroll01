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
    public class DestajoDocumentsController : BaseController
    {
        public DestajoDocumentsController(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var redirect = EnsureCompanySelected();
            if (redirect != null) return redirect;

            var companyId = GetCurrentCompanyId();
            var query = FilterByCurrentCompany(_context.DestajoDocuments.Include(d => d.Productions), nameof(DestajoDocumentEntity.CompanyId));

            if (startDate.HasValue)
                query = query.Where(d => d.DocumentDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(d => d.DocumentDate <= endDate.Value);

            var documents = await query.OrderByDescending(d => d.DocumentDate).ToListAsync();
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            return View(documents);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var document = await FilterByCurrentCompany(_context.DestajoDocuments
                .Include(d => d.Productions)
                    .ThenInclude(p => p.Employee)
                .Include(d => d.Company), nameof(DestajoDocumentEntity.CompanyId))
                .FirstOrDefaultAsync(d => d.Id == id);

            if (document == null) return NotFound();

            var vm = new DestajoDocumentDetailsViewModel
            {
                Document = document,
                ExistingProductions = document.Productions.OrderByDescending(p => p.ProductionDate),
                NewProduction = new DestajoProductionEntity
                {
                    DocumentId = document.Id,
                    ProductionDate = document.DocumentDate,
                    UnitValue = 0
                }
            };

            await LoadEmployeesSelectList();
            return View(vm);
        }

        public async Task<IActionResult> Create()
        {
            var redirect = EnsureCompanySelected();
            if (redirect != null) return redirect;

            var companyId = GetCurrentCompanyId();
            var vm = new DestajoDocumentFormViewModel
            {
                DocumentDate = DateTime.Now,
                CompanyId = companyId ?? 0
            };

            vm.Lines = await BuildEmployeeLines(companyId!.Value, vm.DocumentDate);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DestajoDocumentFormViewModel vm)
        {
            var companyId = GetCurrentCompanyId();
            if (!companyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de crear documentos de destajo.";
                return RedirectToAction("SwitchCompany", "Home");
            }

            var selectedLines = vm.Lines.Where(l => l.Selected).ToList();
            if (!selectedLines.Any())
            {
                ModelState.AddModelError(string.Empty, "Debe seleccionar al menos un empleado destajo.");
            }

            if (vm.UnitsProduced <= 0)
            {
                ModelState.AddModelError(nameof(vm.UnitsProduced), "Debe ingresar unidades mayores a cero.");
            }

            if (ModelState.IsValid)
            {
                var document = new DestajoDocumentEntity
                {
                    CompanyId = companyId.Value,
                    DocumentDate = vm.DocumentDate,
                    ReferenceNumber = vm.ReferenceNumber ?? string.Empty,
                    ClientResponsible = vm.ClientResponsible ?? string.Empty,
                    CompanyResponsible = vm.CompanyResponsible ?? string.Empty,
                    DocumentPath = vm.DocumentPath,
                    Notes = vm.Notes ?? string.Empty,
                    Created = DateTime.UtcNow,
                    CreatedBy = User.Identity?.Name ?? string.Empty
                };

                await _context.Database.BeginTransactionAsync();
                try
                {
                    _context.Add(document);
                    await _context.SaveChangesAsync();

                    foreach (var line in selectedLines)
                    {
                        line.UnitsProduced = vm.UnitsProduced;
                    }

                    var productions = await BuildProductionsFromLines(document.Id, vm.DocumentDate, selectedLines);
                    if (productions.Any())
                    {
                        _context.DestajoProductions.AddRange(productions);
                        await _context.SaveChangesAsync();
                    }

                    await _context.Database.CommitTransactionAsync();
                    return RedirectToAction(nameof(Details), new { id = document.Id });
                }
                catch
                {
                    await _context.Database.RollbackTransactionAsync();
                    throw;
                }
            }

            vm.Lines = await BuildEmployeeLines(companyId.Value, vm.DocumentDate, null, vm.Lines);
            return View(vm);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var document = await FilterByCurrentCompany(_context.DestajoDocuments, nameof(DestajoDocumentEntity.CompanyId)).FirstOrDefaultAsync(d => d.Id == id);
            if (document == null) return NotFound();

            var productions = await _context.DestajoProductions.Where(p => p.DocumentId == document.Id).ToListAsync();
            var vm = new DestajoDocumentFormViewModel
            {
                Id = document.Id,
                CompanyId = document.CompanyId,
                DocumentDate = document.DocumentDate,
                ReferenceNumber = document.ReferenceNumber,
                ClientResponsible = document.ClientResponsible,
                CompanyResponsible = document.CompanyResponsible,
                DocumentPath = document.DocumentPath,
                Notes = document.Notes,
                UnitsProduced = productions.FirstOrDefault()?.UnitsProduced ?? 0
            };

            vm.Lines = await BuildEmployeeLines(document.CompanyId, document.DocumentDate, productions);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DestajoDocumentFormViewModel vm)
        {
            if (id != vm.Id) return NotFound();

            var document = await FilterByCurrentCompany(_context.DestajoDocuments, nameof(DestajoDocumentEntity.CompanyId)).FirstOrDefaultAsync(d => d.Id == id);
            if (document == null) return NotFound();

            var selectedLines = vm.Lines.Where(l => l.Selected).ToList();
            if (!selectedLines.Any())
            {
                ModelState.AddModelError(string.Empty, "Debe seleccionar al menos un empleado destajo.");
            }

            if (vm.UnitsProduced <= 0)
            {
                ModelState.AddModelError(nameof(vm.UnitsProduced), "Debe ingresar unidades mayores a cero.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    document.DocumentDate = vm.DocumentDate;
                    document.ReferenceNumber = vm.ReferenceNumber ?? string.Empty;
                    document.ClientResponsible = vm.ClientResponsible ?? string.Empty;
                    document.CompanyResponsible = vm.CompanyResponsible ?? string.Empty;
                    document.DocumentPath = vm.DocumentPath;
                    document.Notes = vm.Notes ?? string.Empty;
                    document.Modified = DateTime.UtcNow;
                    document.ModifiedBy = User.Identity?.Name ?? string.Empty;

                    await _context.Database.BeginTransactionAsync();

                    _context.Update(document);
                    await _context.SaveChangesAsync();

                    var existingProductions = await _context.DestajoProductions.Where(p => p.DocumentId == document.Id).ToListAsync();
                    if (existingProductions.Any())
                    {
                        _context.DestajoProductions.RemoveRange(existingProductions);
                        await _context.SaveChangesAsync();
                    }

                    foreach (var line in selectedLines)
                    {
                        line.UnitsProduced = vm.UnitsProduced;
                    }

                    var newProductions = await BuildProductionsFromLines(document.Id, vm.DocumentDate, selectedLines);
                    if (newProductions.Any())
                    {
                        _context.DestajoProductions.AddRange(newProductions);
                        await _context.SaveChangesAsync();
                    }

                    await _context.Database.CommitTransactionAsync();
                    return RedirectToAction(nameof(Details), new { id = document.Id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    await _context.Database.RollbackTransactionAsync();
                    if (!DocumentExists(document.Id)) return NotFound();
                    throw;
                }
                catch
                {
                    await _context.Database.RollbackTransactionAsync();
                    throw;
                }
            }

            vm.Lines = await BuildEmployeeLines(document.CompanyId, vm.DocumentDate, null, vm.Lines);
            return View(vm);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var document = await FilterByCurrentCompany(_context.DestajoDocuments, nameof(DestajoDocumentEntity.CompanyId))
                .FirstOrDefaultAsync(m => m.Id == id);
            if (document == null) return NotFound();

            return View(document);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var document = await FilterByCurrentCompany(_context.DestajoDocuments.Include(d => d.Productions), nameof(DestajoDocumentEntity.CompanyId))
                .FirstOrDefaultAsync(d => d.Id == id);
            if (document != null)
            {
                if (document.Productions.Any())
                {
                    TempData["Error"] = "No se puede eliminar un documento que tiene líneas de producción.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                _context.DestajoDocuments.Remove(document);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduction(DestajoProductionEntity production)
        {
            ModelState.Remove("Created");
            ModelState.Remove("CreatedBy");

            var document = await _context.DestajoDocuments.FirstOrDefaultAsync(d => d.Id == production.DocumentId);
            if (document == null) return NotFound();

            if (ModelState.IsValid)
            {
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == production.EmployeeId);
                if (employee != null && production.UnitValue <= 0)
                {
                    production.UnitValue = employee.UnitValue;
                }

                production.Created = DateTime.UtcNow;
                production.CreatedBy = User.Identity?.Name ?? string.Empty;
                production.TotalAmount = decimal.Round(production.UnitsProduced * production.UnitValue, 2, MidpointRounding.AwayFromZero);
                _context.DestajoProductions.Add(production);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = production.DocumentId });
            }

            await LoadEmployeesSelectList();
            var vm = new DestajoDocumentDetailsViewModel
            {
                Document = document,
                ExistingProductions = await _context.DestajoProductions.Include(p => p.Employee)
                    .Where(p => p.DocumentId == production.DocumentId)
                    .OrderByDescending(p => p.ProductionDate)
                    .ToListAsync(),
                NewProduction = production
            };
            return View("Details", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduction(int id)
        {
            var production = await _context.DestajoProductions.FirstOrDefaultAsync(p => p.Id == id);
            if (production == null) return NotFound();

            var documentId = production.DocumentId;
            _context.DestajoProductions.Remove(production);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = documentId });
        }

        private bool DocumentExists(int id)
        {
            return _context.DestajoDocuments.Any(e => e.Id == id);
        }

        private async Task LoadEmployeesSelectList()
        {
            var employees = await FilterByCurrentCompany(_context.Employees, nameof(EmployeeEntity.CompanyId))
                .Where(e => e.SalaryType == "Destajo")
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Select(e => new { e.Id, Name = e.FullNameWithCode })
                .ToListAsync();

            ViewBag.EmployeeId = new SelectList(employees, "Id", "Name");
        }

        private async Task<List<DestajoDocumentEmployeeLine>> BuildEmployeeLines(int companyId, DateTime documentDate, IEnumerable<DestajoProductionEntity>? existingProductions = null, IEnumerable<DestajoDocumentEmployeeLine>? postedLines = null)
        {
            var existingLookup = existingProductions?.ToDictionary(p => p.EmployeeId, p => p) ?? new Dictionary<int, DestajoProductionEntity>();
            var postedLookup = postedLines?.ToDictionary(p => p.EmployeeId, p => p) ?? new Dictionary<int, DestajoDocumentEmployeeLine>();

            var employees = await FilterByCurrentCompany(_context.Employees, nameof(EmployeeEntity.CompanyId))
                .Where(e => e.CompanyId == companyId && e.SalaryType == "Destajo")
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Select(e => new
                {
                    e.Id,
                    e.FullNameWithCode,
                    e.UnitValue,
                    e.IdDocument,
                    e.Code,
                    e.FirstName,
                    e.LastName
                })
                .ToListAsync();

            var lines = new List<DestajoDocumentEmployeeLine>();
            foreach (var emp in employees)
            {
                if (postedLookup.TryGetValue(emp.Id, out var posted))
                {
                    lines.Add(new DestajoDocumentEmployeeLine
                    {
                        EmployeeId = emp.Id,
                        EmployeeName = emp.FullNameWithCode,
                        FirstName = emp.FirstName,
                        LastName = emp.LastName,
                        Code = emp.Code,
                        DocumentId = emp.IdDocument,
                        Selected = posted.Selected,
                        UnitsProduced = posted.UnitsProduced,
                        UnitValue = emp.UnitValue
                    });
                    continue;
                }

                if (existingLookup.TryGetValue(emp.Id, out var existing))
                {
                    lines.Add(new DestajoDocumentEmployeeLine
                    {
                        EmployeeId = emp.Id,
                        EmployeeName = emp.FullNameWithCode,
                        FirstName = emp.FirstName,
                        LastName = emp.LastName,
                        Code = emp.Code,
                        DocumentId = emp.IdDocument,
                        Selected = true,
                        UnitsProduced = existing.UnitsProduced,
                        UnitValue = existing.UnitValue
                    });
                    continue;
                }

                lines.Add(new DestajoDocumentEmployeeLine
                {
                    EmployeeId = emp.Id,
                    EmployeeName = emp.FullNameWithCode,
                    FirstName = emp.FirstName,
                    LastName = emp.LastName,
                    Code = emp.Code,
                    DocumentId = emp.IdDocument,
                    Selected = false,
                    UnitsProduced = 0,
                    UnitValue = emp.UnitValue
                });
            }

            return lines;
        }

        private async Task<List<DestajoProductionEntity>> BuildProductionsFromLines(int documentId, DateTime productionDate, IEnumerable<DestajoDocumentEmployeeLine> lines)
        {
            var employeeIds = lines.Select(l => l.EmployeeId).ToList();
            var employeeValues = await _context.Employees
                .Where(e => employeeIds.Contains(e.Id))
                .Select(e => new { e.Id, e.UnitValue })
                .ToDictionaryAsync(e => e.Id, e => e.UnitValue);

            var result = new List<DestajoProductionEntity>();
            foreach (var line in lines)
            {
                var unitValue = employeeValues.TryGetValue(line.EmployeeId, out var u) ? u : line.UnitValue;
                var total = decimal.Round(line.UnitsProduced * unitValue, 2, MidpointRounding.AwayFromZero);
                result.Add(new DestajoProductionEntity
                {
                    DocumentId = documentId,
                    EmployeeId = line.EmployeeId,
                    ProductionDate = productionDate,
                    UnitsProduced = line.UnitsProduced,
                    UnitValue = unitValue,
                    TotalAmount = total,
                    Created = DateTime.UtcNow,
                    CreatedBy = User.Identity?.Name ?? string.Empty
                });
            }

            return result;
        }
    }
}
