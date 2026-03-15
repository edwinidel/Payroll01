using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _2FA.Data;
using _2FA.Data.Entities;

namespace _2FA.Controllers
{
    public class OverTimeFactorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OverTimeFactorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OverTimeFactors
        public async Task<IActionResult> Index()
        {
            return View(await _context.OverTimeFactors.ToListAsync());
        }

        // GET: OverTimeFactors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var overTimeFactorEntity = await _context.OverTimeFactors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (overTimeFactorEntity == null)
            {
                return NotFound();
            }

            return View(overTimeFactorEntity);
        }

        // GET: OverTimeFactors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OverTimeFactors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OverTimeFactorEntity overTimeFactorEntity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(overTimeFactorEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(overTimeFactorEntity);
        }

        // GET: OverTimeFactors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var overTimeFactorEntity = await _context.OverTimeFactors.FindAsync(id);
            if (overTimeFactorEntity == null)
            {
                return NotFound();
            }
            return View(overTimeFactorEntity);
        }

        // POST: OverTimeFactors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OverTimeFactorEntity overTimeFactorEntity)
        {
            if (id != overTimeFactorEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(overTimeFactorEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OverTimeFactorEntityExists(overTimeFactorEntity.Id))
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
            return View(overTimeFactorEntity);
        }

        // GET: OverTimeFactors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var overTimeFactorEntity = await _context.OverTimeFactors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (overTimeFactorEntity == null)
            {
                return NotFound();
            }

            return View(overTimeFactorEntity);
        }

        // POST: OverTimeFactors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var overTimeFactorEntity = await _context.OverTimeFactors.FindAsync(id);
            if (overTimeFactorEntity != null)
            {
                _context.OverTimeFactors.Remove(overTimeFactorEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OverTimeFactorEntityExists(int id)
        {
            return _context.OverTimeFactors.Any(e => e.Id == id);
        }
    }
}
