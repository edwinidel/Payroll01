using _2FA.Data;
using _2FA.Data.Entities;
using _2FA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.Extensions.Caching.Memory;

namespace _2FA.Controllers
{
    public class PayrollTmpHeadersController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PayrollTmpHeadersController> _logger;
        private readonly IMemoryCache _cache;
        private static readonly TimeSpan _otcTokenTtl = TimeSpan.FromMinutes(25); // slightly less than OTC validity

        public PayrollTmpHeadersController(ApplicationDbContext context, IHttpClientFactory httpClientFactory, ILogger<PayrollTmpHeadersController> logger, IMemoryCache cache) : base(context)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _cache = cache;
        }

        // GET: PayrollTmpHeaders
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

            // Debug: Check context and company selection
            Console.WriteLine($"[DEBUG] Index: _context is null: {_context == null}");
            var currentCompanyId = GetCurrentCompanyId();
            Console.WriteLine($"[DEBUG] Index: CurrentCompanyId: {currentCompanyId}");
            if (_context?.PayrollTmpHeaders == null)
            {
                Console.WriteLine("[DEBUG] Index: _context.PayrollTmpHeaders is null");
                return View(new List<PayrollTmpHeaderEntity>());
            }

            var applicationDbContext = FilterByCurrentCompany(_context.PayrollTmpHeaders.Include(p => p.PaymentGroup));
            Console.WriteLine($"[DEBUG] Index: applicationDbContext is null: {applicationDbContext == null}");

            if (applicationDbContext == null)
            {
                Console.WriteLine("[DEBUG] Index: Returning empty list due to null context");
                return View(new List<PayrollTmpHeaderEntity>());
            }

            try
            {
                Console.WriteLine("[DEBUG] Index: About to execute ToListAsync");
                var results = await applicationDbContext.ToListAsync();
                Console.WriteLine($"[DEBUG] Index: ToListAsync returned {results?.Count ?? 0} items");

                // Debug: Check for null references in results
                if (results != null)
                {
                    for (int i = 0; i < results.Count; i++)
                    {
                        var item = results[i];
                        Console.WriteLine($"[DEBUG] Index: Item {i} - Id: {item?.Id}, PaymentGroup: {item?.PaymentGroup?.Name ?? "NULL"}, CompanyId: {item?.CompanyId}");

                        if (item?.PaymentGroup == null)
                        {
                            Console.WriteLine($"[WARNING] Index: Item {i} has null PaymentGroup! PaymentGroupId: {item?.PaymentGroupId}");
                        }
                    }
                }

                return View(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Index: Exception during ToListAsync: {ex.Message}");
                Console.WriteLine($"[ERROR] Index: StackTrace: {ex.StackTrace}");
                throw; // Re-throw to maintain original behavior
            }
        }

        // GET: PayrollTmpHeaders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var redirect = EnsureCompanySelected();
            if (redirect != null) return redirect;

            if (id == null)
            {
                return NotFound();
            }

            var payrollTmpHeaderEntity = await GetFullPayrollHeader(id.Value);
            if (payrollTmpHeaderEntity == null)
            {
                return NotFound();
            }

            ViewBag.PayrollTmpHeaderId = payrollTmpHeaderEntity.Id;

            // Populate dropdown data for overtime modal
            ViewBag.TypeOfDays = await _context.TypeOfDays
                .Where(t => t.IsActive)
                .OrderBy(t => t.Code)
                .ToListAsync();

            ViewBag.TypeOfWorkSchedules = await _context.TypeOfWorkSchedules
                .Where(t => t.IsActive)
                .OrderBy(t => t.Code)
                .ToListAsync();

            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Home", Url = "/" },
                new BreadcrumbItem { Text = "Administración", Url = "/Home/Administration" },
                new BreadcrumbItem { Text = "Listado de Planillas", Url = "/PayrollTmpHeaders" },
                new BreadcrumbItem { Text = $"Detalle {(payrollTmpHeaderEntity.Notes.Length <= 100 ? payrollTmpHeaderEntity.Notes : payrollTmpHeaderEntity.Notes[..100])}" }
            };

            return View(payrollTmpHeaderEntity);
        }

        // GET: PayrollTmpHeaders/Create
        public IActionResult Create(bool isContractor = false)
        {
            var redirect = EnsureCompanySelected();
            if (redirect != null) return redirect;

            var payrollHeader = new PayrollHeaderViewModel
            {
                Status = "Draft"
            };

            if (_context?.PaymentGroups == null)
            {
                ViewData["PaymentGroupId"] = new SelectList(Enumerable.Empty<PaymentGroupEntity>(), "Id", "Name");
            }
            else
            {
                ViewData["PaymentGroupId"] = new SelectList(FilterByCurrentCompany(_context.PaymentGroups), "Id", "Name");
            }

            if (_context?.PayrollTypes == null)
            {
                ViewData["PayrollTypeId"] = new SelectList(Enumerable.Empty<PayrollTypeEntity>(), "Id", "Name");
            }
            else
            {
                ViewData["PayrollTypeId"] = new SelectList(_context.PayrollTypes, "Id", "Name");
            }

            // Pass the isContractor parameter to the view
            ViewBag.IsContractor = isContractor;
            ViewBag.Title = isContractor ? "Nueva Planilla de Contratistas" : "Nueva Nómina";

            return View(payrollHeader);
        }

        // POST: PayrollTmpHeaders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PayrollTmpHeaderEntity payrollTmpHeaderEntity)
        {
            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de continuar.";
                return RedirectToAction("SwitchCompany", "Home");
            }

            if (ModelState.IsValid)
            {
                payrollTmpHeaderEntity.CompanyId = currentCompanyId.Value;
                _context.Add(payrollTmpHeaderEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            if (_context?.PaymentGroups == null)
            {
                ViewData["PaymentGroupId"] = new SelectList(Enumerable.Empty<PaymentGroupEntity>(), "Id", "Name", payrollTmpHeaderEntity.PaymentGroupId);
            }
            else
            {
                ViewData["PaymentGroupId"] = new SelectList(FilterByCurrentCompany(_context.PaymentGroups), "Id", "Name", payrollTmpHeaderEntity.PaymentGroupId);
            }

            return View(payrollTmpHeaderEntity);
        }

        // GET: PayrollTmpHeaders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var redirect = EnsureCompanySelected();
            if (redirect != null) return redirect;

            if (id == null)
            {
                return NotFound();
            }

            var payrollTmpHeaderEntity = await FilterByCurrentCompany(_context.PayrollTmpHeaders).FirstOrDefaultAsync(e => e.Id == id);
            if (payrollTmpHeaderEntity == null)
            {
                return NotFound();
            }
            ViewData["PaymentGroupId"] = new SelectList(FilterByCurrentCompany(_context.PaymentGroups), "Id", "Name", payrollTmpHeaderEntity.PaymentGroupId);
            return View(payrollTmpHeaderEntity);
        }

        // POST: PayrollTmpHeaders/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PayrollTmpHeaderEntity payrollTmpHeaderEntity)
        {
            if (id != payrollTmpHeaderEntity.Id)
            {
                return NotFound();
            }

            // Verify the entity belongs to the current company
            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue || payrollTmpHeaderEntity.CompanyId != currentCompanyId.Value)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payrollTmpHeaderEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!payrollTmpHeaderEntityExists(payrollTmpHeaderEntity.Id))
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
            ViewData["PaymentGroupId"] = new SelectList(FilterByCurrentCompany(_context.PaymentGroups), "Id", "Name", payrollTmpHeaderEntity.PaymentGroupId);
            ViewData["PayrollTypeId"] = new SelectList(_context.PayrollTypes, "Id", "Name", payrollTmpHeaderEntity.PayrollTypeId);

            return View(payrollTmpHeaderEntity);
        }

        // GET: PayrollTmpHeaders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var redirect = EnsureCompanySelected();
            if (redirect != null) return redirect;

            if (id == null)
            {
                return NotFound();
            }

            var payrollTmpHeaderEntity = await FilterByCurrentCompany(_context.PayrollTmpHeaders
                .Include(p => p.PaymentGroup))
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payrollTmpHeaderEntity == null)
            {
                return NotFound();
            }

            return View(payrollTmpHeaderEntity);
        }

        // POST: PayrollTmpHeaders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payrollTmpHeaderEntity = await FilterByCurrentCompany(_context.PayrollTmpHeaders).FirstOrDefaultAsync(e => e.Id == id);
            if (payrollTmpHeaderEntity != null)
            {
                _context.PayrollTmpHeaders.Remove(payrollTmpHeaderEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool payrollTmpHeaderEntityExists(int id)
        {
            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
                return false;

            return _context.PayrollTmpHeaders.Any(e => e.Id == id && e.CompanyId == currentCompanyId.Value);
        }

        private async Task<PayrollTmpHeaderEntity?> GetFullPayrollHeader(int id)
        {
            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
                return null;

            var payrollTmpHeaderEntity = await _context.PayrollTmpHeaders
                .Where(p => p.Id == id && p.CompanyId == currentCompanyId.Value)
                .Include(p => p.PaymentGroup)
                .Include(p => p.Employees)
                    .ThenInclude(e => e.PayrollTmpConcepts)
                        .ThenInclude(pc => pc.PaymentConcept)
                .Include(p => p.Employees)
                    .ThenInclude(e => e.PayrollTmpLegalDeductions)
                        .ThenInclude(ld => ld.LegalDeduction)
                .Include(p => p.PayrollTmpLegalDeductions)
                    .ThenInclude(ld => ld.LegalDeduction)
                .Include(p => p.Employees)
                    .ThenInclude(emp => emp.Employee)
                .Include(p => p.Employees)
                    .ThenInclude(emp => emp.PayrollTmpOvertime)
                .FirstOrDefaultAsync();

            if (payrollTmpHeaderEntity == null)
            {
                return null;
            }

            // Load HistoricTmpLiabilities for each employee separately to avoid EF translation issues
            foreach (var emp in payrollTmpHeaderEntity.Employees)
            {
                emp.HistoricTmpLiabilities = await _context.HistoricTmpLiabilities
                    .Where(h => h.PayrollTmpHeaderId == id && h.PayrollTmpEmployeeId == emp.Id)
                    .Include(hl => hl.Liability)
                        .ThenInclude(l => l.Creditor)
                    .Include(h => h.PayrollTmpEmployee)
                    .ToListAsync();
            }

            return payrollTmpHeaderEntity;
        }

        [HttpPost]
        public async Task<IActionResult> GeneratePayRoll(PayrollHeaderViewModel model, bool isContractor = false)
        {
            // Add logging to debug form submission
            Console.WriteLine($"[DEBUG] GeneratePayRoll: Action method called");
            Console.WriteLine($"[DEBUG] GeneratePayRoll: Model is null: {model == null}");

            if (model != null)
            {
                Console.WriteLine($"[DEBUG] GeneratePayRoll: PaymentGroupId: {model.PaymentGroupId}");
                Console.WriteLine($"[DEBUG] GeneratePayRoll: PayrollTypeId: {model.PayrollTypeId}");
                Console.WriteLine($"[DEBUG] GeneratePayRoll: StartDate: {model.StartDate}");
                Console.WriteLine($"[DEBUG] GeneratePayRoll: EndDate: {model.EndDate}");
                Console.WriteLine($"[DEBUG] GeneratePayRoll: SelectedEmployeeIds count: {model.SelectedEmployeeIds?.Count ?? 0}");
                if (model.SelectedEmployeeIds != null)
                {
                    for (int i = 0; i < model.SelectedEmployeeIds.Count; i++)
                    {
                        Console.WriteLine($"[DEBUG] GeneratePayRoll: SelectedEmployeeIds[{i}]: {model.SelectedEmployeeIds[i]}");
                    }
                }
                Console.WriteLine($"[DEBUG] GeneratePayRoll: Notes: {model.Notes}");
                Console.WriteLine($"[DEBUG] GeneratePayRoll: ModelState.IsValid: {ModelState.IsValid}");

                if (!ModelState.IsValid)
                {
                    foreach (var key in ModelState.Keys)
                    {
                        var errors = ModelState[key].Errors;
                        if (errors.Any())
                        {
                            Console.WriteLine($"[DEBUG] GeneratePayRoll: ModelState error for {key}: {string.Join(", ", errors.Select(e => e.ErrorMessage))}");
                        }
                    }
                }
            }

            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de continuar.";
                return RedirectToAction("SwitchCompany", "Home");
            }

            if (model == null)
            {
                return NotFound("No se puede procesar por que no se han seleccionado datos.");
            }

            //Guardamos el Encabezado
            var payrollTmpHeader = new PayrollTmpHeaderEntity
            {
                Created = DateTime.UtcNow,
                AbsensestDateEnd = model.AbsensestDateEnd,
                AbsensestDateStart = model.AbsensestDateStart,
                ExtraTimeDateEnd = model.ExtraTimeDateEnd,
                ExtraTimeDateStart = model.ExtraTimeDateStart,
                Notes = model.Notes,
                PaymentGroupId = model.PaymentGroupId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                PaymentGroup = await FilterByCurrentCompany(_context.PaymentGroups.Include(pg => pg.PaymentFrequency)).FirstOrDefaultAsync(pg => pg.Id == model.PaymentGroupId),
                PayrollTypeId = model.PayrollTypeId,
                CompanyId = currentCompanyId.Value
            };

            // Save the PayrollHeader first to ensure its Id is generated
            await _context.PayrollTmpHeaders.AddAsync(payrollTmpHeader);
            await _context.SaveChangesAsync();

            //Validamos los conceptos predeterminados que deben ser incluidos
            var paymentConcepts = await _context.PaymentConcepts.Where(pc => pc.IsPredetermined).ToListAsync();

            if (paymentConcepts == null || !paymentConcepts.Any())
            {
                return NotFound("No hay ingresos predeterminados");
            }

            var QDescFreq = 0;
            //Validamos la frecuencia de los descuentos de acreedores
            QDescFreq = (payrollTmpHeader.PaymentGroup?.PaymentFrequency?.QuantityOfDays) switch
            {
                7 => 30 / 7,
                14 => 30 / 14,
                15 => 30 / 15,
                30 => 30 / 30,
                _ => 1,
            };

            //Validamos los empleados que fueron seleccionados (filtrar por tipo de empleado)
            foreach (var employeeId in model.SelectedEmployeeIds)
            {
                var employeeData = await _context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId && e.IsContractor == isContractor);

                if (employeeData == null)
                {
                    // Suponiendo que un empleado no exista, pero el bucle debe continuar
                    continue;
                }

                var payrollTmpEmployee = new PayrollTmpEmployeeEntity
                {
                    Created = DateTime.UtcNow,
                    EmployeeId = employeeData.Id,
                    AgreeSalary = employeeData.AgreeSalary,
                    PayrollTmpHeaderId = payrollTmpHeader.Id,
                    NetPay = 0.00m,
                    TotalDeductions = 0.00m,
                    TotalLegalDiscount = 0.00m,
                    TotalOtherDiscount = 0.00m,
                    TotalEarnings = 0.00m,
                    HourlySalary = employeeData.HourSalary,
                    RegularHours = employeeData.RegularHours,
                    RegularSalary = employeeData.RegularHours * employeeData.HourSalary
                };

                // Trae producción destajo en el rango de la planilla
                if (employeeData.SalaryType == "Destajo")
                {
                    var destajoEntries = await _context.DestajoProductions
                        .Include(dp => dp.Document)
                        .Where(dp => dp.EmployeeId == employeeData.Id
                                     && dp.Document != null
                                     && dp.Document.DocumentDate >= payrollTmpHeader.StartDate
                                     && dp.Document.DocumentDate <= payrollTmpHeader.EndDate)
                        .ToListAsync();

                    if (destajoEntries.Any())
                    {
                        var totalUnits = destajoEntries.Sum(d => d.UnitsProduced);
                        var totalAmount = destajoEntries.Sum(d => d.TotalAmount);
                        payrollTmpEmployee.UnitsProduced = totalUnits;
                        payrollTmpEmployee.UnitValue = employeeData.UnitValue;
                        payrollTmpEmployee.DestajoAmount = totalAmount;
                    }
                }

                await _context.PayrollTmpEmployees.AddAsync(payrollTmpEmployee);
                await _context.SaveChangesAsync(); // Se guarda el empleado para obtener su ID

                // AQUI se mueven los conceptos para que se asocien a cada empleado
                foreach (var concept in paymentConcepts)
                {
                    decimal hours = 0.00m;
                    decimal totalAmount = 0.00m;
                    decimal unitAmount = 0.00m;

                    if (concept.Name == "Salario Regular")
                    {
                        if (employeeData.SalaryType == "Destajo")
                        {
                            continue;
                        }
                        hours = employeeData.RegularHours;
                        totalAmount = employeeData.RegularHours * employeeData.HourSalary;
                        unitAmount = employeeData.HourSalary;
                    }

                    await _context.PayrollTmpConcepts.AddAsync(
                            new PayrollTmpConceptEntity
                            {
                                Created = DateTime.UtcNow,
                                PaymentConceptId = concept.Id,
                                PayrollTmpEmployeeId = payrollTmpEmployee.Id,
                                TotalAmount = totalAmount,
                                Hours = hours,
                                PayFactor = concept.PayFactor,
                                PayrollTmpHeaderId = payrollTmpHeader.Id,
                                UnitAmount = unitAmount
                            }
                        );
                }

                if (employeeData.SalaryType == "Destajo" && payrollTmpEmployee.DestajoAmount > 0)
                {
                    var destajoConcept = await EnsureDestajoConceptAsync(payrollTmpHeader.CompanyId);
                    if (destajoConcept != null)
                    {
                        await _context.PayrollTmpConcepts.AddAsync(
                            new PayrollTmpConceptEntity
                            {
                                Created = DateTime.UtcNow,
                                PaymentConceptId = destajoConcept.Id,
                                PayrollTmpEmployeeId = payrollTmpEmployee.Id,
                                TotalAmount = payrollTmpEmployee.DestajoAmount,
                                Hours = payrollTmpEmployee.UnitsProduced,
                                PayFactor = destajoConcept.PayFactor,
                                PayrollTmpHeaderId = payrollTmpHeader.Id,
                                UnitAmount = payrollTmpEmployee.UnitValue
                            }
                        );
                    }
                }

                //Revisamos las Deudas con Acreedor que cada empleato tiene vigente
                var activeLibilities = await _context.Liabilities.Where(l => l.EmployeeId == employeeData.Id && l.Status == "Activo").ToListAsync();

                if (activeLibilities.Count > 0)
                {
                    foreach (var liability in activeLibilities)
                    {
                        var liabilityEmployee = new HistoricTmpLiabilityEntity
                        {
                            AmountToDiscount = liability.Dicsount / QDescFreq,
                            Created = DateTime.UtcNow,
                            PayrollTmpEmployeeId = payrollTmpEmployee.Id,
                            LiabilityId = liability.Id,
                            PayrollTmpHeaderId = payrollTmpHeader.Id,
                        };

                        await _context.HistoricTmpLiabilities.AddAsync(liabilityEmployee);
                        await _context.SaveChangesAsync();
                    }
                 }
             }

             // Calcular nóminas para todos los empleados después de agregar todas las entidades relacionadas
             var employees = await _context.PayrollTmpEmployees
                 .Where(e => model.SelectedEmployeeIds.Contains(e.Id) && e.PayrollTmpHeaderId == payrollTmpHeader.Id)
                 .ToListAsync();

             foreach (var employee in employees)
             {
                 await CalculateEmployeePayroll(employee.Id, payrollTmpHeader);
             }

             try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "PayrollTmpHeaders", new { id = payrollTmpHeader.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(500, "Ocurrió un error al guardar los datos.");
            }
        }

        private async Task<PaymentTmpConceptsViewModel?> GetConceptsByEmployeeViewModel(int employeeId)
        {
            var employee = await _context.PayrollTmpEmployees
                .Include(e => e.PayrollTmpConcepts)
                    .ThenInclude(pc => pc.PaymentConcept)
                .Include(e => e.PayrollTmpHeader)
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null) return null;

            var companyCountryId = employee.PayrollTmpHeader != null
                ? await GetCompanyCountryIdAsync(employee.PayrollTmpHeader.CompanyId)
                : null;

            var legalDeductions = await LoadApplicableTmpLegalDeductionsAsync(employeeId, employee.PayrollTmpHeaderId, companyCountryId);

            var liabilities = await _context.HistoricTmpLiabilities
                .Include(l => l.PayrollTmpEmployee)
                .Include(l => l.Liability)
                    .ThenInclude(l => l.Creditor)
                .Where(l => l.PayrollTmpEmployeeId == employeeId)
                .ToListAsync();

            var overtimeSegments = await _context.PayrollTmpOvertime
                .Where(o => o.PayrollTmpEmployeeId == employeeId)
                .OrderBy(o => o.OvertimeDate)
                .ThenBy(o => o.EntryTime)
                .ThenBy(o => o.FactorCode)
                .ToListAsync();

            var vm = new PaymentTmpConceptsViewModel
            {
                Concepts = employee.PayrollTmpConcepts.Where(c => c.PaymentConcept?.Code != "SBTM0001"),
                LegalDeductions = legalDeductions,
                Liabilities = liabilities,
                Overtime = overtimeSegments
            };

            // Aquí llenamos el ViewBag.
            ViewBag.EmployeeId = employeeId;
            ViewBag.PaymentConcepts = await _context.PaymentConcepts
                                                    .OrderBy(p => p.Name)
                                                    .ToListAsync();
            return vm;
        }

        public async Task<IActionResult> GetConceptsByEmployee(int employeeId, bool partial = false)
        {
            var vm = await GetConceptsByEmployeeViewModel(employeeId);
            if (vm == null) return Content("<p>No se encontró información del empleado.</p>");

            // If explicitly requested as partial or AJAX request
            if (partial || Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_EmployeeConcepts", vm);
            }
            else
            {
                // Return full view for direct navigation
                return View("_EmployeeConcepts", vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> AddConcept(PayrollTmpConceptEntity concept)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos");

            _context.PayrollTmpConcepts.Add(concept);
            await _context.SaveChangesAsync();

            var payrollTmpHeader = await _context.PayrollTmpHeaders
                                        .FirstOrDefaultAsync(ph => ph.Id == concept.PayrollTmpHeaderId);

            // Recalculamos la nómina del empleado después de agregar el concepto
            if (payrollTmpHeader != null)
            {
                await RecalculateDraftEmployeeAsync(concept.PayrollTmpEmployeeId, payrollTmpHeader);
            }

            var vm = await GetConceptsByEmployeeViewModel(concept.PayrollTmpEmployeeId);

            // Check if this is an AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_EmployeeConcepts", vm);
            }
            else
            {
                // If not AJAX, redirect back to Details
                return RedirectToAction("Details", new { id = concept.PayrollTmpHeaderId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConcept(int conceptId)
        {
            // If conceptId is 0, try to get it from form data
            if (conceptId == 0)
            {
                var formConceptId = Request.Form["conceptId"].ToString();
                if (!string.IsNullOrEmpty(formConceptId) && int.TryParse(formConceptId, out int parsedId))
                {
                    conceptId = parsedId;
                }
            }

            var concept = await _context.PayrollTmpConcepts.FindAsync(conceptId);
            if (concept == null)
            {
                return NotFound(new { success = false, message = "Concepto no encontrado" });
            }

            _context.PayrollTmpConcepts.Remove(concept);
            await _context.SaveChangesAsync();

            var payrollTmpHeader = await _context.PayrollTmpHeaders
                                        .FirstOrDefaultAsync(ph => ph.Id == concept.PayrollTmpHeaderId);

            // Recalculamos la nómina del empleado después de eliminar el concepto
            if (payrollTmpHeader != null)
            {
                await RecalculateDraftEmployeeAsync(concept.PayrollTmpEmployeeId, payrollTmpHeader);
            }

            return Json(new { success = true, message = "Concepto eliminado correctamente" });
        }

        public async Task<IActionResult> GetConceptForm(int employeeId)
        {
            var model = new ConceptFormViewModel
            {
                EmployeeId = employeeId,
                ConceptList = await _context.PaymentConcepts.OrderBy(c => c.Name).ToListAsync()
            };

            return PartialView("_ConceptFormFields", model);
        }

        [HttpGet]
        public IActionResult GetPaymentConcepts()
        {
            var concepts = _context.PaymentConcepts
                                .Select(pc => new { pc.Id, pc.Name })
                                .ToList();

            return Json(concepts);
        }

        [HttpGet]
        public async Task<IActionResult> CalculatePayroll(int id)
        {
            // 1. Obtiene la planilla y todos los empleados, conceptos y deducciones relacionados
            var payrollHeader = await _context.PayrollTmpHeaders
                .Include(p => p.Employees)
                .Include(e => e.PayrollTmpConcepts)
                    .ThenInclude(e => e.PaymentConcept)
                .Include(p => p.Employees)
                .Include(e => e.PayrollTmpLegalDeductions)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payrollHeader == null)
            {
                return NotFound("No se proporcionó un Id de planilla correcto.") ;
            }

            await SyncDraftLegalDeductionsAsync(payrollHeader);

            decimal payrollTotalGrossPay = 0;
            decimal payrollTotalLegalDeductions = 0;
            decimal payrollTotalOtherDeductions = 0;
            decimal payrollTotalNetPay = 0;

            // 2. Itera sobre cada empleado para realizar los cálculos individuales
            foreach (var employee in payrollHeader.Employees)
            {
                await CalculateEmployeePayroll(employee.Id, payrollHeader);
            }

            // Recargar empleados con los valores actualizados
            var updatedEmployees = await _context.PayrollTmpEmployees
                .Where(e => e.PayrollTmpHeaderId == id)
                .ToListAsync();

            // 3. Acumular totales de la planilla
            foreach (var employee in updatedEmployees)
            {
                payrollTotalGrossPay += employee.TotalEarnings;
                payrollTotalLegalDeductions += employee.TotalLegalDiscount;
                payrollTotalOtherDeductions += employee.TotalOtherDiscount;
                payrollTotalNetPay += employee.NetPay;
            }

            TempData["PayrollDraftMessage"] = $"Borrador generado correctamente para la planilla #{payrollHeader.Id}. Se sincronizaron y recalcularon los descuentos legales aplicables.";

            await _context.SaveChangesAsync();
            return RedirectToAction("PayrollReport", "PayrollTmpHeaders", new { id = payrollHeader.Id });
        }

        private async Task SyncDraftLegalDeductionsAsync(PayrollTmpHeaderEntity payrollHeader)
        {
            var payrollEmployeeIds = payrollHeader.Employees?.Select(e => e.Id).ToList() ?? new List<int>();
            if (payrollEmployeeIds.Count == 0)
            {
                payrollEmployeeIds = await _context.PayrollTmpEmployees
                    .Where(e => e.PayrollTmpHeaderId == payrollHeader.Id)
                    .Select(e => e.Id)
                    .ToListAsync();
            }

            var companyCountryId = await GetCompanyCountryIdAsync(payrollHeader.CompanyId);
            var applicableDeductionIds = await GetApplicableLegalDeductionIdsAsync(payrollHeader.PayrollTypeId, companyCountryId);

            var existingTmpDeductions = await _context.PayrollTmpLegalDeductions
                .Where(ld => ld.PayrollTmpHeaderId == payrollHeader.Id)
                .ToListAsync();

            var deductionsToRemove = existingTmpDeductions
                .Where(ld => !applicableDeductionIds.Contains(ld.LegalDeductionId))
                .ToList();

            if (deductionsToRemove.Count > 0)
            {
                _context.PayrollTmpLegalDeductions.RemoveRange(deductionsToRemove);
            }

            foreach (var payrollTmpEmployeeId in payrollEmployeeIds)
            {
                var employeeDeductionIds = existingTmpDeductions
                    .Where(ld => ld.PayrollTmpEmployeeId == payrollTmpEmployeeId)
                    .Select(ld => ld.LegalDeductionId)
                    .ToHashSet();

                var missingDeductionIds = applicableDeductionIds
                    .Where(id => !employeeDeductionIds.Contains(id));

                foreach (var deductionId in missingDeductionIds)
                {
                    await _context.PayrollTmpLegalDeductions.AddAsync(new PayrollTmpLegalDeductionEntity
                    {
                        LegalDeductionId = deductionId,
                        Created = DateTime.UtcNow,
                        PayrollTmpEmployeeId = payrollTmpEmployeeId,
                        PayrollTmpHeaderId = payrollHeader.Id,
                        Amount = 0m
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task RecalculateDraftEmployeeAsync(int payrollTmpEmployeeId, PayrollTmpHeaderEntity payrollHeader)
        {
            await SyncDraftLegalDeductionsAsync(payrollHeader);
            await CalculateEmployeePayroll(payrollTmpEmployeeId, payrollHeader);
            await _context.SaveChangesAsync();
        }

        private async Task<int?> GetCompanyCountryIdAsync(int companyId)
        {
            return await _context.Companies
                .Where(c => c.Id == companyId)
                .Select(c => c.CountryId)
                .FirstOrDefaultAsync();
        }

        private async Task<List<int>> GetApplicableLegalDeductionIdsAsync(int payrollTypeId, int? companyCountryId)
        {
            var query = _context.LegalDeductionEntity
                .Where(l => l.IsActive
                    && l.DeducFromPayroll
                    && l.PayrollTypeId == payrollTypeId);

            if (companyCountryId.HasValue)
            {
                query = query.Where(l => l.CountryId == companyCountryId.Value || l.CountryId == null);
            }

            return await query
                .Select(l => l.Id)
                .ToListAsync();
        }

        private async Task<List<PayrollTmpLegalDeductionEntity>> LoadApplicableTmpLegalDeductionsAsync(int payrollTmpEmployeeId, int payrollTmpHeaderId, int? companyCountryId)
        {
            var query = _context.PayrollTmpLegalDeductions
                .Include(ld => ld.PayrollTmpEmployee)
                .Include(ld => ld.LegalDeduction)
                .Where(ld => ld.PayrollTmpEmployeeId == payrollTmpEmployeeId
                    && ld.PayrollTmpHeaderId == payrollTmpHeaderId
                    && ld.LegalDeduction != null
                    && ld.LegalDeduction.DeducFromPayroll);

            if (companyCountryId.HasValue)
            {
                query = query.Where(ld => ld.LegalDeduction != null
                    && (ld.LegalDeduction.CountryId == companyCountryId.Value || ld.LegalDeduction.CountryId == null));
            }

            return await query.ToListAsync();
        }

        public async Task<IActionResult> PayrollReport(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payrollTmpHeaderEntity = await GetFullPayrollHeader(id.Value);
            if (payrollTmpHeaderEntity == null)
            {
                return NotFound();
            }
            if (payrollTmpHeaderEntity?.Employees?.FirstOrDefault()?.HistoricTmpLiabilities.Count == 0)
            {
                foreach (var emp in payrollTmpHeaderEntity.Employees)
                {
                    List<HistoricTmpLiabilityEntity> historicLiabities = null;

                    historicLiabities = await _context.HistoricTmpLiabilities
                                        .Include(h => h.Liability)
                                            .ThenInclude(l => l.Creditor)
                                        .Where(h => h.PayrollTmpEmployeeId == emp.Id && h.PayrollTmpHeaderId == payrollTmpHeaderEntity.Id)
                                        .ToListAsync();

                    emp.HistoricTmpLiabilities = historicLiabities;
                }
            }

            if (payrollTmpHeaderEntity?.Employees.FirstOrDefault()?.PayrollTmpLegalDeductions.Count == 0)
            {
                List<PayrollTmpLegalDeductionEntity> legalDeductions = null;
                foreach (var emp in payrollTmpHeaderEntity.Employees)
                {
                    legalDeductions = await _context.PayrollTmpLegalDeductions
                                        .Include(ld => ld.LegalDeduction)
                                        .Where(ld => ld.PayrollTmpEmployeeId == emp.Id && ld.PayrollTmpHeaderId == payrollTmpHeaderEntity.Id)
                                        .ToListAsync();

                    emp.PayrollTmpLegalDeductions = legalDeductions;
                }   
            }

            return View(payrollTmpHeaderEntity);
        }

        public async Task<IActionResult> EmployeeVoucher(int employeeId, int payrollId)
        {
            var employee = await _context.PayrollTmpEmployees
                .Include(e => e.Employee)
                .Include(e => e.PayrollTmpConcepts)
                    .ThenInclude(pc => pc.PaymentConcept)
                .Include(e => e.PayrollTmpLegalDeductions)
                    .ThenInclude(ld => ld.LegalDeduction)
                .Include(e => e.HistoricTmpLiabilities)
                    .ThenInclude(h => h.Liability)
                        .ThenInclude(l => l.Creditor)
                .Include(e => e.PayrollTmpOvertime)
                .Include(e => e.PayrollTmpHeader)
                    .ThenInclude(ph => ph.PaymentGroup)
                .FirstOrDefaultAsync(e => e.Id == employeeId && e.PayrollTmpHeaderId == payrollId);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        public async Task<IActionResult> ExportToPdf(int id)
        {
            var payrollTmpHeaderEntity = await _context.PayrollTmpHeaders
                .Include(p => p.PaymentGroup)
                .Include(p => p.Employees)
                    .ThenInclude(e => e.Employee)
                .Include(p => p.Employees)
                    .ThenInclude(e => e.PayrollTmpConcepts)
                        .ThenInclude(pc => pc.PaymentConcept)
                .Include(p => p.Employees)
                    .ThenInclude(e => e.PayrollTmpLegalDeductions)
                        .ThenInclude(ld => ld.LegalDeduction)
                .Include(p => p.Employees)
                    .ThenInclude(e => e.HistoricTmpLiabilities)
                        .ThenInclude(h => h.Liability)
                            .ThenInclude(l => l.Creditor)
                .Include(p => p.Employees)
                    .ThenInclude(e => e.PayrollTmpOvertime)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (payrollTmpHeaderEntity == null)
            {
                return NotFound();
            }

            using var stream = new System.IO.MemoryStream();
            var pdfWriter = new PdfWriter(stream);
            var pdfDocument = new PdfDocument(pdfWriter);
            var document = new Document(pdfDocument);

            document.Add(new Paragraph("Reporte de Nómina")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(20));

            document.Add(new Paragraph($"{payrollTmpHeaderEntity.PaymentGroup?.Name} - {payrollTmpHeaderEntity.StartDate:dd/MM/yyyy} - {payrollTmpHeaderEntity.EndDate:dd/MM/yyyy}")
                .SetTextAlignment(TextAlignment.CENTER));

            foreach (var employee in payrollTmpHeaderEntity.Employees)
            {
                document.Add(new Paragraph($"Empleado: {employee.Employee?.FullName}")
                    .SetFontSize(14));

                document.Add(new Paragraph($"Cédula: {employee.Employee?.IdDocument}"));

                // Ingresos
                document.Add(new Paragraph("Ingresos:").SetFontSize(12));
                var table = new Table(UnitValue.CreatePercentArray(new float[] { 3, 1, 2 })).UseAllAvailableWidth();
                table.AddHeaderCell("Concepto");
                table.AddHeaderCell("Horas");
                table.AddHeaderCell("Monto");

                foreach (var concept in employee.PayrollTmpConcepts)
                {
                    table.AddCell(concept.PaymentConcept?.Name ?? "");
                    table.AddCell(concept.Hours.ToString());
                    table.AddCell(concept.TotalAmount.ToString("C"));
                }

                // Add overtime details only if SBTM0001 concept is not present
                bool hasOvertimeConcept = employee.PayrollTmpConcepts.Any(c => c.PaymentConcept?.Code == "SBTM0001");

                if (!hasOvertimeConcept && employee.PayrollTmpOvertime.Any())
                {
                    var overtimeGroups = employee.PayrollTmpOvertime
                        .GroupBy(o => new { o.OvertimeDate, o.EntryTime, o.ExitTime });

                    foreach (var group in overtimeGroups)
                    {
                        var firstOvertime = group.First();
                        var totalHours = group.Sum(o => o.CalculatedHours);
                        var totalAmount = group.Sum(o => o.TotalAmount);

                        table.AddCell($"{firstOvertime.OvertimeDate:dd/MM/yyyy} {firstOvertime.EntryTime:hh\\:mm}-{firstOvertime.ExitTime:hh\\:mm}");
                        table.AddCell(totalHours.ToString("N2"));
                        table.AddCell(totalAmount.ToString("C"));

                        // Add factor breakdown as additional rows
                        foreach (var segment in group.OrderBy(s => s.FactorCode))
                        {
                            table.AddCell($"  └ {segment.FactorCode}");
                            table.AddCell(segment.CalculatedHours.ToString("N2"));
                            table.AddCell(segment.TotalAmount.ToString("C"));
                        }
                    }
                }

                table.AddCell(new Cell(1, 2).Add(new Paragraph("Total Ingresos")).SetTextAlignment(TextAlignment.RIGHT));
                table.AddCell(employee.TotalEarnings.ToString("C"));

                document.Add(table);

                // Deducciones
                document.Add(new Paragraph("Deducciones:").SetFontSize(12));
                var deductionTable = new Table(UnitValue.CreatePercentArray(new float[] { 2, 3, 2 })).UseAllAvailableWidth();
                deductionTable.AddHeaderCell("Tipo");
                deductionTable.AddHeaderCell("Descripción");
                deductionTable.AddHeaderCell("Monto");

                var legalDeductions = employee.PayrollTmpLegalDeductions
                    .Where(ld => ld.Amount > 0)
                    .OrderBy(ld => ld.LegalDeduction!.Name)
                    .ToList();

                var creditorDiscounts = employee.HistoricTmpLiabilities
                    .Where(liability => liability.DiscountedAmount > 0)
                    .OrderBy(liability => liability.Liability!.Creditor!.Name)
                    .ThenBy(liability => liability.Liability!.Code)
                    .ToList();

                if (legalDeductions.Any())
                {
                    deductionTable.AddCell(new Cell(1, 3)
                        .Add(new Paragraph("Deducciones legales aplicadas")));

                    foreach (var legalDeduction in legalDeductions)
                    {
                        deductionTable.AddCell("Legal");
                        deductionTable.AddCell($"{legalDeduction.LegalDeduction?.Code} - {legalDeduction.LegalDeduction?.Name}");
                        deductionTable.AddCell(legalDeduction.Amount.ToString("C"));
                    }
                }

                deductionTable.AddCell(new Cell(1, 2).Add(new Paragraph("Total deducciones legales")).SetTextAlignment(TextAlignment.RIGHT));
                deductionTable.AddCell(employee.TotalLegalDiscount.ToString("C"));

                if (creditorDiscounts.Any())
                {
                    deductionTable.AddCell(new Cell(1, 3)
                        .Add(new Paragraph("Descuentos de acreedores")));

                    foreach (var liability in creditorDiscounts)
                    {
                        deductionTable.AddCell("Acreedor");
                        deductionTable.AddCell($"{liability.Liability?.Code} - {liability.Liability?.Creditor?.Name}");
                        deductionTable.AddCell(liability.DiscountedAmount.ToString("C"));
                    }
                }

                deductionTable.AddCell(new Cell(1, 2).Add(new Paragraph("Total acreedores")).SetTextAlignment(TextAlignment.RIGHT));
                deductionTable.AddCell(employee.TotalOtherDiscount.ToString("C"));

                deductionTable.AddCell(new Cell(1, 2).Add(new Paragraph("Total Deducciones")).SetTextAlignment(TextAlignment.RIGHT));
                deductionTable.AddCell(employee.TotalDeductions.ToString("C"));

                document.Add(deductionTable);

                document.Add(new Paragraph($"Salario Neto: {employee.NetPay:C}")
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetFontSize(12));

                document.Add(new Paragraph(" ").SetMarginBottom(8));
            }

            document.Close();

            return File(stream.ToArray(), "application/pdf", $"Nomina_{payrollTmpHeaderEntity.Id}.pdf");
        }

        private async Task CalculateEmployeePayroll(int id, PayrollTmpHeaderEntity payrollHeader)
        {
            var employee = await _context.PayrollTmpEmployees.Where(e => e.Id == id)
                                .Include(e => e.PayrollTmpConcepts)
                                    .ThenInclude(pc => pc.PaymentConcept)
                                .Include(e => e.PayrollTmpOvertime)
                                .Include(e => e.HistoricTmpLiabilities)
                                    .ThenInclude(hl => hl.Liability)
                                        .ThenInclude(l => l.Creditor)
                                .Include(e => e.PayrollTmpHeader)
                                .FirstOrDefaultAsync(e => e.Id == id && e.PayrollTmpHeaderId == payrollHeader.Id);

            if (employee == null) return;

            if (employee.PayrollTmpHeader == null) return;

            // Load legal deductions separately to ensure they are loaded correctly
            var companyCountryId = await GetCompanyCountryIdAsync(payrollHeader.CompanyId);
            var employeeLegalDeductions = await LoadApplicableTmpLegalDeductionsAsync(employee.Id, payrollHeader.Id, companyCountryId);

            var employeeLiabilities = employee.HistoricTmpLiabilities.ToList();

            // Inicializa los totales
            decimal totalGrossPay = 0;
            decimal totalLegalDeductions = 0;
            decimal totalOtherDeductions = 0;
            decimal totalNetPay = 0;
            decimal totalOvertimePay = employee.PayrollTmpOvertime.Sum(o => o.TotalAmount);
            bool hasOvertimeConcept = employee.PayrollTmpConcepts.Any(c => c.PaymentConcept?.Code == "SBTM0001");

            // Suma el salario bruto de todos los conceptos (ya calculados)
            foreach (var concept in employee.PayrollTmpConcepts)
            {
                totalGrossPay += concept.TotalAmount;
            }

            if (!hasOvertimeConcept)
            {
                totalGrossPay += totalOvertimePay;
            }

            totalNetPay = totalGrossPay;

            // Calcula los Descuentos Legales basados en PayrollTmpLegalDeductions
            foreach (var tmpLegal in employeeLegalDeductions)
            {
                decimal legalDiscount = 0;

                // Special calculation for IMPSRTA (Impuesto Sobre La Renta)
                if (tmpLegal.LegalDeduction?.Code == "IMPSRTA")
                {
                    legalDiscount = await CalculateIMPSRTADeductionAsync(employee, payrollHeader, totalGrossPay);
                }
                else
                {
                    // Default percentage calculation for other legal deductions
                    legalDiscount = totalGrossPay * ((tmpLegal.LegalDeduction?.EmployeeDiscount ?? 0) / 100);
                }

                tmpLegal.Amount = legalDiscount;
                totalLegalDeductions += legalDiscount;
                _context.PayrollTmpLegalDeductions.Update(tmpLegal);
            }

            totalNetPay -= totalLegalDeductions;

            // Calcula otros descuentos basados en HistoricTmpLiabilities
            foreach (var historyDeduction in employeeLiabilities)
            {
                decimal discountAmount = 0;
                if (totalNetPay > historyDeduction.AmountToDiscount)
                {
                    discountAmount = historyDeduction.AmountToDiscount;
                }
                else
                {
                    discountAmount = totalNetPay;
                }
                historyDeduction.DiscountedAmount = discountAmount;
                totalOtherDeductions += discountAmount;
                _context.HistoricTmpLiabilities.Update(historyDeduction);
            }

            totalNetPay -= totalOtherDeductions;

            // Actualiza las propiedades del empleado
            employee.OverTimeSalary = totalOvertimePay;
            employee.TotalEarnings = totalGrossPay;
            employee.TotalLegalDiscount = totalLegalDeductions;
            employee.TotalOtherDiscount = totalOtherDeductions;
            employee.TotalDeductions = totalLegalDeductions + totalOtherDeductions;
            employee.NetPay = totalNetPay;

            _context.PayrollTmpEmployees.Update(employee);
        }

        private async Task<decimal> CalculateIMPSRTADeductionAsync(PayrollTmpEmployeeEntity employee, PayrollTmpHeaderEntity payrollHeader, decimal currentTotalEarnings)
        {
            // Step 1: Sum historical deductions from PayrollLegalDeductions
            var historicalDeductions = _context.PayrollLegalDeductions
                .Include(pld => pld.PayrollEmployee)
                .Include(pld => pld.LegalDeduction)
                .Include(pld => pld.PayrollHeader)
                .Where(pld => pld.PayrollEmployee.EmployeeId == employee.EmployeeId &&
                             pld.LegalDeduction.Code == "IMPSRTA" &&
                             pld.PayrollHeader.EndDate < payrollHeader.EndDate)
                .Sum(pld => pld.Amount);
        
            decimal monthlyAverage;
            decimal divisor;

            var paymentGroup = await _context.PaymentGroups.FirstOrDefaultAsync(pg => pg.Id == employee.PayrollTmpHeader.PaymentGroupId) ?? throw new InvalidOperationException("Payment group not found.");
            if (historicalDeductions == 0)
            {
                //monthlyAverage = employee.AgreeSalary;
                switch (paymentGroup.QuantityOfDays)
                {
                    case 7:
                        monthlyAverage = ((employee.AgreeSalary / 4.33m) * 3) + currentTotalEarnings;
                        divisor = 52.14m ;
                        break;
                    case 14:
                        monthlyAverage = (employee.AgreeSalary / 2.14m) + currentTotalEarnings;
                        divisor = 26.07m ;
                        break;
                    case 15:
                        monthlyAverage = (employee.AgreeSalary / 2) + currentTotalEarnings;
                        divisor = 24.33m ;
                        break;
                    case 30:
                        monthlyAverage = currentTotalEarnings;
                        divisor = 12.17m; //365/30
                        break;
                    default:
                        monthlyAverage = employee.AgreeSalary;
                        divisor = 12.17m; //365/30
                        break;
                }
            }
            else
            {
                // Step 2: Add current total earnings (this represents payment concepts that pay ImpSRta)
                decimal totalIncome = historicalDeductions + currentTotalEarnings;

                // Step 3: Calculate months elapsed from Jan 1 to PayrollTmpHeaders.EndDate
                DateTime startOfYear = new DateTime(payrollHeader.EndDate.Year, 1, 1);
                int monthsElapsed = ((payrollHeader.EndDate.Year - startOfYear.Year) * 12) + payrollHeader.EndDate.Month - startOfYear.Month + 1;

                // Step 4: Total income perceived up to that date divided by months elapsed, projected to Dec 31
                monthlyAverage = totalIncome / monthsElapsed;

                // Assign divisor for the else branch
                int quantityOfDays = payrollHeader.PaymentGroup?.PaymentFrequency?.QuantityOfDays ?? 30;
                divisor = 395.0m / quantityOfDays;
            }
            //decimal projectedAnnualIncome = monthlyAverage * 12;
            decimal projectedAnnualIncome = monthlyAverage * 13;

            // Step 5: Subtract 11,000.00
            decimal taxableIncome = projectedAnnualIncome - 11000.00m;

            // Step 6: Calculate tax based on brackets
            decimal taxAmount = 0;
            if (taxableIncome > 0)
            {
                if (taxableIncome <= 50000.00m)
                {
                    // 15% for income up to 50,000
                    taxAmount = taxableIncome * 0.15m;
                }
                else
                {
                    // 15% for first 50,000
                    taxAmount = 50000.00m * 0.15m;
                    // 20% for the remaining amount
                    decimal remainingIncome = taxableIncome - 50000.00m;
                    taxAmount += remainingIncome * 0.20m;
                }
            }

            // Step 7: Divide by (365 / PaymentFrequencies.QuantityOfDays)
            // divisor is now always assigned

            decimal deductionAmount = taxAmount / divisor;

            return deductionAmount;
        }

        // OVERTIME METHODS

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOvertime()
        {
            var overtime = new PayrollTmpOvertimeEntity();

            // Manual parsing of all form fields
            if (int.TryParse(Request.Form["PayrollTmpEmployeeId"], out int employeeId))
                overtime.PayrollTmpEmployeeId = employeeId;

            if (int.TryParse(Request.Form["PayrollTmpHeaderId"], out int headerId))
                overtime.PayrollTmpHeaderId = headerId;

            if (DateTime.TryParse(Request.Form["OvertimeDate"], out DateTime overtimeDate))
                overtime.OvertimeDate = overtimeDate;

            if (int.TryParse(Request.Form["TypeOfDayId"], out int typeOfDayId))
                overtime.TypeOfDayId = typeOfDayId;

            if (int.TryParse(Request.Form["TypeOfWorkScheduleId"], out int typeOfWorkScheduleId))
                overtime.TypeOfWorkScheduleId = typeOfWorkScheduleId;

            var entryTimeStr = Request.Form["EntryTime"].ToString();
            var exitTimeStr = Request.Form["ExitTime"].ToString();

            bool TryParseTime(string value, out TimeSpan result)
            {
                result = default;
                if (string.IsNullOrWhiteSpace(value)) return false;

                var normalized = value.Trim();

                // Special case: allow 24:00 as midnight of next day
                if (normalized.Equals("24:00", StringComparison.OrdinalIgnoreCase))
                {
                    result = TimeSpan.FromHours(24);
                    return true;
                }

                if (TimeSpan.TryParse(normalized, out result)) return true;

                // Allow 12-hour inputs like "05:00 PM"
                if (DateTime.TryParse(normalized, CultureInfo.CurrentCulture, DateTimeStyles.None, out var dt) ||
                    DateTime.TryParse(normalized, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    result = dt.TimeOfDay;
                    return true;
                }

                return false;
            }

            var exitWas24 = exitTimeStr.Trim().Equals("24:00", StringComparison.OrdinalIgnoreCase);

            if (TryParseTime(entryTimeStr, out var parsedEntry) && TryParseTime(exitTimeStr, out var parsedExit))
            {
                // Store 24:00 as 00:00 on the entity but keep a flag for validation
                overtime.EntryTime = parsedEntry == TimeSpan.FromHours(24) ? TimeSpan.Zero : parsedEntry;
                overtime.ExitTime = exitWas24 ? TimeSpan.Zero : parsedExit;
            }
            else
            {
                return BadRequest(new { success = false, message = "Formato de hora inválido. Use formato HH:MM (ej: 17:00 o 5:00 PM)" });
            }

            // Validate that exit time is after entry time
            // Handle the case where exit time is 24:00 (midnight of next day)
            TimeSpan adjustedExitTime = exitWas24 ? TimeSpan.FromHours(24) : overtime.ExitTime;

            if (adjustedExitTime <= overtime.EntryTime)
            {
                return BadRequest(new { success = false, message = "La hora de salida debe ser posterior a la hora de entrada" });
            }

            // Validate that shift type is selected (needed for OTC payload)
            if (!overtime.TypeOfWorkScheduleId.HasValue)
            {
                return BadRequest(new { success = false, message = "Debe seleccionar el tipo de jornada (Diurno/Nocturno/Mixto) para enviar el registro a OTC" });
            }

            // Ensure OTC credentials are valid and token is available
            var employeeWrapper = await _context.PayrollTmpEmployees
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(e => e.Id == overtime.PayrollTmpEmployeeId);

            if (employeeWrapper?.Employee == null)
            {
                return BadRequest(new { success = false, message = "No se encontró el empleado para enviar a OTC." });
            }

            var otcConfig = await _context.OtcConfigurations.AsNoTracking().FirstOrDefaultAsync(c => c.IsActive);
            if (otcConfig == null)
            {
                return BadRequest(new { success = false, message = "Debe configurar un endpoint OTC activo antes de registrar sobretiempos." });
            }

            string? otcToken;
            try
            {
                otcToken = await EnsureOtcTokenAsync(otcConfig);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo obtener token OTC para el empleado {EmployeeId}", employeeWrapper.Employee.Id);
                return BadRequest(new { success = false, message = ex.Message });
            }

            OtcCalculationResponse otcCalculation;
            try
            {
                otcCalculation = await SendOtcCalculationAsync(overtime, employeeWrapper.Employee, otcConfig, otcToken);
                await PersistOtcOvertimeSegmentsAsync(overtime, employeeWrapper.Employee, otcCalculation);

                var payrollHeader = await _context.PayrollTmpHeaders.FindAsync(overtime.PayrollTmpHeaderId);
                if (payrollHeader != null)
                {
                    await RecalculateDraftEmployeeAsync(overtime.PayrollTmpEmployeeId, payrollHeader);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando cálculo OTC para empleado {EmployeeId}", employeeWrapper.Employee.Id);
                return BadRequest(new { success = false, message = ex.Message });
            }

            // Check if this is an AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var vm = await GetConceptsByEmployeeViewModel(overtime.PayrollTmpEmployeeId);
                return PartialView("_EmployeeConcepts", vm);
            }
            else
            {
                // If not AJAX, redirect back to Details
                return RedirectToAction("Details", new { id = overtime.PayrollTmpHeaderId });
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOvertime(int overtimeId)
        {
            // If overtimeId is 0, try to get it from form data
            if (overtimeId == 0)
            {
                var formOvertimeId = Request.Form["overtimeId"].ToString();
                if (!string.IsNullOrEmpty(formOvertimeId) && int.TryParse(formOvertimeId, out int parsedId))
                {
                    overtimeId = parsedId;
                }
            }

            var overtime = await _context.PayrollTmpOvertime.FindAsync(overtimeId);
            if (overtime == null)
            {
                return NotFound(new { success = false, message = "Registro de horas extras no encontrado" });
            }

            // Find all overtime segments for the same time period
            var overtimeSegments = await _context.PayrollTmpOvertime
                .Where(o => o.PayrollTmpEmployeeId == overtime.PayrollTmpEmployeeId &&
                           o.PayrollTmpHeaderId == overtime.PayrollTmpHeaderId &&
                           o.OvertimeDate == overtime.OvertimeDate &&
                           o.EntryTime == overtime.EntryTime &&
                           o.ExitTime == overtime.ExitTime)
                .ToListAsync();

            _context.PayrollTmpOvertime.RemoveRange(overtimeSegments);
            await _context.SaveChangesAsync();

            var payrollHeader = await _context.PayrollTmpHeaders.FindAsync(overtime.PayrollTmpHeaderId);
            if (payrollHeader != null)
            {
                await RecalculateEmployeeOvertimeWeekAsync(overtime, payrollHeader);
                await RecalculateDraftEmployeeAsync(overtime.PayrollTmpEmployeeId, payrollHeader);
            }

            // Check if this is an AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, message = "Horas extras eliminadas correctamente" });
            }
            else
            {
                // If not AJAX, redirect back to Details
                return RedirectToAction("Details", new { id = overtime.PayrollTmpHeaderId });
            }
        }

        public async Task<IActionResult> GetOvertimeForm(int employeeId)
        {
            var employee = await _context.PayrollTmpEmployees
                .Include(e => e.PayrollTmpHeader)
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
                return NotFound();

            ViewBag.EmployeeId = employeeId;
            ViewBag.PayrollHeaderId = employee.PayrollTmpHeaderId;

            // Populate dropdown data
            ViewBag.TypeOfDays = await _context.TypeOfDays
                .Where(t => t.IsActive)
                .OrderBy(t => t.Code)
                .ToListAsync();

            ViewBag.TypeOfWorkSchedules = await _context.TypeOfWorkSchedules
                .Where(t => t.IsActive)
                .OrderBy(t => t.Code)
                .ToListAsync();

            return PartialView("_OvertimeForm", new PayrollTmpOvertimeEntity
            {
                PayrollTmpEmployeeId = employeeId,
                PayrollTmpHeaderId = employee.PayrollTmpHeaderId
            });
        }

        private async Task<PaymentConceptEntity?> EnsureDestajoConceptAsync(int companyId)
        {
            var existingConcept = await _context.PaymentConcepts.FirstOrDefaultAsync(pc => pc.Code == "DESTJ");
            if (existingConcept != null)
            {
                return existingConcept;
            }

            var company = await _context.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == companyId);
            var countryId = company?.CountryId ?? await _context.Countries.Select(c => (int?)c.Id).FirstOrDefaultAsync();
            if (countryId == null)
            {
                return null;
            }

            var destajoConcept = new PaymentConceptEntity
            {
                Name = "Destajo",
                Code = "DESTJ",
                CountryId = countryId.Value,
                IsActive = true,
                RegularHours = false,
                ExtraHours = false,
                PayFactor = 1.0m,
                RecurrentPayment = false,
                IsPredetermined = false,
                IsConstruction = false
            };

            await _context.PaymentConcepts.AddAsync(destajoConcept);
            await _context.SaveChangesAsync();

            return destajoConcept;
        }

        private record OtcCachedToken(string Token, DateTimeOffset ExpiresAt);
        private record OtcCalculationRange(string Id, double Horas, double Factor);
        private record OtcCalculationResponse(List<OtcCalculationRange>? Rangos, DateTime FechaMarcacion, double Tardanza, string? Mensaje, int Id);

        private static string? ExtractTokenFromRawResponse(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;

            var trimmed = raw.Trim();
            if (!trimmed.StartsWith('{'))
            {
                return trimmed.Trim('"');
            }

            try
            {
                using var document = JsonDocument.Parse(trimmed);
                var root = document.RootElement;

                foreach (var propertyName in new[] { "token", "Token", "access_token", "accessToken" })
                {
                    if (root.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String)
                    {
                        var value = property.GetString();
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            return value;
                        }
                    }
                }
            }
            catch
            {
                // Fall back to null when the response cannot be parsed.
            }

            return null;
        }

        private async Task<string> EnsureOtcTokenAsync(OtcConfigurationEntity config)
        {
            // La autenticación de la API OTC usa las credenciales configuradas, no las del empleado
            var username = config.Username;
            var password = config.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException("La configuración de la API OTC no tiene credenciales válidas.");
            }

            var cacheKey = $"otc_token_{config.Id}";
            if (_cache.TryGetValue<OtcCachedToken>(cacheKey, out var cached) && cached.ExpiresAt > DateTimeOffset.UtcNow)
            {
                return cached.Token;
            }

            // Try login first
            var token = await TryLoginAsync(config.LoginEndpoint, username, password);
            if (string.IsNullOrWhiteSpace(token))
            {
                // Attempt register then login again
                var registered = await TryRegisterAsync(config.RegisterEndpoint, username, password);
                if (!registered)
                {
                    throw new InvalidOperationException("No se pudo registrar el usuario en la API OTC.");
                }

                token = await TryLoginAsync(config.LoginEndpoint, username, password);
                if (string.IsNullOrWhiteSpace(token))
                {
                    throw new InvalidOperationException("No se pudo obtener el token de la API OTC luego del registro.");
                }
            }

            // Validar formato básico de JWT (3 segmentos separados por '.') para evitar enviar tokens malformados
            var tokenValue = token!;
            if (tokenValue.Count(c => c == '.') < 2)
            {
                throw new InvalidOperationException("La API OTC devolvió un token malformado.");
            }

            var expiresAt = DateTimeOffset.UtcNow.Add(_otcTokenTtl);
            _cache.Set(cacheKey, new OtcCachedToken(tokenValue, expiresAt), new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = expiresAt
            });

            return tokenValue;
        }

        private async Task<string?> TryLoginAsync(string endpoint, string username, string password)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestVersion = HttpVersion.Version11;
            client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
            var response = await client.PostAsJsonAsync(endpoint, new { username, password });
            if (!response.IsSuccessStatusCode)
            {
                var rawFail = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("OTC login fallo ({StatusCode}) body: {Body}", (int)response.StatusCode, rawFail);
                return null;
            }

            var raw = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("OTC login respuesta cruda: {Raw}", raw);
            var token = ExtractTokenFromRawResponse(raw);
            if (!string.IsNullOrWhiteSpace(token))
            {
                _logger.LogInformation("OTC login devolvió token con longitud {Length} y {DotCount} segmentos.", token.Length, token.Count(c => c == '.'));
            }

            return token;
        }

        private async Task<bool> TryRegisterAsync(string endpoint, string username, string password)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync(endpoint, new { username, password, confirmPassword = password });
            return response.IsSuccessStatusCode;
        }

        private async Task<OtcCalculationResponse> SendOtcCalculationAsync(PayrollTmpOvertimeEntity overtime, EmployeeEntity employee, OtcConfigurationEntity config, string token)
        {
            int maxRetries = 3;
            int delayMs = 500; // initial delay
            int timeoutSeconds = 30; // timeout per attempt
            var currentToken = token;

            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
                using var client = _httpClientFactory.CreateClient();
                client.DefaultRequestVersion = HttpVersion.Version11;
                client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
                var payload = await BuildOtcCalculatePayload(overtime, employee);
                var payloadJson = JsonSerializer.Serialize(payload);

                using var request = new HttpRequestMessage(HttpMethod.Post, config.CalculateEndpoint)
                {
                    Content = JsonContent.Create(payload, options: new JsonSerializerOptions { PropertyNamingPolicy = null })
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", currentToken);

                _logger.LogInformation("Enviando cálculo OTC a {Endpoint} para empleado {EmployeeId} (intento {Attempt}/{MaxRetries}) payloadJson={PayloadJson}", config.CalculateEndpoint, employee.Id, attempt + 1, maxRetries + 1, payloadJson);

            try
            {
                var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cts.Token);
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized && attempt < maxRetries)
                    {
                        _logger.LogWarning("OTC rechazó el token con 401. Se limpiará el token en caché y se intentará obtener uno nuevo.");
                        _cache.Remove($"otc_token_{config.Id}");
                        currentToken = await EnsureOtcTokenAsync(config);
                        await Task.Delay(delayMs, cts.Token);
                        delayMs *= 2;
                        continue;
                    }

                    string body;
                    try
                    {
                        body = await response.Content.ReadAsStringAsync(cts.Token);
                    }
                    catch (Exception readEx)
                    {
                        _logger.LogWarning(readEx, "Error reading response body from OTC for employee {EmployeeId}", employee.Id);
                        // Treat read errors as network errors to trigger retries
                        throw new HttpRequestException($"Error reading response body: {readEx.Message}", readEx);
                    }
                    _logger.LogWarning("OTC cálculo rechazado ({StatusCode}) body: {Body}", (int)response.StatusCode, body);
                    // For non-success status codes, we might want to retry on 5xx errors
                    if ((int)response.StatusCode >= 500 && attempt < maxRetries)
                    {
                        _logger.LogWarning("OTC devolvió error del servidor ({(int)response.StatusCode}), reintentando en {DelayMs} ms...", (int)response.StatusCode, delayMs);
                        await Task.Delay(delayMs, cts.Token);
                        delayMs *= 2; // exponential backoff
                        continue;
                    }
                    throw new InvalidOperationException($"La API OTC rechazó la marcación ({(int)response.StatusCode}). Detalle: {body}");
                }

                string successBody;
                try
                {
                    successBody = await response.Content.ReadAsStringAsync(cts.Token);
                }
                catch (Exception readEx)
                {
                    _logger.LogWarning(readEx, "Error reading success response body from OTC for employee {EmployeeId}", employee.Id);
                    throw new HttpRequestException($"Error reading success response body: {readEx.Message}", readEx);
                }

                var otcResponse = JsonSerializer.Deserialize<OtcCalculationResponse>(successBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (otcResponse?.Rangos == null)
                {
                    throw new InvalidOperationException($"OTC devolvió una respuesta sin tramos. Respuesta: {successBody}");
                }

                _logger.LogInformation("OTC devolvió {RangeCount} tramos para empleado {EmployeeId}.", otcResponse.Rangos.Count, employee.Id);
                return otcResponse;
            }
            catch (OperationCanceledException) when (cts.IsCancellationRequested)
            {
                // Timeout occurred
                if (attempt < maxRetries)
                {
                    _logger.LogWarning("Timeout al enviar cálculo OTC (después de {TimeoutSeconds} segundos), reintentando en {DelayMs} ms...", timeoutSeconds, delayMs);
                    await Task.Delay(delayMs, cts.Token);
                    delayMs *= 2;
                    continue;
                }
                else
                {
                    _logger.LogError("Timeout al enviar cálculo OTC después de {MaxRetries} intentos.", maxRetries + 1);
                    throw new InvalidOperationException($"Timeout al enviar la marcación a OTC después de {maxRetries + 1} intentos.");
                }
            }
            catch (HttpRequestException ex) when (attempt < maxRetries)
            {
                // Network error (includes errors reading response body)
                _logger.LogWarning(ex, "Error de red al enviar cálculo OTC, reintentando en {DelayMs} ms...", delayMs);
                await Task.Delay(delayMs, cts.Token);
                delayMs *= 2;
                continue;
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException?.Message ?? string.Empty;
                var detail = string.IsNullOrWhiteSpace(inner) ? ex.Message : $"{ex.Message} - {inner}";
                _logger.LogError(ex, "Fallo al enviar cálculo OTC para empleado {EmployeeId} en {Endpoint}", employee.Id, config.CalculateEndpoint);
                throw new InvalidOperationException($"Error enviando la marcación a OTC: {detail}", ex);
            }
            }

            throw new InvalidOperationException("No se recibió respuesta válida de OTC luego de los reintentos.");
        }

        private async Task DeactivateOtcCalculationAsync(PayrollTmpOvertimeEntity overtime, EmployeeEntity employee, OtcConfigurationEntity config, string token)
        {
            var endpoint = ResolveOtcDeactivateEndpoint(config.CalculateEndpoint);
            var payload = BuildOtcDeactivatePayload(overtime, employee);

            using var client = _httpClientFactory.CreateClient();
            client.DefaultRequestVersion = HttpVersion.Version11;
            client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;

            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = JsonContent.Create(payload, options: new JsonSerializerOptions { PropertyNamingPolicy = null })
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"OTC no pudo desactivar el cálculo anterior ({(int)response.StatusCode}). Detalle: {body}");
            }
        }

        private async Task RecalculateEmployeeOvertimeWeekAsync(PayrollTmpOvertimeEntity deletedOvertime, PayrollTmpHeaderEntity payrollHeader)
        {
            var employeeWrapper = await _context.PayrollTmpEmployees
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(e => e.Id == deletedOvertime.PayrollTmpEmployeeId && e.PayrollTmpHeaderId == payrollHeader.Id);

            if (employeeWrapper?.Employee == null)
            {
                return;
            }

            var otcConfig = await _context.OtcConfigurations.AsNoTracking().FirstOrDefaultAsync(c => c.IsActive);
            if (otcConfig == null)
            {
                throw new InvalidOperationException("Debe configurar un endpoint OTC activo antes de recalcular sobretiempos.");
            }

            var token = await EnsureOtcTokenAsync(otcConfig);
            await DeactivateOtcCalculationAsync(deletedOvertime, employeeWrapper.Employee, otcConfig, token);

            var weekStart = GetWeekStart(deletedOvertime.OvertimeDate);
            var weekEndExclusive = weekStart.AddDays(7);

            var weeklySegments = await _context.PayrollTmpOvertime
                .Where(o => o.PayrollTmpEmployeeId == deletedOvertime.PayrollTmpEmployeeId
                    && o.PayrollTmpHeaderId == deletedOvertime.PayrollTmpHeaderId
                    && o.OvertimeDate >= weekStart
                    && o.OvertimeDate < weekEndExclusive)
                .OrderBy(o => o.OvertimeDate)
                .ThenBy(o => o.EntryTime)
                .ThenBy(o => o.ExitTime)
                .ToListAsync();

            var overtimeEntries = weeklySegments
                .GroupBy(o => new { o.OvertimeDate, o.EntryTime, o.ExitTime, o.TypeOfDayId, o.TypeOfWorkScheduleId })
                .Select(g => new PayrollTmpOvertimeEntity
                {
                    PayrollTmpEmployeeId = deletedOvertime.PayrollTmpEmployeeId,
                    PayrollTmpHeaderId = deletedOvertime.PayrollTmpHeaderId,
                    OvertimeDate = g.Key.OvertimeDate,
                    EntryTime = g.Key.EntryTime,
                    ExitTime = g.Key.ExitTime,
                    TypeOfDayId = g.Key.TypeOfDayId,
                    TypeOfWorkScheduleId = g.Key.TypeOfWorkScheduleId
                })
                .OrderBy(o => o.OvertimeDate)
                .ThenBy(o => o.EntryTime)
                .ThenBy(o => o.ExitTime)
                .ToList();

            foreach (var overtimeEntry in overtimeEntries)
            {
                await DeactivateOtcCalculationAsync(overtimeEntry, employeeWrapper.Employee, otcConfig, token);
            }

            foreach (var overtimeEntry in overtimeEntries)
            {
                var otcCalculation = await SendOtcCalculationAsync(overtimeEntry, employeeWrapper.Employee, otcConfig, token);
                await PersistOtcOvertimeSegmentsAsync(overtimeEntry, employeeWrapper.Employee, otcCalculation);
            }
        }

        private static DateTime GetWeekStart(DateTime date)
        {
            var diff = ((int)date.DayOfWeek + 6) % 7;
            return date.Date.AddDays(-diff);
        }

        private static string ResolveOtcDeactivateEndpoint(string calculateEndpoint)
        {
            if (string.IsNullOrWhiteSpace(calculateEndpoint))
            {
                throw new InvalidOperationException("El endpoint de cálculo OTC no está configurado.");
            }

            if (calculateEndpoint.Contains("/calculate-overtime-simple", StringComparison.OrdinalIgnoreCase))
            {
                return calculateEndpoint.Replace("/calculate-overtime-simple", "/deactivate-calculation", StringComparison.OrdinalIgnoreCase);
            }

            if (calculateEndpoint.Contains("/calculate", StringComparison.OrdinalIgnoreCase))
            {
                return calculateEndpoint.Replace("/calculate", "/deactivate-calculation", StringComparison.OrdinalIgnoreCase);
            }

            throw new InvalidOperationException("No se pudo resolver el endpoint OTC para desactivar cálculos.");
        }

        private static Dictionary<string, object?> BuildOtcDeactivatePayload(PayrollTmpOvertimeEntity overtime, EmployeeEntity employee)
        {
            var entryDateTime = overtime.OvertimeDate.Date + overtime.EntryTime;
            var exitDateTime = overtime.OvertimeDate.Date + overtime.ExitTime;
            if (exitDateTime <= entryDateTime)
            {
                exitDateTime = exitDateTime.AddDays(1);
            }

            return new Dictionary<string, object?>
            {
                ["Codigo"] = employee.Code,
                ["FechaMarcacion"] = overtime.OvertimeDate.Date,
                ["HoraEntrada"] = entryDateTime,
                ["HoraSalida"] = exitDateTime
            };
        }

        private async Task PersistOtcOvertimeSegmentsAsync(PayrollTmpOvertimeEntity original, EmployeeEntity employee, OtcCalculationResponse otcCalculation)
        {
            var ranges = otcCalculation.Rangos?
                .Where(r => r.Horas > 0)
                .ToList() ?? new List<OtcCalculationRange>();

            if (!ranges.Any())
            {
                throw new InvalidOperationException("OTC no devolvió tramos de sobretiempo con horas mayores a cero.");
            }

            var existingSegments = await _context.PayrollTmpOvertime
                .Where(o => o.PayrollTmpEmployeeId == original.PayrollTmpEmployeeId &&
                           o.PayrollTmpHeaderId == original.PayrollTmpHeaderId &&
                           o.OvertimeDate == original.OvertimeDate &&
                           o.EntryTime == original.EntryTime &&
                           o.ExitTime == original.ExitTime)
                .ToListAsync();

            if (existingSegments.Any())
            {
                _context.PayrollTmpOvertime.RemoveRange(existingSegments);
            }

            var dayTypeCode = original.TypeOfDayId.HasValue
                ? await _context.TypeOfDays.AsNoTracking()
                    .Where(t => t.Id == original.TypeOfDayId.Value)
                    .Select(t => t.Code)
                    .FirstOrDefaultAsync()
                : null;

            var isSunday = string.Equals(dayTypeCode, "DOMINGO", StringComparison.OrdinalIgnoreCase)
                || string.Equals(dayTypeCode, "DOM", StringComparison.OrdinalIgnoreCase)
                || string.Equals(dayTypeCode, "DESCANSO", StringComparison.OrdinalIgnoreCase);
            var isHoliday = string.Equals(dayTypeCode, "FIESTA", StringComparison.OrdinalIgnoreCase)
                || string.Equals(dayTypeCode, "FERIADO", StringComparison.OrdinalIgnoreCase)
                || string.Equals(dayTypeCode, "DUELO NACIONAL", StringComparison.OrdinalIgnoreCase);

            var segments = ranges.Select(range => new PayrollTmpOvertimeEntity
            {
                PayrollTmpEmployeeId = original.PayrollTmpEmployeeId,
                PayrollTmpHeaderId = original.PayrollTmpHeaderId,
                OvertimeDate = original.OvertimeDate,
                EntryTime = original.EntryTime,
                ExitTime = original.ExitTime,
                TypeOfDayId = original.TypeOfDayId,
                TypeOfWorkScheduleId = original.TypeOfWorkScheduleId,
                CalculatedHours = decimal.Round((decimal)range.Horas, 4, MidpointRounding.AwayFromZero),
                FactorCode = range.Id,
                AppliedFactor = decimal.Round((decimal)range.Factor, 6, MidpointRounding.AwayFromZero),
                HourlyRate = employee.HourSalary,
                TotalAmount = decimal.Round(employee.HourSalary * (decimal)range.Horas * (decimal)range.Factor, 2, MidpointRounding.AwayFromZero),
                IsSunday = isSunday,
                IsHoliday = isHoliday,
                WeeklyAccumulated = 0m,
                Created = DateTime.UtcNow,
                CreatedBy = User?.Identity?.Name ?? "system"
            }).ToList();

            await _context.PayrollTmpOvertime.AddRangeAsync(segments);
            await _context.SaveChangesAsync();
        }

        private async Task<Dictionary<string, object?>> BuildOtcCalculatePayload(PayrollTmpOvertimeEntity original, EmployeeEntity employee)
        {
            var entryDateTime = original.OvertimeDate.Date + original.EntryTime;
            var exitDateTime = original.OvertimeDate.Date + original.ExitTime;
            if (exitDateTime <= entryDateTime)
            {
                exitDateTime = exitDateTime.AddDays(1);
            }

            // Resolve schedule/day labels from DB if navigation properties are not populated
            string? scheduleCode = original.TypeOfWorkSchedule?.Code;
            if (string.IsNullOrWhiteSpace(scheduleCode) && original.TypeOfWorkScheduleId.HasValue)
            {
                scheduleCode = await _context.TypeOfWorkSchedules.AsNoTracking()
                    .Where(t => t.Id == original.TypeOfWorkScheduleId.Value)
                    .Select(t => t.Code)
                    .FirstOrDefaultAsync();
            }

            var tipoHorario = scheduleCode?.ToUpperInvariant() switch
            {
                "DIURNO" => "Diurno",
                "NOCTURNO" => "Nocturno",
                "MIXTO" => "Mixto",
                _ => "Mixto"
            };

            string? dayTypeCode = original.TypeOfDay?.Code;
            if (string.IsNullOrWhiteSpace(dayTypeCode) && original.TypeOfDayId.HasValue)
            {
                dayTypeCode = await _context.TypeOfDays.AsNoTracking()
                    .Where(t => t.Id == original.TypeOfDayId.Value)
                    .Select(t => t.Code)
                    .FirstOrDefaultAsync();
            }

            var tipoDia = string.IsNullOrWhiteSpace(dayTypeCode) ? "Regular" : dayTypeCode;
            var scheduleStart = entryDateTime.ToString("HH:mm");
            var scheduleEnd = exitDateTime.ToString("HH:mm");

            return new Dictionary<string, object?>
            {
                ["Codigo"] = employee.Code,
                ["Nombre"] = employee.FirstName,
                ["Apellido"] = employee.LastName,
                ["Cedula"] = employee.IdDocument,
                ["SalarioPorHora"] = employee.HourSalary,
                ["Compania"] = employee.CompanyId,
                ["Sucursal"] = employee.BranchId,
                ["Departamento"] = employee.DepartmentId,
                ["CentroDeCosto"] = employee.CostCenterId,
                ["Proyecto"] = employee.ProjectId,
                ["Fase"] = employee.PhaseId,
                ["Actividad"] = employee.ActivityId,
                ["TipoDeDia"] = tipoDia,
                ["TipoDeHorario"] = tipoHorario,
                ["TiempoComida"] = 0,
                ["HoraEntrada"] = entryDateTime,
                ["HoraSalida"] = exitDateTime
            };
        }

        // HOLIDAY MANAGEMENT METHODS

        /// <summary>
        /// Updates holidays from Nager.Date API for a specific year
        /// </summary>
        /// <param name="year">Year to update holidays for (default: current year)</param>
        /// <param name="countryCode">ISO2 country code (default: PA for Panama)</param>
        [HttpPost]
        public async Task<IActionResult> UpdateHolidays(int? year = null, string countryCode = "PA")
        {
            try
            {
                var targetYear = year ?? DateTime.Now.Year;
                var holidayService = new Services.HolidayService(_context);

                var updatedCount = await holidayService.UpdateHolidaysFromApiAsync(targetYear, countryCode);

                return Ok(new
                {
                    success = true,
                    message = $"Se actualizaron {updatedCount} días festivos para el año {targetYear} en {countryCode}",
                    updatedCount = updatedCount,
                    year = targetYear,
                    countryCode = countryCode
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error al actualizar días festivos: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Gets all holidays for a specific year
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetHolidays(int? year = null)
        {
            try
            {
                var targetYear = year ?? DateTime.Now.Year;
                var holidayService = new Services.HolidayService(_context);

                var holidays = await holidayService.GetHolidaysForYearAsync(targetYear);

                return Ok(new
                {
                    success = true,
                    holidays = holidays.Select(h => new
                    {
                        id = h.Id,
                        date = h.Date.ToString("yyyy-MM-dd"),
                        description = h.Description,
                        isActive = h.IsActive
                    }),
                    count = holidays.Count,
                    year = targetYear
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error al obtener días festivos: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Checks if a specific date is a holiday
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CheckHoliday(string date)
        {
            try
            {
                if (!DateTime.TryParse(date, out DateTime checkDate))
                {
                    return BadRequest(new { success = false, message = "Formato de fecha inválido" });
                }

                var holidayService = new Services.HolidayService(_context);
                var isHoliday = await holidayService.IsHolidayAsync(checkDate);

                return Ok(new
                {
                    success = true,
                    date = checkDate.ToString("yyyy-MM-dd"),
                    isHoliday = isHoliday
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error al verificar día festivo: {ex.Message}"
                });
            }
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
                ViewData["Message"] = "No hay empleados para este Grupo de Pago";
                return BadRequest();
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
                return NotFound();

            return new JsonResult(new
            {
                employees,
                paymentGroup
            });
        }
    }
}
