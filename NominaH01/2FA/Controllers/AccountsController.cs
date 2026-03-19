using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExcelDataReader;
using System.Data;
using _2FA.Data;
using _2FA.Data.Entities;
using _2FA.Models;

namespace _2FA.Controllers
{
    public class AccountsController : BaseController
    {
        private static readonly string[] ImportTemplateHeaders = { "AccountNumber", "Description", "Clasification" };

        public AccountsController(ApplicationDbContext context) : base(context)
        {
        }

        // GET: Accounts
        public async Task<IActionResult> Index()
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            var accounts = FilterByCurrentCompany(_context.Accounts);
            return View(await accounts.ToListAsync());
        }

        // GET: Accounts/Import
        [Authorize(Roles = "Administrator")]
        public IActionResult Import()
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            return View();
        }

        // GET: Accounts/DownloadTemplate
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DownloadTemplate()
        {
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            var company = await GetCurrentCompanyAsync();

            using var workbook = new XLWorkbook();

            var instructionsSheet = workbook.Worksheets.Add("Instrucciones");
            instructionsSheet.Cell(1, 1).Value = "Importación de catálogo de cuentas";
            instructionsSheet.Cell(2, 1).Value = "Columnas requeridas:";
            instructionsSheet.Cell(3, 1).Value = "AccountNumber y Clasification.";
            instructionsSheet.Cell(4, 1).Value = "Description es opcional.";
            instructionsSheet.Cell(5, 1).Value = "Clasification acepta: Activo, Pasivo, Patrimonio, Ingreso, Gasto, Costo.";
            instructionsSheet.Cell(6, 1).Value = "No duplique números de cuenta dentro de la misma compañía.";
            instructionsSheet.Columns().AdjustToContents();

            var accountsSheet = workbook.Worksheets.Add("Cuentas");
            for (var i = 0; i < ImportTemplateHeaders.Length; i++)
            {
                accountsSheet.Cell(1, i + 1).Value = ImportTemplateHeaders[i];
                accountsSheet.Cell(1, i + 1).Style.Font.Bold = true;
                accountsSheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
            }

            accountsSheet.Cell(2, 1).Value = "1-01-001";
            accountsSheet.Cell(2, 2).Value = "Caja general";
            accountsSheet.Cell(2, 3).Value = AccountClasificationType.Activo.ToString();
            accountsSheet.SheetView.FreezeRows(1);
            accountsSheet.Columns().AdjustToContents();

            var catalogSheet = workbook.Worksheets.Add("Catalogos");
            catalogSheet.Cell(1, 1).Value = "Clasificaciones válidas";
            catalogSheet.Cell(1, 1).Style.Font.Bold = true;
            var row = 2;
            foreach (var clasification in Enum.GetValues<AccountClasificationType>())
            {
                catalogSheet.Cell(row, 1).Value = clasification.ToString();
                row++;
            }
            catalogSheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            var safeCompanyName = (company?.Name ?? "empresa").Replace(" ", "-").ToLowerInvariant();
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"plantilla-cuentas-{safeCompanyName}.xlsx");
        }

        // POST: Accounts/Preview
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Preview(IFormFile excelFile)
        {
            // Debug logging
            System.Diagnostics.Debug.WriteLine("Preview action called");

            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            if (excelFile == null || excelFile.Length == 0)
            {
                TempData["Error"] = "Por favor seleccione un archivo Excel válido.";
                return RedirectToAction(nameof(Import));
            }

            // Additional validation for file size (optional, max 10MB)
            if (excelFile.Length > 10 * 1024 * 1024)
            {
                TempData["Error"] = "El archivo es demasiado grande. El tamaño máximo permitido es 10MB.";
                return RedirectToAction(nameof(Import));
            }

            // Validate file extension
            var allowedExtensions = new[] { ".xlsx", ".xls" };
            var fileExtension = Path.GetExtension(excelFile.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                TempData["Error"] = "Solo se permiten archivos Excel (.xlsx, .xls).";
                return RedirectToAction(nameof(Import));
            }

            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de importar cuentas.";
                return RedirectToAction("SwitchCompany", "Home");
            }

            var previewData = new List<AccountImportViewModel>();
            var errors = new List<string>();

            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                using (var stream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(stream);
                    stream.Position = 0;

                    using (var reader = fileExtension == ".xlsx"
                        ? ExcelReaderFactory.CreateOpenXmlReader(stream)
                        : ExcelReaderFactory.CreateBinaryReader(stream))
                    {
                        var dataSet = reader.AsDataSet();
                        if (dataSet == null || dataSet.Tables.Count == 0)
                        {
                            TempData["Error"] = "El archivo Excel no contiene hojas de trabajo válidas.";
                            return RedirectToAction(nameof(Import));
                        }

                        var dataTable = dataSet.Tables[0];
                        if (dataTable.Rows.Count < 2) // At least header + 1 data row
                        {
                            TempData["Error"] = "El archivo Excel debe contener al menos una fila de datos además del encabezado.";
                            return RedirectToAction(nameof(Import));
                        }

                        // Process rows starting from row 1 (skip header row 0)
                        for (int row = 1; row < dataTable.Rows.Count; row++)
                        {
                            try
                            {
                                var dataRow = dataTable.Rows[row];
                                var accountNumber = GetCellValue(dataRow, 0);
                                var description = GetCellValue(dataRow, 1);
                                var clasificationText = GetCellValue(dataRow, 2);
                                var clasification = TryParseClasification(clasificationText, out var parsedClasification)
                                    ? parsedClasification
                                    : null;

                                var importItem = new AccountImportViewModel
                                {
                                    RowNumber = row, // 1-based for display
                                    AccountNumber = accountNumber ?? "",
                                    Description = description ?? "",
                                    Clasification = clasification,
                                    IsValid = true,
                                    ErrorMessage = ""
                                };

                                // Validate required fields
                                if (string.IsNullOrEmpty(accountNumber))
                                {
                                    importItem.IsValid = false;
                                    importItem.ErrorMessage = "El número de cuenta es requerido.";
                                }
                                else if (!clasification.HasValue)
                                {
                                    importItem.IsValid = false;
                                    importItem.ErrorMessage = string.IsNullOrWhiteSpace(clasificationText)
                                        ? "La clasificación es requerida."
                                        : $"La clasificación '{clasificationText}' no es válida.";
                                }
                                else
                                {
                                    // Check if account already exists for this company
                                    var existingAccount = await _context.Accounts
                                        .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber && a.CompanyId == currentCompanyId.Value);

                                    if (existingAccount != null)
                                    {
                                        importItem.IsValid = false;
                                        importItem.ErrorMessage = "La cuenta ya existe para esta compañía.";
                                    }
                                }

                                previewData.Add(importItem);
                            }
                            catch (Exception ex)
                            {
                                var dataRow = dataTable.Rows[row];
                                previewData.Add(new AccountImportViewModel
                                {
                                    RowNumber = row,
                                    AccountNumber = GetCellValue(dataRow, 0) ?? "",
                                    Description = GetCellValue(dataRow, 1) ?? "",
                                    Clasification = TryParseClasification(GetCellValue(dataRow, 2), out var parsedClasification) ? parsedClasification : null,
                                    IsValid = false,
                                    ErrorMessage = $"Error procesando la fila: {ex.Message}"
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error procesando el archivo: {ex.Message}";
                return RedirectToAction(nameof(Import));
            }

            // Store preview data in TempData for the SaveImported action
            TempData["PreviewData"] = System.Text.Json.JsonSerializer.Serialize(previewData);

            return View("Import", previewData);
        }

        // POST: Accounts/SaveImported
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> SaveImported(List<AccountImportViewModel> accounts)
        {
            // Debug logging
            System.Diagnostics.Debug.WriteLine($"SaveImported called with {accounts?.Count ?? 0} accounts");
            if (accounts != null)
            {
                foreach (var account in accounts)
                {
                    System.Diagnostics.Debug.WriteLine($"Account: {account.AccountNumber}, Valid: {account.IsValid}");
                }
            }

            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de importar cuentas.";
                return RedirectToAction("SwitchCompany", "Home");
            }

            var importedCount = 0;
            var errors = new List<string>();

            if (accounts == null || !accounts.Any())
            {
                TempData["Error"] = "No se recibieron cuentas para importar.";
                return RedirectToAction(nameof(Import));
            }

            try
            {
                foreach (var account in accounts)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(account.AccountNumber))
                        {
                            errors.Add($"Fila {account.RowNumber}: el número de cuenta es requerido.");
                            continue;
                        }

                        if (!account.Clasification.HasValue)
                        {
                            errors.Add($"Fila {account.RowNumber}: la clasificación es requerida.");
                            continue;
                        }

                        // Double-check that account doesn't exist (in case of concurrent operations)
                        var existingAccount = await _context.Accounts
                            .FirstOrDefaultAsync(a => a.AccountNumber == account.AccountNumber && a.CompanyId == currentCompanyId.Value);

                        if (existingAccount == null)
                        {
                            var newAccount = new AccountEntity
                            {
                                AccountNumber = account.AccountNumber,
                                Description = account.Description,
                                Clasification = account.Clasification,
                                CompanyId = currentCompanyId.Value,
                                IsActive = true,
                                Created = DateTime.Now,
                                CreatedBy = User.Identity?.Name ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system"
                            };

                            _context.Accounts.Add(newAccount);
                            importedCount++;
                        }
                        else
                        {
                            errors.Add($"Fila {account.RowNumber}: la cuenta '{account.AccountNumber}' ya existe.");
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Fila {account.RowNumber}: error guardando cuenta '{account.AccountNumber}': {ex.Message}");
                    }
                }

                await _context.SaveChangesAsync();

                if (importedCount > 0)
                {
                    TempData["Success"] = $"Se importaron exitosamente {importedCount} cuentas.";
                }

                if (errors.Any())
                {
                    TempData["Warning"] = $"Errores encontrados:\n" + string.Join("\n", errors.Take(10));
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error guardando las cuentas: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Accounts/Details/5
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

            var accountEntity = await FilterByCurrentCompany(_context.Accounts)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (accountEntity == null)
            {
                return NotFound();
            }

            return View(accountEntity);
        }

        // GET: Accounts/Create
        public IActionResult Create()
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            return View();
        }

        // POST: Accounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( AccountEntity accountEntity)
        {
            // Ensure company is selected and set it
            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de crear una cuenta.";
                return RedirectToAction("SwitchCompany", "Home");
            }
            accountEntity.CompanyId = currentCompanyId.Value;

            ModelState.Remove("Created");
            ModelState.Remove("CreatedBy");

            if (!accountEntity.Clasification.HasValue)
            {
                ModelState.AddModelError(nameof(AccountEntity.Clasification), "El campo Clasificación es requerido");
            }

            if (ModelState.IsValid)
            {
                accountEntity.Created = DateTime.UtcNow;
                accountEntity.CreatedBy = User.Identity.Name ?? "system";

                _context.Add(accountEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(accountEntity);
        }

        // GET: Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            if (id == null)
            {
                return NotFound();
            }

            var accountEntity = await FilterByCurrentCompany(_context.Accounts).FirstOrDefaultAsync(m => m.Id == id);
            if (accountEntity == null)
            {
                return NotFound();
            }
            return View(accountEntity);
        }

        // POST: Accounts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AccountEntity accountEntity)
        {
            if (id != accountEntity.Id)
            {
                return NotFound();
            }

            // Ensure company is selected and set it
            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de editar una cuenta.";
                return RedirectToAction("SwitchCompany", "Home");
            }
            accountEntity.CompanyId = currentCompanyId.Value;

            ModelState.Remove("Modified");
            ModelState.Remove("ModifiedBy");

            if (!accountEntity.Clasification.HasValue)
            {
                ModelState.AddModelError(nameof(AccountEntity.Clasification), "El campo Clasificación es requerido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    accountEntity.Modified = DateTime.UtcNow;
                    accountEntity.ModifiedBy = User.Identity.Name ?? "system";
                    
                    _context.Update(accountEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountEntityExists(accountEntity.Id))
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
            return View(accountEntity);
        }

        // GET: Accounts/Delete/5
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

            var accountEntity = await FilterByCurrentCompany(_context.Accounts)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (accountEntity == null)
            {
                return NotFound();
            }

            return View(accountEntity);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var accountEntity = await _context.Accounts.FindAsync(id);
            if (accountEntity != null)
            {
                _context.Accounts.Remove(accountEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountEntityExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }

        private static string? GetCellValue(DataRow dataRow, int index)
        {
            if (index < 0 || index >= dataRow.ItemArray.Length)
            {
                return null;
            }

            return dataRow[index]?.ToString()?.Trim();
        }

        private static bool TryParseClasification(string? value, out AccountClasificationType? clasification)
        {
            clasification = null;

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var normalized = value.Trim().ToUpperInvariant();
            clasification = normalized switch
            {
                "ACTIVO" => AccountClasificationType.Activo,
                "PASIVO" => AccountClasificationType.Pasivo,
                "PATRIMONIO" => AccountClasificationType.Patrimonio,
                "INGRESO" => AccountClasificationType.Ingreso,
                "GASTO" => AccountClasificationType.Gasto,
                "COSTO" => AccountClasificationType.Costo,
                _ => null
            };

            return clasification.HasValue;
        }
    }
}