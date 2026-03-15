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

namespace _2FA.Controllers
{
    public class ShiftsController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // GET: Shifts
        public async Task<IActionResult> Index()
        {
            var shifts = await _context.Shifts
                .Include(s => s.Schedule)
                .Include(s => s.ShiftSegments)
                .ToListAsync();
            return View(shifts);
        }

        // GET: Shifts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shiftEntity = await _context.Shifts
                .Include(s => s.Schedule)
                .Include(s => s.ShiftSegments)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shiftEntity == null)
            {
                return NotFound();
            }

            return View(shiftEntity);
        }

        // GET: Shifts/Create
        public IActionResult Create()
        {
            ViewData["ScheduleId"] = new SelectList(_context.Schedules.Where(s => s.IsActive), "Id", "Name");
            var shiftEntity = new ShiftEntity
            {
                Segments = new List<ShiftSegmentEntity>()
            };
            return View(shiftEntity);
        }

        // POST: Shifts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ShiftEntity shiftEntity)
        {
            ModelState.Remove("Id");
            ModelState.Remove("Created");
            ModelState.Remove("CreatedBy");
            if (ModelState.IsValid)
            {
                shiftEntity.Created = DateTime.UtcNow;
                shiftEntity.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Add(shiftEntity);
                await _context.SaveChangesAsync();

                // Add segments
                if (shiftEntity.Segments != null && shiftEntity.Segments.Any())
                {
                    foreach (var segment in shiftEntity.Segments)
                    {
                        segment.ShiftId = shiftEntity.Id;
                        _context.Add(segment);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["ScheduleId"] = new SelectList(_context.Schedules.Where(s => s.IsActive), "Id", "Name", shiftEntity.ScheduleId);
            // Ensure Segments is not null for the view
            if (shiftEntity.Segments == null)
            {
                shiftEntity.Segments = new List<ShiftSegmentEntity>();
            }
            return View(shiftEntity);
        }

        // GET: Shifts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shiftEntity = await _context.Shifts
                .Include(s => s.ShiftSegments)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (shiftEntity == null)
            {
                return NotFound();
            }

            // Populate Segments property for form binding
            if (shiftEntity.ShiftSegments != null)
            {
                shiftEntity.Segments = shiftEntity.ShiftSegments.ToList();
            }
            else
            {
                shiftEntity.Segments = new List<ShiftSegmentEntity>();
            }

            ViewData["ScheduleId"] = new SelectList(_context.Schedules.Where(s => s.IsActive), "Id", "Name", shiftEntity.ScheduleId);
            return View(shiftEntity);
        }

        // POST: Shifts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ShiftEntity shiftEntity)
        {
            if (id != shiftEntity.Id)
            {
                return NotFound();
            }

            ModelState.Remove("ModifiedBy");
            if (ModelState.IsValid)
            {
                try
                {
                    shiftEntity.Modified = DateTime.UtcNow;
                    shiftEntity.ModifiedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    _context.Update(shiftEntity);
                    await _context.SaveChangesAsync();

                    // Handle segments
                    var existingSegments = await _context.ShiftSegments.Where(s => s.ShiftId == id).ToListAsync();

                    if (shiftEntity.Segments != null && shiftEntity.Segments.Any())
                    {
                        // Update or add segments
                        foreach (var segment in shiftEntity.Segments)
                        {
                            if (segment.Id > 0)
                            {
                                // Update existing segment
                                var existingSegment = existingSegments.FirstOrDefault(s => s.Id == segment.Id);
                                if (existingSegment != null)
                                {
                                    existingSegment.StartTime = segment.StartTime;
                                    existingSegment.EndTime = segment.EndTime;
                                    existingSegment.SegmentType = segment.SegmentType;
                                    existingSegment.Order = segment.Order;
                                    _context.Update(existingSegment);
                                }
                            }
                            else
                            {
                                // Add new segment
                                segment.ShiftId = shiftEntity.Id;
                                _context.Add(segment);
                            }
                        }

                        // Remove segments that are no longer present
                        var segmentIds = shiftEntity.Segments.Where(s => s.Id > 0).Select(s => s.Id).ToList();
                        var segmentsToRemove = existingSegments.Where(s => !segmentIds.Contains(s.Id)).ToList();
                        foreach (var segmentToRemove in segmentsToRemove)
                        {
                            _context.ShiftSegments.Remove(segmentToRemove);
                        }
                    }
                    else
                    {
                        // Remove all segments if none provided
                        foreach (var segment in existingSegments)
                        {
                            _context.ShiftSegments.Remove(segment);
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShiftEntityExists(shiftEntity.Id))
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
            ViewData["ScheduleId"] = new SelectList(_context.Schedules.Where(s => s.IsActive), "Id", "Name", shiftEntity.ScheduleId);
            // Ensure Segments is not null for the view
            if (shiftEntity.Segments == null)
            {
                shiftEntity.Segments = new List<ShiftSegmentEntity>();
            }
            return View(shiftEntity);
        }

        // GET: Shifts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shiftEntity = await _context.Shifts
                .Include(s => s.Schedule)
                .Include(s => s.ShiftSegments)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shiftEntity == null)
            {
                return NotFound();
            }

            return View(shiftEntity);
        }

        // POST: Shifts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shiftEntity = await _context.Shifts.FindAsync(id);
            if (shiftEntity != null)
            {
                _context.Shifts.Remove(shiftEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShiftEntityExists(int id)
        {
            return _context.Shifts.Any(e => e.Id == id);
        }
    }
}