using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _2FA.Data;
using _2FA.Data.Entities;

namespace _2FA.Controllers
{
    public class TypeOfDaysController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TypeOfDaysController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TypeOfDays
        public async Task<IActionResult> Index()
        {
            return View(await _context.TypeOfDays.ToListAsync());
        }

        // GET: TypeOfDays/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeOfDayEntity = await _context.TypeOfDays
                .FirstOrDefaultAsync(m => m.Id == id);
            if (typeOfDayEntity == null)
            {
                return NotFound();
            }

            return View(typeOfDayEntity);
        }

        // GET: TypeOfDays/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TypeOfDays/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TypeOfDayEntity typeOfDayEntity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(typeOfDayEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(typeOfDayEntity);
        }

        // GET: TypeOfDays/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeOfDayEntity = await _context.TypeOfDays.FindAsync(id);
            if (typeOfDayEntity == null)
            {
                return NotFound();
            }
            return View(typeOfDayEntity);
        }

        // POST: TypeOfDays/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TypeOfDayEntity typeOfDayEntity)
        {
            if (id != typeOfDayEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(typeOfDayEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TypeOfDayEntityExists(typeOfDayEntity.Id))
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
            return View(typeOfDayEntity);
        }

        // GET: TypeOfDays/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeOfDayEntity = await _context.TypeOfDays
                .FirstOrDefaultAsync(m => m.Id == id);
            if (typeOfDayEntity == null)
            {
                return NotFound();
            }

            return View(typeOfDayEntity);
        }

        // POST: TypeOfDays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var typeOfDayEntity = await _context.TypeOfDays.FindAsync(id);
            if (typeOfDayEntity != null)
            {
                _context.TypeOfDays.Remove(typeOfDayEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TypeOfDayEntityExists(int id)
        {
            return _context.TypeOfDays.Any(e => e.Id == id);
        }
    }
}
