using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _2FA.Data;
using _2FA.Models;
using _2FA.Services;

namespace _2FA.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class InitialEmployeeImportsController : BaseController
    {
        private const string PreviewSessionKey = "InitialEmployeeImports.PreviewRows";

        private readonly InitialEmployeeImportService _importService;

        public InitialEmployeeImportsController(ApplicationDbContext context, InitialEmployeeImportService importService) : base(context)
        {
            _importService = importService;
        }

        public async Task<IActionResult> Index()
        {
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
            {
                return redirectResult;
            }

            return View(await BuildViewModelAsync(GetPreviewRowsFromSession()));
        }

        [HttpGet]
        public async Task<IActionResult> DownloadTemplate()
        {
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
            {
                return redirectResult;
            }

            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de descargar la plantilla.";
                return RedirectToAction(nameof(Index));
            }

            var company = await GetCurrentCompanyAsync();
            var bytes = await _importService.BuildTemplateAsync(currentCompanyId.Value);
            var fileName = $"plantilla-empleados-iniciales-{(company?.Name ?? "empresa").Replace(" ", "-").ToLowerInvariant()}.xlsx";
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Preview(IFormFile excelFile)
        {
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
            {
                return redirectResult;
            }

            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de importar empleados.";
                return RedirectToAction(nameof(Index));
            }

            if (excelFile == null || excelFile.Length == 0)
            {
                TempData["Error"] = "Seleccione un archivo XLSX válido.";
                return RedirectToAction(nameof(Index));
            }

            if (!string.Equals(Path.GetExtension(excelFile.FileName), ".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Solo se permite el formato .xlsx para esta importación.";
                return RedirectToAction(nameof(Index));
            }

            if (excelFile.Length > 10 * 1024 * 1024)
            {
                TempData["Error"] = "El archivo excede el tamaño máximo permitido de 10 MB.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await using var stream = excelFile.OpenReadStream();
                var previewRows = await _importService.PreviewAsync(stream, currentCompanyId.Value);
                SavePreviewRowsInSession(previewRows);

                if (!previewRows.Any())
                {
                    TempData["Warning"] = "El archivo no contiene filas de empleados para procesar.";
                }
                else if (previewRows.All(r => !r.IsValid))
                {
                    TempData["Warning"] = "La vista previa se generó, pero todas las filas tienen errores y no podrán importarse.";
                }
                else
                {
                    TempData["Success"] = $"Vista previa generada. Filas válidas: {previewRows.Count(r => r.IsValid)}.";
                }

                return View("Index", await BuildViewModelAsync(previewRows));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"No se pudo procesar el archivo: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveImported()
        {
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
            {
                return redirectResult;
            }

            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de importar empleados.";
                return RedirectToAction(nameof(Index));
            }

            var previewRows = GetPreviewRowsFromSession();
            if (!previewRows.Any())
            {
                TempData["Error"] = "No hay una vista previa lista para importar. Cargue un archivo nuevamente.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _importService.SaveAsync(currentCompanyId.Value, previewRows, User.Identity?.Name ?? "system");
            HttpContext.Session.Remove(PreviewSessionKey);

            if (result.ImportedCount > 0)
            {
                TempData["Success"] = $"Se importaron {result.ImportedCount} empleados correctamente.";
            }

            if (result.Errors.Any())
            {
                TempData["Warning"] = string.Join(" ", result.Errors.Take(5));
            }

            return RedirectToAction("Index", "Employees");
        }

        private async Task<InitialEmployeeImportViewModel> BuildViewModelAsync(List<InitialEmployeeImportRowViewModel>? previewRows = null)
        {
            var company = await GetCurrentCompanyAsync();
            var companyId = GetCurrentCompanyId();
            var issues = companyId.HasValue
                ? await _importService.GetPrerequisiteIssuesAsync(companyId.Value)
                : new List<string> { "Debe seleccionar una compañía antes de continuar." };

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Administración SuperAdmin", Url = "/SuperAdminManagement/Index" },
                new BreadcrumbItem { Text = "Importación inicial de empleados" }
            };

            return new InitialEmployeeImportViewModel
            {
                CompanyName = company?.Name ?? string.Empty,
                ExistingEmployeeCount = companyId.HasValue ? await _context.Employees.CountAsync(e => e.CompanyId == companyId.Value && !e.IsDeleted) : 0,
                PrerequisiteIssues = issues,
                PreviewRows = previewRows ?? new List<InitialEmployeeImportRowViewModel>()
            };
        }

        private List<InitialEmployeeImportRowViewModel> GetPreviewRowsFromSession()
        {
            var json = HttpContext.Session.GetString(PreviewSessionKey);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<InitialEmployeeImportRowViewModel>();
            }

            return JsonSerializer.Deserialize<List<InitialEmployeeImportRowViewModel>>(json) ?? new List<InitialEmployeeImportRowViewModel>();
        }

        private void SavePreviewRowsInSession(List<InitialEmployeeImportRowViewModel> rows)
        {
            HttpContext.Session.SetString(PreviewSessionKey, JsonSerializer.Serialize(rows));
        }
    }
}