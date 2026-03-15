using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
        public IActionResult Import()
        {
            // Ensure company is selected
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            return View();
        }

        // POST: Accounts/Preview
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                                var accountNumber = dataRow[0]?.ToString()?.Trim();
                                var description = dataRow[1]?.ToString()?.Trim();

                                var importItem = new AccountImportViewModel
                                {
                                    RowNumber = row, // 1-based for display
                                    AccountNumber = accountNumber ?? "",
                                    Description = description ?? "",
                                    IsValid = true,
                                    ErrorMessage = ""
                                };

                                // Validate required fields
                                if (string.IsNullOrEmpty(accountNumber))
                                {
                                    importItem.IsValid = false;
                                    importItem.ErrorMessage = "El número de cuenta es requerido.";
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
                                    AccountNumber = dataRow[0]?.ToString()?.Trim() ?? "",
                                    Description = dataRow[1]?.ToString()?.Trim() ?? "",
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

            try
            {
                foreach (var account in accounts.Where(a => a.IsValid))
                {
                    try
                    {
                        // Double-check that account doesn't exist (in case of concurrent operations)
                        var existingAccount = await _context.Accounts
                            .FirstOrDefaultAsync(a => a.AccountNumber == account.AccountNumber && a.CompanyId == currentCompanyId.Value);

                        if (existingAccount == null)
                        {
                            var newAccount = new AccountEntity
                            {
                                AccountNumber = account.AccountNumber,
                                Description = account.Description,
                                CompanyId = currentCompanyId.Value,
                                IsActive = true,
                                Created = DateTime.Now,
                                CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            };

                            _context.Accounts.Add(newAccount);
                            importedCount++;
                        }
                        else
                        {
                            errors.Add($"Cuenta '{account.AccountNumber}' ya existe.");
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error guardando cuenta '{account.AccountNumber}': {ex.Message}");
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
    }
}