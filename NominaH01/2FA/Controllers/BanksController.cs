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
using Microsoft.AspNetCore.Identity;
using _2FA.Models;

namespace _2FA.Controllers
{
    public class BanksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BanksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Banks
        public async Task<IActionResult> Index()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Configuración", Url = "/Home/Configurations" },
                new BreadcrumbItem { Text = "Configuración de Bancos", Url = "/Home/Banks" },
                new BreadcrumbItem { Text = "Listado de Bancos" }
            };

            return View(await _context.Banks.Include(b => b.TransitBank).ToListAsync());
        }

        // GET: Banks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Configuración", Url = "/Home/Configurations" },
                new BreadcrumbItem { Text = "Configuración de Bancos", Url = "/Home/Banks" },
                new BreadcrumbItem { Text = "Listado de Bancos", Url = "/Banks/Index" },
                new BreadcrumbItem { Text = "Detalle de Bancos" }
            };

            if (id == null)
            {
                return NotFound();
            }

            var bankEntity = await _context.Banks
                .Include(b => b.TransitBank)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bankEntity == null)
            {
                return NotFound();
            }

            return View(bankEntity);
        }

        // GET: Banks/Create
        public IActionResult Create()
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Configuración", Url = "/Home/Configurations" },
                new BreadcrumbItem { Text = "Configuración de Bancos", Url = "/Home/Banks" },
                new BreadcrumbItem { Text = "Listado de Bancos", Url = "/Banks/Index" },
                new BreadcrumbItem { Text = "Nuevo Banco" }
            };

            var bank = new BankEntity();
            bank.IsActive = true;

            ViewData["TransitBankId"] = new SelectList(_context.TransitBanks, "Id", "Name");
            return View(bank);
        }

        // POST: Banks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BankEntity bankEntity)
        {
            if (ModelState.IsValid)
            {
                bankEntity.Created = DateTime.UtcNow;
                bankEntity.CreatedBy = User?.Identity?.Name ?? string.Empty;
                _context.Add(bankEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TransitBankId"] = new SelectList(_context.TransitBanks, "Id", "Name", bankEntity.TransitBankId);
            return View(bankEntity);
        }

        // GET: Banks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["Breadcrumb"] = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Text = "Inicio", Url = "/" },
                new BreadcrumbItem { Text = "Configuración", Url = "/Home/Configurations" },
                new BreadcrumbItem { Text = "Configuración de Bancos", Url = "/Home/Banks" },
                new BreadcrumbItem { Text = "Listado de Bancos", Url = "/Banks/Index" },
                new BreadcrumbItem { Text = "Editar Banco" }
            };

            if (id == null)
            {
                return NotFound();
            }

            var bankEntity = await _context.Banks.FindAsync(id);
            if (bankEntity == null)
            {
                return NotFound();
            }
            ViewData["TransitBankId"] = new SelectList(_context.TransitBanks, "Id", "Name", bankEntity.TransitBankId);
            return View(bankEntity);
        }

        // POST: Banks/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BankEntity bankEntity)
        {
            if (id != bankEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bankEntity.Modified = DateTime.UtcNow;
                    bankEntity.ModifiedBy = User?.Identity?.Name ?? string.Empty;
                    
                    _context.Update(bankEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BankEntityExists(bankEntity.Id))
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
            ViewData["TransitBankId"] = new SelectList(_context.TransitBanks, "Id", "Name", bankEntity.TransitBankId);
            return View(bankEntity);
        }

        // GET: Banks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bankEntity = await _context.Banks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bankEntity == null)
            {
                return NotFound();
            }

            return View(bankEntity);
        }

        // POST: Banks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bankEntity = await _context.Banks.FindAsync(id);
            if (bankEntity != null)
            {
                bankEntity.Deleted = DateTime.UtcNow;
                bankEntity.DeletedBy = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
                bankEntity.IsActive = false;
                bankEntity.IsDeleted = true;
                _context.Banks.Remove(bankEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BankEntityExists(int id)
        {
            return _context.Banks.Any(e => e.Id == id);
        }
    }
}
