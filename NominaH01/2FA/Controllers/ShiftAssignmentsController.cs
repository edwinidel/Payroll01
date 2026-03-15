using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using _2FA.Data;
using _2FA.Data.Entities;
using System.Security.Claims;

namespace _2FA.Controllers
{
    public class ShiftAssignmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public ShiftAssignmentsController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        // GET: ShiftAssignments
        public async Task<IActionResult> Index()
        {
            var shiftAssignments = await _context.ShiftAssignments
                .Include(sa => sa.Employee)
                .Include(sa => sa.Shift)
                .Include(sa => sa.Schedule)
                .ToListAsync();
            return View(shiftAssignments);
        }

        // GET: ShiftAssignments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shiftAssignmentEntity = await _context.ShiftAssignments
                .Include(sa => sa.Employee)
                .Include(sa => sa.Shift)
                .Include(sa => sa.Schedule)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shiftAssignmentEntity == null)
            {
                return NotFound();
            }

            return View(shiftAssignmentEntity);
        }

        // GET: ShiftAssignments/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            ViewData["ShiftId"] = new SelectList(_context.Shifts.Where(s => s.IsActive), "Id", "Name");
            ViewData["ScheduleId"] = new SelectList(_context.Schedules.Where(s => s.IsActive), "Id", "Name");
            return View();
        }

        // POST: ShiftAssignments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ShiftAssignmentEntity shiftAssignmentEntity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shiftAssignmentEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", shiftAssignmentEntity.EmployeeId);
            ViewData["ShiftId"] = new SelectList(_context.Shifts.Where(s => s.IsActive), "Id", "Name", shiftAssignmentEntity.ShiftId);
            ViewData["ScheduleId"] = new SelectList(_context.Schedules.Where(s => s.IsActive), "Id", "Name", shiftAssignmentEntity.ScheduleId);
            return View(shiftAssignmentEntity);
        }

        // GET: ShiftAssignments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shiftAssignmentEntity = await _context.ShiftAssignments.FindAsync(id);
            if (shiftAssignmentEntity == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", shiftAssignmentEntity.EmployeeId);
            ViewData["ShiftId"] = new SelectList(_context.Shifts.Where(s => s.IsActive), "Id", "Name", shiftAssignmentEntity.ShiftId);
            ViewData["ScheduleId"] = new SelectList(_context.Schedules.Where(s => s.IsActive), "Id", "Name", shiftAssignmentEntity.ScheduleId);
            return View(shiftAssignmentEntity);
        }

        // POST: ShiftAssignments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ShiftAssignmentEntity shiftAssignmentEntity)
        {
            if (id != shiftAssignmentEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shiftAssignmentEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShiftAssignmentEntityExists(shiftAssignmentEntity.Id))
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
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", shiftAssignmentEntity.EmployeeId);
            ViewData["ShiftId"] = new SelectList(_context.Shifts.Where(s => s.IsActive), "Id", "Name", shiftAssignmentEntity.ShiftId);
            ViewData["ScheduleId"] = new SelectList(_context.Schedules.Where(s => s.IsActive), "Id", "Name", shiftAssignmentEntity.ScheduleId);
            return View(shiftAssignmentEntity);
        }

        // GET: ShiftAssignments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shiftAssignmentEntity = await _context.ShiftAssignments
                .Include(sa => sa.Employee)
                .Include(sa => sa.Shift)
                .Include(sa => sa.Schedule)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shiftAssignmentEntity == null)
            {
                return NotFound();
            }

            return View(shiftAssignmentEntity);
        }

        // POST: ShiftAssignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shiftAssignmentEntity = await _context.ShiftAssignments.FindAsync(id);
            if (shiftAssignmentEntity != null)
            {
                _context.ShiftAssignments.Remove(shiftAssignmentEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: ShiftAssignments/Calendar
        public async Task<IActionResult> Calendar()
        {
            var shiftAssignments = await _context.ShiftAssignments
                .Include(sa => sa.Employee)
                .Include(sa => sa.Shift)
                .Include(sa => sa.Schedule)
                .ToListAsync();
            return View(shiftAssignments);
        }

        // POST: ShiftAssignments/SendNotification/5
        [HttpPost]
        public async Task<IActionResult> SendNotification(int id)
        {
            var shiftAssignment = await _context.ShiftAssignments
                .Include(sa => sa.Employee)
                .Include(sa => sa.Shift)
                .Include(sa => sa.Schedule)
                .FirstOrDefaultAsync(sa => sa.Id == id);

            if (shiftAssignment == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(shiftAssignment.Employee.CEMAIL))
            {
                TempData["Error"] = "El empleado no tiene correo electrónico registrado.";
                return RedirectToAction(nameof(Index));
            }

            var subject = "Asignación de Turno";
            var segmentsHtml = "";
            if (shiftAssignment.Shift.ShiftSegments != null && shiftAssignment.Shift.ShiftSegments.Any())
            {
                foreach (var segment in shiftAssignment.Shift.ShiftSegments.OrderBy(s => s.Order))
                {
                    segmentsHtml += $"<li><strong>{segment.SegmentType}:</strong> {segment.StartTime.ToString("hh\\:mm")} - {segment.EndTime.ToString("hh\\:mm")}</li>";
                }
            }

            var htmlMessage = $@"
                <h2>Asignación de Turno</h2>
                <p>Hola {shiftAssignment.Employee.FullName},</p>
                <p>Se le ha asignado el siguiente turno:</p>
                <ul>
                    <li><strong>Turno:</strong> {shiftAssignment.Shift.Name}</li>
                    <li><strong>Fecha:</strong> {shiftAssignment.Date.ToString("dd/MM/yyyy")}</li>
                    {segmentsHtml}
                    <li><strong>Horario:</strong> {shiftAssignment.Schedule.Name}</li>
                </ul>
                <p>Por favor, confirme su asistencia.</p>
                <p>Saludos,<br>Equipo de RRHH</p>
            ";

            await _emailSender.SendEmailAsync(shiftAssignment.Employee.CEMAIL, subject, htmlMessage);

            TempData["Success"] = "Notificación enviada exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        private bool ShiftAssignmentEntityExists(int id)
        {
            return _context.ShiftAssignments.Any(e => e.Id == id);
        }
    }
}