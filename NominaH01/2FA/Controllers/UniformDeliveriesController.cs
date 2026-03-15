using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _2FA.Data;
using _2FA.Data.Entities;

namespace _2FA.Controllers
{
    public class UniformDeliveriesController : BaseController
    {
        public UniformDeliveriesController(ApplicationDbContext context) : base(context)
        {
        }

        // GET: UniformDeliveries
        public async Task<IActionResult> Index(int? employeeId)
        {
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            var deliveriesQuery = FilterByCurrentCompany(_context.UniformDeliveries
                    .Include(u => u.Employee)
                    .Include(u => u.Company))
                .Where(u => !u.IsDeleted);

            if (employeeId.HasValue)
            {
                deliveriesQuery = deliveriesQuery.Where(u => u.EmployeeId == employeeId.Value);
            }

            var deliveries = await deliveriesQuery
                .OrderBy(u => u.ExpirationDate)
                .ThenBy(u => u.Employee!.LastName)
                .ToListAsync();

            ViewData["EmployeeId"] = GetEmployeesSelectList(employeeId);
            ViewData["ExpiredCount"] = deliveries.Count(u => u.IsExpired);
            ViewData["ExpiringSoonCount"] = deliveries.Count(u => u.IsExpiringSoon);

            return View(deliveries);
        }

        // GET: UniformDeliveries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            if (id == null)
            {
                return NotFound();
            }

            var uniformDeliveryEntity = await FilterByCurrentCompany(_context.UniformDeliveries
                    .Include(u => u.Employee)
                    .Include(u => u.Company))
                .Where(u => !u.IsDeleted)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (uniformDeliveryEntity == null)
            {
                return NotFound();
            }

            return View(uniformDeliveryEntity);
        }

        // GET: UniformDeliveries/Create
        public IActionResult Create(int? employeeId)
        {
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            var model = new UniformDeliveryEntity
            {
                DeliveryDate = DateTime.UtcNow.Date,
                ValidityDays = 365,
                AlertDaysBefore = 30,
                Quantity = 1
            };
            model.ExpirationDate = CalculateExpirationDate(model);

            ViewData["EmployeeId"] = GetEmployeesSelectList(employeeId);
            return View(model);
        }

        // POST: UniformDeliveries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UniformDeliveryEntity uniformDeliveryEntity)
        {
            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de crear un registro.";
                return RedirectToAction("SwitchCompany", "Home");
            }

            uniformDeliveryEntity.CompanyId = currentCompanyId.Value;
            uniformDeliveryEntity.ExpirationDate = CalculateExpirationDate(uniformDeliveryEntity);

            if (ModelState.IsValid)
            {
                uniformDeliveryEntity.Created = DateTime.UtcNow;
                uniformDeliveryEntity.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
                _context.Add(uniformDeliveryEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { employeeId = uniformDeliveryEntity.EmployeeId });
            }

            ViewData["EmployeeId"] = GetEmployeesSelectList(uniformDeliveryEntity.EmployeeId);
            return View(uniformDeliveryEntity);
        }

        // GET: UniformDeliveries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            if (id == null)
            {
                return NotFound();
            }

            var uniformDeliveryEntity = await FilterByCurrentCompany(_context.UniformDeliveries)
                .Where(u => !u.IsDeleted)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (uniformDeliveryEntity == null)
            {
                return NotFound();
            }

            ViewData["EmployeeId"] = GetEmployeesSelectList(uniformDeliveryEntity.EmployeeId);
            return View(uniformDeliveryEntity);
        }

        // POST: UniformDeliveries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UniformDeliveryEntity uniformDeliveryEntity)
        {
            if (id != uniformDeliveryEntity.Id)
            {
                return NotFound();
            }

            var currentCompanyId = GetCurrentCompanyId();
            if (!currentCompanyId.HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de editar un registro.";
                return RedirectToAction("SwitchCompany", "Home");
            }

            var existing = await FilterByCurrentCompany(_context.UniformDeliveries)
                .Where(u => !u.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (existing == null)
            {
                return NotFound();
            }

            uniformDeliveryEntity.CompanyId = currentCompanyId.Value;
            uniformDeliveryEntity.ExpirationDate = CalculateExpirationDate(uniformDeliveryEntity);

            if (ModelState.IsValid)
            {
                try
                {
                    uniformDeliveryEntity.Modified = DateTime.UtcNow;
                    uniformDeliveryEntity.ModifiedBy = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
                    _context.Update(uniformDeliveryEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UniformDeliveryEntityExists(uniformDeliveryEntity.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { employeeId = uniformDeliveryEntity.EmployeeId });
            }

            ViewData["EmployeeId"] = GetEmployeesSelectList(uniformDeliveryEntity.EmployeeId);
            return View(uniformDeliveryEntity);
        }

        // GET: UniformDeliveries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var redirectResult = EnsureCompanySelected();
            if (redirectResult != null)
                return redirectResult;

            if (id == null)
            {
                return NotFound();
            }

            var uniformDeliveryEntity = await FilterByCurrentCompany(_context.UniformDeliveries
                    .Include(u => u.Employee)
                    .Include(u => u.Company))
                .Where(u => !u.IsDeleted)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (uniformDeliveryEntity == null)
            {
                return NotFound();
            }

            return View(uniformDeliveryEntity);
        }

        // POST: UniformDeliveries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var uniformDeliveryEntity = await FilterByCurrentCompany(_context.UniformDeliveries)
                .Where(u => !u.IsDeleted)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (uniformDeliveryEntity != null)
            {
                uniformDeliveryEntity.Deleted = DateTime.UtcNow;
                uniformDeliveryEntity.DeletedBy = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
                uniformDeliveryEntity.IsDeleted = true;
                _context.Update(uniformDeliveryEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { employeeId = uniformDeliveryEntity.EmployeeId });
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UniformDeliveryEntityExists(int id)
        {
            return _context.UniformDeliveries.Any(e => e.Id == id && !e.IsDeleted);
        }

        private SelectList GetEmployeesSelectList(int? selectedId = null)
        {
            var employees = FilterByCurrentCompany(_context.Employees)
                .Where(e => !e.IsDeleted)
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Select(e => new { e.Id, Name = e.FullNameWithCode });

            return new SelectList(employees, "Id", "Name", selectedId);
        }

        private static DateTime CalculateExpirationDate(UniformDeliveryEntity entity)
        {
            var deliveryDate = entity.DeliveryDate.Date;
            var validityDays = entity.ValidityDays < 0 ? 0 : entity.ValidityDays;
            return deliveryDate.AddDays(validityDays);
        }
    }
}
