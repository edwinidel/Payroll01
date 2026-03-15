using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _2FA.Data;
using _2FA.Data.Entities;

namespace _2FA.Controllers
{
    public class OvertimeCodesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OvertimeCodesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OvertimeCodes
        public async Task<IActionResult> Index()
        {
            return View(await _context.OvertimeCodes.ToListAsync());
        }

        // GET: OvertimeCodes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var overtimeCodeEntity = await _context.OvertimeCodes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (overtimeCodeEntity == null)
            {
                return NotFound();
            }

            return View(overtimeCodeEntity);
        }

        // GET: OvertimeCodes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OvertimeCodes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OvertimeCodeEntity overtimeCodeEntity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(overtimeCodeEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(overtimeCodeEntity);
        }

        // GET: OvertimeCodes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var overtimeCodeEntity = await _context.OvertimeCodes.FindAsync(id);
            if (overtimeCodeEntity == null)
            {
                return NotFound();
            }
            return View(overtimeCodeEntity);
        }

        // POST: OvertimeCodes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OvertimeCodeEntity overtimeCodeEntity)
        {
            if (id != overtimeCodeEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(overtimeCodeEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OvertimeCodeEntityExists(overtimeCodeEntity.Id))
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
            return View(overtimeCodeEntity);
        }

        // GET: OvertimeCodes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var overtimeCodeEntity = await _context.OvertimeCodes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (overtimeCodeEntity == null)
            {
                return NotFound();
            }

            return View(overtimeCodeEntity);
        }

        // POST: OvertimeCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var overtimeCodeEntity = await _context.OvertimeCodes.FindAsync(id);
            if (overtimeCodeEntity != null)
            {
                _context.OvertimeCodes.Remove(overtimeCodeEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OvertimeCodeEntityExists(int id)
        {
            return _context.OvertimeCodes.Any(e => e.Id == id);
        }
    }
}
