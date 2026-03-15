using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _2FA.Data;
using _2FA.Data.Entities;
using _2FA.Models;

namespace _2FA.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class EmployeeTransferController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmployeeTransferController> _logger;

        public EmployeeTransferController(
            ApplicationDbContext context,
            ILogger<EmployeeTransferController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: EmployeeTransfer
        public async Task<IActionResult> Index()
        {
            var viewModel = new EmployeeTransferViewModel
            {
                SourceCompanies = await GetCompaniesByBusinessGroupAsync(),
                TargetCompanies = await GetCompaniesByBusinessGroupAsync()
            };

            return View(viewModel);
        }

        // GET: EmployeeTransfer/LoadEmployees/5
        [HttpGet]
        public async Task<IActionResult> LoadEmployees(int companyId)
        {
            if (companyId <= 0)
            {
                return Json(new { success = false, message = "ID de compañía inválido" });
            }

            var employees = await _context.Employees
                .Include(e => e.Company)
                .Include(e => e.Position)
                .Include(e => e.Department)
                .Where(e => e.CompanyId == companyId && e.Status != "INACTIVO" &&
                    !_context.PayrollTmpEmployees.Any(pte => pte.EmployeeId == e.Id &&
                        pte.PayrollTmpHeader != null && pte.PayrollTmpHeader.Status != "Approved"))
                .Select(e => new EmployeeSummaryViewModel
                {
                    Id = e.Id,
                    Code = e.Code,
                    FullName = e.FirstName + " " + e.LastName,
                    CurrentPosition = e.Position != null ? e.Position.Name : "Sin Cargo",
                    CurrentDepartment = e.Department != null ? e.Department.Name : "Sin Departamento",
                    HiringDate = e.HiringDate,
                    Status = e.Status
                })
                .OrderBy(e => e.FullName)
                .ToListAsync();

            return Json(new { success = true, employees });
        }

        // GET: EmployeeTransfer/LoadCompaniesInGroup/5
        [HttpGet]
        public async Task<IActionResult> LoadCompaniesInGroup(int businessGroupId)
        {
            if (businessGroupId <= 0)
            {
                return Json(new { success = false, message = "ID de grupo de negocio inválido" });
            }

            var companies = await _context.Companies
                .Where(c => c.BusinessGroupId == businessGroupId && c.IsActive)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .OrderBy(c => c.Text)
                .ToListAsync();

            return Json(new { success = true, companies });
        }

        // POST: EmployeeTransfer/TransferEmployees
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TransferEmployees(EmployeeTransferViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Por favor complete todos los campos requeridos.";
                    return RedirectToAction("Index");
                }

                if (model.SelectedEmployeeIds == null || !model.SelectedEmployeeIds.Any())
                {
                    TempData["Error"] = "Debe seleccionar al menos un empleado para transferir.";
                    return RedirectToAction("Index");
                }

                if (model.SourceCompanyId == model.TargetCompanyId)
                {
                    TempData["Error"] = "La compañía de origen y destino deben ser diferentes.";
                    return RedirectToAction("Index");
                }

                // Validate that both companies belong to the same business group
                var sourceCompany = await _context.Companies
                    .Include(c => c.BusinessGroup)
                    .FirstOrDefaultAsync(c => c.Id == model.SourceCompanyId);

                var targetCompany = await _context.Companies
                    .Include(c => c.BusinessGroup)
                    .FirstOrDefaultAsync(c => c.Id == model.TargetCompanyId);

                if (sourceCompany == null || targetCompany == null)
                {
                    TempData["Error"] = "Una o ambas compañías no fueron encontradas.";
                    return RedirectToAction("Index");
                }

                if (sourceCompany.BusinessGroupId != targetCompany.BusinessGroupId)
                {
                    TempData["Error"] = "Las compañías deben pertenecer al mismo grupo de negocio.";
                    return RedirectToAction("Index");
                }

                // Perform the transfer
                using var transaction = await _context.Database.BeginTransactionAsync();
                
                try
                {
                    var employeesToTransfer = await _context.Employees
                        .Where(e => model.SelectedEmployeeIds.Contains(e.Id) && e.CompanyId == model.SourceCompanyId)
                        .ToListAsync();

                    if (!employeesToTransfer.Any())
                    {
                        TempData["Error"] = "No se encontraron empleados válidos para transferir.";
                        return RedirectToAction("Index");
                    }

                    var transferredCount = 0;
                    foreach (var employee in employeesToTransfer)
                    {
                        employee.CompanyId = model.TargetCompanyId;
                        employee.Modified = DateTime.Now;
                        employee.ModifiedBy = User.Identity?.Name ?? "System";
                        transferredCount++;
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var sourceName = sourceCompany.Name;
                    var targetName = targetCompany.Name;
                    TempData["Success"] = $"Se transfirieron exitosamente {transferredCount} empleados de {sourceName} a {targetName}.";
                    
                    _logger.LogInformation("Empleados transferidos exitosamente: {Count} empleados de {SourceCompany} a {TargetCompany} por usuario {User}", 
                        transferredCount, sourceName, targetName, User.Identity?.Name);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error durante la transferencia de empleados");
                    TempData["Error"] = "Ocurrió un error durante la transferencia. Por favor inténtelo de nuevo.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en TransferEmployees");
                TempData["Error"] = "Ocurrió un error inesperado. Por favor inténtelo de nuevo.";
            }

            return RedirectToAction("Index");
        }

        // GET: EmployeeTransfer/ConfirmTransfer
        [HttpGet]
        public async Task<IActionResult> ConfirmTransfer(EmployeeTransferViewModel model)
        {
            if (model.SelectedEmployeeIds == null || !model.SelectedEmployeeIds.Any())
            {
                TempData["Error"] = "Debe seleccionar empleados para la transferencia.";
                return RedirectToAction("Index");
            }

            var sourceCompany = await _context.Companies
                .Include(c => c.BusinessGroup)
                .FirstOrDefaultAsync(c => c.Id == model.SourceCompanyId);

            var targetCompany = await _context.Companies
                .Include(c => c.BusinessGroup)
                .FirstOrDefaultAsync(c => c.Id == model.TargetCompanyId);

            if (sourceCompany == null || targetCompany == null)
            {
                TempData["Error"] = "Una o ambas compañías no fueron encontradas.";
                return RedirectToAction("Index");
            }

            // Load employee details
            var employees = await _context.Employees
                .Include(e => e.Position)
                .Include(e => e.Department)
                .Where(e => model.SelectedEmployeeIds.Contains(e.Id))
                .Select(e => new EmployeeSummaryViewModel
                {
                    Id = e.Id,
                    Code = e.Code,
                    FullName = e.FirstName + " " + e.LastName,
                    CurrentPosition = e.Position != null ? e.Position.Name : "Sin Cargo",
                    CurrentDepartment = e.Department != null ? e.Department.Name : "Sin Departamento",
                    HiringDate = e.HiringDate,
                    Status = e.Status
                })
                .OrderBy(e => e.FullName)
                .ToListAsync();

            var viewModel = new EmployeeTransferViewModel
            {
                SelectedEmployeeIds = model.SelectedEmployeeIds,
                SourceCompanyId = model.SourceCompanyId,
                TargetCompanyId = model.TargetCompanyId,
                Comments = model.Comments,
                Employees = employees,
                SourceCompanyName = sourceCompany.Name,
                TargetCompanyName = targetCompany.Name,
                BusinessGroupName = sourceCompany.BusinessGroup?.Name ?? "N/A"
            };

            return View(viewModel);
        }

        private async Task<List<SelectListItem>> GetCompaniesByBusinessGroupAsync()
        {
            // Get all companies grouped by business group name
            var companiesWithGroups = await _context.Companies
                .Include(c => c.BusinessGroup)
                .Where(c => c.IsActive && c.BusinessGroup != null)
                .OrderBy(c => c.BusinessGroup.Name)
                .ThenBy(c => c.Name)
                .ToListAsync();

            return companiesWithGroups.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"{c.BusinessGroup!.Name} - {c.Name}"
            }).ToList();
        }
    }
}