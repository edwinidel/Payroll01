using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _2FA.Data;
using _2FA.Data.Entities;
using _2FA.Models;
using _2FA.Services;

namespace _2FA.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class BankDepositStructuresController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BankDepositFileGenerator _fileGenerator;

        public BankDepositStructuresController(ApplicationDbContext context, BankDepositFileGenerator fileGenerator)
        {
            _context = context;
            _fileGenerator = fileGenerator;
        }

        // GET: BankDepositStructures
        public async Task<IActionResult> Index()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Administración SuperAdmin", Url = "/SuperAdminManagement/Index" },
                new BreadcrumbItem { Text = "Configuración de Bancos", Url = "/Home/Banks" },
                new BreadcrumbItem { Text = "Estructuras de Depósito Bancario" }
            };

            return View(await _context.BankDepositStructures.Where(s => !s.IsDeleted).ToListAsync());
        }

        // GET: BankDepositStructures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bankDepositStructureEntity = await _context.BankDepositStructures
                .Include(b => b.Fields)
                .ThenInclude(f => f.Company)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (bankDepositStructureEntity == null)
            {
                return NotFound();
            }

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Administración SuperAdmin", Url = "/SuperAdminManagement/Index" },
                new BreadcrumbItem { Text = "Configuración de Bancos", Url = "/Home/Banks" },
                new BreadcrumbItem { Text = "Estructuras de Depósito Bancario", Url = "/BankDepositStructures/Index" },
                new BreadcrumbItem { Text = $"Detalle - {bankDepositStructureEntity.StructureName}" }
            };

            return View(bankDepositStructureEntity);
        }

        // GET: BankDepositStructures/Create
        public IActionResult Create()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Administración SuperAdmin", Url = "/SuperAdminManagement/Index" },
                new BreadcrumbItem { Text = "Configuración de Bancos", Url = "/Home/Banks" },
                new BreadcrumbItem { Text = "Estructuras de Depósito Bancario", Url = "/BankDepositStructures/Index" },
                new BreadcrumbItem { Text = "Nueva Estructura" }
            };

            ViewData["BankDepositType"] = new SelectList(Enum.GetValues(typeof(BankDepositType)).Cast<BankDepositType>().Select(e => new { Value = e, Text = e.ToString() }), "Value", "Text");
            ViewData["FileType"] = new SelectList(Enum.GetValues(typeof(FileType)).Cast<FileType>().Select(e => new { Value = e, Text = e.ToString() }), "Value", "Text");

            return View();
        }

        // POST: BankDepositStructures/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BankDepositStructureEntity bankDepositStructureEntity)
        {
            if (ModelState.IsValid)
            {
                bankDepositStructureEntity.Created = DateTime.UtcNow;
                bankDepositStructureEntity.CreatedBy = User.Identity?.Name ?? string.Empty;
                _context.Add(bankDepositStructureEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["BankDepositType"] = new SelectList(Enum.GetValues(typeof(BankDepositType)).Cast<BankDepositType>().Select(e => new { Value = e, Text = e.ToString() }), "Value", "Text", bankDepositStructureEntity.BankDepositType);
            ViewData["FileType"] = new SelectList(Enum.GetValues(typeof(FileType)).Cast<FileType>().Select(e => new { Value = e, Text = e.ToString() }), "Value", "Text", bankDepositStructureEntity.FileType);

            return View(bankDepositStructureEntity);
        }

        // GET: BankDepositStructures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bankDepositStructureEntity = await _context.BankDepositStructures.FindAsync(id);
            if (bankDepositStructureEntity == null)
            {
                return NotFound();
            }

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Administración SuperAdmin", Url = "/SuperAdminManagement/Index" },
                new BreadcrumbItem { Text = "Configuración de Bancos", Url = "/Home/Banks" },
                new BreadcrumbItem { Text = "Estructuras de Depósito Bancario", Url = "/BankDepositStructures/Index" },
                new BreadcrumbItem { Text = $"Editar - {bankDepositStructureEntity.StructureName}" }
            };

            ViewData["BankDepositType"] = new SelectList(Enum.GetValues(typeof(BankDepositType)).Cast<BankDepositType>().Select(e => new { Value = e, Text = e.ToString() }), "Value", "Text", bankDepositStructureEntity.BankDepositType);
            ViewData["FileType"] = new SelectList(Enum.GetValues(typeof(FileType)).Cast<FileType>().Select(e => new { Value = e, Text = e.ToString() }), "Value", "Text", bankDepositStructureEntity.FileType);

            return View(bankDepositStructureEntity);
        }

        // POST: BankDepositStructures/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BankDepositStructureEntity bankDepositStructureEntity)
        {
            if (id != bankDepositStructureEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bankDepositStructureEntity.Modified = DateTime.UtcNow;
                    bankDepositStructureEntity.ModifiedBy = User.Identity?.Name ?? string.Empty;
                    _context.Update(bankDepositStructureEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BankDepositStructureEntityExists(bankDepositStructureEntity.Id))
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

            ViewData["BankDepositType"] = new SelectList(Enum.GetValues(typeof(BankDepositType)).Cast<BankDepositType>().Select(e => new { Value = e, Text = e.ToString() }), "Value", "Text", bankDepositStructureEntity.BankDepositType);
            ViewData["FileType"] = new SelectList(Enum.GetValues(typeof(FileType)).Cast<FileType>().Select(e => new { Value = e, Text = e.ToString() }), "Value", "Text", bankDepositStructureEntity.FileType);

            return View(bankDepositStructureEntity);
        }

        // GET: BankDepositStructures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bankDepositStructureEntity = await _context.BankDepositStructures
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bankDepositStructureEntity == null)
            {
                return NotFound();
            }

            return View(bankDepositStructureEntity);
        }

        // POST: BankDepositStructures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bankDepositStructureEntity = await _context.BankDepositStructures.FindAsync(id);
            if (bankDepositStructureEntity != null)
            {
                bankDepositStructureEntity.Deleted = DateTime.UtcNow;
                bankDepositStructureEntity.DeletedBy = User.Identity?.Name ?? string.Empty;
                bankDepositStructureEntity.IsDeleted = true;
                _context.BankDepositStructures.Update(bankDepositStructureEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: BankDepositStructures/ManageFields/5
        public async Task<IActionResult> ManageFields(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var structure = await _context.BankDepositStructures
                .Include(s => s.Fields)
                .ThenInclude(f => f.Company)
                // Removed incorrect ordering by structure.Order (non-existent property)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (structure == null)
            {
                return NotFound();
            }

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Administración SuperAdmin", Url = "/SuperAdminManagement/Index" },
                new BreadcrumbItem { Text = "Estructuras de Depósito Bancario", Url = "/BankDepositStructures/Index" },
                new BreadcrumbItem { Text = $"Gestionar Campos - {structure.StructureName}" }
            };

            var companies = await _context.Companies.Where(c => !c.IsDeleted).ToListAsync();
            ViewBag.Companies = companies;

            // Get available fields (all field content options not already in this structure)
            var availableFields = GetFieldContentOptions()
                .Where(f => !structure.Fields.Any(field => field.FieldContent == f.Value))
                .ToList();
            
            ViewBag.AvailableFields = availableFields;
            return View(structure);
        }

        /// <summary>
        /// Generates the bank deposit file based on the selected structure and payroll.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateFile(int id, int companyId)
        {
            try
            {
                var bytes = await _fileGenerator.GenerateFileAsync(id, companyId);
                var structure = await _context.BankDepositStructures.FindAsync(id);
                if (structure == null)
                {
                    return NotFound();
                }
                var extension = structure.FileType switch
                {
                    FileType.CSV => "csv",
                    FileType.TXT => "txt",
                    FileType.XLSX => "xlsx",
                    _ => "txt"
                };
                var fileName = $"{structure.StructureName}_{DateTime.Now:yyyyMMdd}.{extension}";
                return File(bytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error generando archivo: {ex.Message}";
                return RedirectToAction("ManageFields", new { id });
            }
        }

        // GET: BankDepositStructures/CreateField/5
        public async Task<IActionResult> CreateField(int structureId)
        {
            var structure = await _context.BankDepositStructures.FindAsync(structureId);
            if (structure == null)
            {
                return NotFound();
            }

            ViewData["Companies"] = new SelectList(await _context.Companies.Where(c => !c.IsDeleted).ToListAsync(), "Id", "Name");
            ViewData["FieldContents"] = GetFieldContentOptions();

            var field = new BankDepositFieldEntity { BankDepositStructureId = structureId };
            return View(field);
        }

        // POST: BankDepositStructures/BulkCreateFields
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkCreateFields(int structureId, int companyId, List<string> selectedFields)
        {
            if (structureId == 0 || companyId == 0 || selectedFields == null || !selectedFields.Any())
            {
                return BadRequest("Invalid request data");
            }
        
            try
            {
                var maxOrder = _context.BankDepositFields
                    .Where(f => f.BankDepositStructureId == structureId && !f.IsDeleted)
                    .Max(f => (int?)f.Order) ?? 0;
        
                var options = GetFieldContentOptions().ToDictionary(o => o.Value, o => o.Text);
        
                foreach (var fieldContent in selectedFields)
                {
                    var field = new BankDepositFieldEntity
                    {
                        BankDepositStructureId = structureId,
                        CompanyId = companyId,
                        FieldContent = fieldContent,
                        FieldName = options.TryGetValue(fieldContent, out var text) ? text : fieldContent,
                        Order = maxOrder + 1,
                        Created = DateTime.UtcNow,
                        CreatedBy = User.Identity?.Name ?? string.Empty
                    };
                    maxOrder++;
                    _context.Add(field);
                }
        
                await _context.SaveChangesAsync();
                return RedirectToAction("ManageFields", new { id = structureId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error creating fields: {ex.Message}";
                return RedirectToAction("ManageFields", new { id = structureId });
            }
        }

        // POST: BankDepositStructures/CreateField
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateFieldOrders(int structureId, List<int> fieldIds, List<int> fieldOrders)
        {
            if (fieldIds.Count != fieldOrders.Count)
            {
                return BadRequest("Invalid data");
            }
        
            try
            {
                for (int i = 0; i < fieldIds.Count; i++)
                {
                    var field = await _context.BankDepositFields.FindAsync(fieldIds[i]);
                    if (field != null && field.BankDepositStructureId == structureId && !field.IsDeleted)
                    {
                        field.Order = fieldOrders[i];
                        field.Modified = DateTime.UtcNow;
                        field.ModifiedBy = User.Identity?.Name ?? string.Empty;
                        _context.Update(field);
                    }
                }
        
                await _context.SaveChangesAsync();
                return RedirectToAction("ManageFields", new { id = structureId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating orders: {ex.Message}";
                return RedirectToAction("ManageFields", new { id = structureId });
            }
        }

        // POST: BankDepositStructures/BulkEditFields
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkEditFields(int structureId, List<BankDepositFieldEntity> fields)
        {
            if (structureId == 0 || fields == null || !fields.Any())
            {
                return BadRequest("Invalid request data");
            }
        
            try
            {
                foreach (var postedField in fields)
                {
                    var existingField = await _context.BankDepositFields
                        .FirstOrDefaultAsync(f => f.Id == postedField.Id && f.BankDepositStructureId == structureId && !f.IsDeleted);
        
                    if (existingField == null)
                    {
                        continue;
                    }
        
                    existingField.FieldName = postedField.FieldName ?? existingField.FieldName;
                    existingField.Order = postedField.Order;
                    existingField.Ignore = postedField.Ignore;
                    existingField.CompanyId = postedField.CompanyId;
                    // Ensure FieldContent is not overwritten if not posted
                    if (!string.IsNullOrEmpty(postedField.FieldContent))
                    {
                        existingField.FieldContent = postedField.FieldContent;
                    }
        
                    existingField.Modified = DateTime.UtcNow;
                    existingField.ModifiedBy = User.Identity?.Name ?? string.Empty;
        
                    _context.Update(existingField);
                }
        
                await _context.SaveChangesAsync();
                return RedirectToAction("ManageFields", new { id = structureId });
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = $"Database update error: {ex.Message}";
                if (ex.InnerException != null)
                {
                    TempData["Error"] += $" Details: {ex.InnerException.Message}";
                }
                return RedirectToAction("ManageFields", new { id = structureId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating fields: {ex.Message}";
                if (ex.InnerException != null)
                {
                    TempData["Error"] += $" Inner: {ex.InnerException.Message}";
                }
                return RedirectToAction("ManageFields", new { id = structureId });
            }
        }

        // GET: BankDepositStructures/EditField/5
        public async Task<IActionResult> EditField(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bankDepositFieldEntity = await _context.BankDepositFields.FindAsync(id);
            if (bankDepositFieldEntity == null)
            {
                return NotFound();
            }

            ViewData["Companies"] = new SelectList(await _context.Companies.Where(c => !c.IsDeleted).ToListAsync(), "Id", "Name", bankDepositFieldEntity.CompanyId);
            ViewData["FieldContents"] = GetFieldContentOptions();

            return View(bankDepositFieldEntity);
        }

        // POST: BankDepositStructures/EditField/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditField(int id, BankDepositFieldEntity bankDepositFieldEntity)
        {
            if (id != bankDepositFieldEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bankDepositFieldEntity.Modified = DateTime.UtcNow;
                    bankDepositFieldEntity.ModifiedBy = User.Identity?.Name ?? string.Empty;
                    _context.Update(bankDepositFieldEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BankDepositFieldEntityExists(bankDepositFieldEntity.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("ManageFields", new { id = bankDepositFieldEntity.BankDepositStructureId });
            }

            ViewData["Companies"] = new SelectList(await _context.Companies.Where(c => !c.IsDeleted).ToListAsync(), "Id", "Name", bankDepositFieldEntity.CompanyId);
            ViewData["FieldContents"] = GetFieldContentOptions();

            return View(bankDepositFieldEntity);
        }

        // GET: BankDepositStructures/DeleteField/5
        public async Task<IActionResult> DeleteField(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bankDepositFieldEntity = await _context.BankDepositFields
                .Include(b => b.Company)
                .Include(b => b.BankDepositStructure)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bankDepositFieldEntity == null)
            {
                return NotFound();
            }

            return View(bankDepositFieldEntity);
        }

        // POST: BankDepositStructures/DeleteField/5
        [HttpPost, ActionName("DeleteField")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFieldConfirmed(int id)
        {
            var bankDepositFieldEntity = await _context.BankDepositFields.FindAsync(id);
            if (bankDepositFieldEntity != null)
            {
                bankDepositFieldEntity.Deleted = DateTime.UtcNow;
                bankDepositFieldEntity.DeletedBy = User.Identity?.Name ?? string.Empty;
                bankDepositFieldEntity.IsDeleted = true;
                _context.BankDepositFields.Update(bankDepositFieldEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("ManageFields", new { id = bankDepositFieldEntity?.BankDepositStructureId });
        }

        private List<SelectListItem> GetFieldContentOptions()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "Employee.FullName", Text = "Nombre Completo del Empleado" },
                new SelectListItem { Value = "Employee.IdDocument", Text = "Documento de Identidad" },
                new SelectListItem { Value = "Employee.Code", Text = "Código del Empleado" },
                new SelectListItem { Value = "Employee.CEMAIL", Text = "Correo Electrónico" },
                new SelectListItem { Value = "Employee.PayAccount", Text = "Cuenta de Pago" },
                new SelectListItem { Value = "Banks.Name", Text = "Nombre del Banco" },
                new SelectListItem { Value = "Banks.TransitBankId", Text = "ID de Banco Transitorio" },
                new SelectListItem { Value = "PayrollTmpHeader.PaymentGroupId", Text = "ID de Grupo de Pago" },
                new SelectListItem { Value = "PayrollTmpHeader.EndDate", Text = "Fecha de Fin de Nómina" },
                new SelectListItem { Value = "PayrollTmpEmployees.NetPay", Text = "Pago Neto" }
            };
        }

        private bool BankDepositStructureEntityExists(int id)
        {
            return _context.BankDepositStructures.Any(e => e.Id == id);
        }

        private bool BankDepositFieldEntityExists(int id)
        {
            return _context.BankDepositFields.Any(e => e.Id == id);
        }
    }
}