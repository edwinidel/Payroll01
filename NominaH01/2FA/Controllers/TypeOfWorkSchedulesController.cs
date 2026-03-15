using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _2FA.Data;
using _2FA.Data.Entities;

namespace _2FA.Controllers
{
    public class TypeOfWorkSchedulesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TypeOfWorkSchedulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TypeOfWorkSchedules
        public async Task<IActionResult> Index()
        {
            return View(await _context.TypeOfWorkSchedules.ToListAsync());
        }

        // GET: TypeOfWorkSchedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeOfWorkScheduleEntity = await _context.TypeOfWorkSchedules
                .FirstOrDefaultAsync(m => m.Id == id);
            if (typeOfWorkScheduleEntity == null)
            {
                return NotFound();
            }

            return View(typeOfWorkScheduleEntity);
        }

        // GET: TypeOfWorkSchedules/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TypeOfWorkSchedules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TypeOfWorkScheduleEntity typeOfWorkScheduleEntity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(typeOfWorkScheduleEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(typeOfWorkScheduleEntity);
        }

        // GET: TypeOfWorkSchedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeOfWorkScheduleEntity = await _context.TypeOfWorkSchedules.FindAsync(id);
            if (typeOfWorkScheduleEntity == null)
            {
                return NotFound();
            }
            return View(typeOfWorkScheduleEntity);
        }

        // POST: TypeOfWorkSchedules/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TypeOfWorkScheduleEntity typeOfWorkScheduleEntity)
        {
            if (id != typeOfWorkScheduleEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(typeOfWorkScheduleEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TypeOfWorkScheduleEntityExists(typeOfWorkScheduleEntity.Id))
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
            return View(typeOfWorkScheduleEntity);
        }

        // GET: TypeOfWorkSchedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeOfWorkScheduleEntity = await _context.TypeOfWorkSchedules
                .FirstOrDefaultAsync(m => m.Id == id);
            if (typeOfWorkScheduleEntity == null)
            {
                return NotFound();
            }

            return View(typeOfWorkScheduleEntity);
        }

        // POST: TypeOfWorkSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var typeOfWorkScheduleEntity = await _context.TypeOfWorkSchedules.FindAsync(id);
            if (typeOfWorkScheduleEntity != null)
            {
                _context.TypeOfWorkSchedules.Remove(typeOfWorkScheduleEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TypeOfWorkScheduleEntityExists(int id)
        {
            return _context.TypeOfWorkSchedules.Any(e => e.Id == id);
        }
    }
}
