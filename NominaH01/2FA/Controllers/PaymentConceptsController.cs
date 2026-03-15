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
using _2FA.Models;

namespace _2FA.Controllers
{
    public class PaymentConceptsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentConceptsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PaymentConceptEntities
        public async Task<IActionResult> Index()
        {
            return View(await _context.PaymentConcepts.ToListAsync());
        }

        // GET: PaymentConceptEntities/Details/5
        [HttpGet]
        public IActionResult Details(int id)
        {
            var concept = _context.PaymentConcepts.FirstOrDefault(c => c.Id == id);
            if (concept == null) return NotFound();

            // IDs de deducciones ya asignadas
            var assignedIds = _context.ConceptLegalDeductions
                .Where(cld => cld.PaymentConceptId == id)
                .Select(cld => cld.LegalDeductionEntityId)
                .ToList();

            // Lista de asignados
            var assignedList = _context.LegalDeductions
                .Where(ld => assignedIds.Contains(ld.Id))
                .Select(ld => new LegalDeductionAssigned
                {
                    Id = ld.Id,
                    Code = ld.Code,
                    Name = ld.Name,
                    EmployeeDiscount = ld.EmployeeDiscount, 
                    EmployerDiscount = ld.EmployerDiscount
                })
                .ToList();

            // Lista de disponibles (para el select)
            var availableList = _context.LegalDeductions
                .Where(ld => !assignedIds.Contains(ld.Id))
                .Select(ld => new
                {
                    ld.Id,
                    ld.Code,
                    ld.Name,
                    EmployeeDiscount = ld.EmployeeDiscount,
                    EmployerDiscount = ld.EmployerDiscount
                })
                .ToList();

            var vm = new PaymentConceptAssignmentViewModel
            {
                PaymentConceptId = concept.Id,
                PaymentConceptName = concept.Name,
                AssignedDeductions = assignedList,
                ExtraHours = concept.ExtraHours,
                CountryId = concept.CountryId,
                IsActive = concept.IsActive,
                Name = concept.Name,
                PayFactor = concept.PayFactor,
                RecurrentPayment = concept.RecurrentPayment,
                 RegularHours = concept.RegularHours
            };

            ViewBag.AvailableDeductionsData = availableList;

            return View(vm);
        }

        [HttpPost]
        public IActionResult SaveAssignments(int PaymentConceptId, List<int> AssignedIds)
        {
            // Eliminar relaciones existentes
            var existing = _context.ConceptLegalDeductions
                .Where(cld => cld.PaymentConceptId == PaymentConceptId);
            _context.ConceptLegalDeductions.RemoveRange(existing);

            // Agregar las nuevas
            if (AssignedIds != null && AssignedIds.Count > 0)
            {
                foreach (var id in AssignedIds)
                {
                    _context.ConceptLegalDeductions.Add(new ConceptLegalDeductionEntity
                    {
                        PaymentConceptId = PaymentConceptId,
                        LegalDeductionEntityId = id
                    });
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Details", new { id = PaymentConceptId });
        }


        // GET: PaymentConceptEntities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PaymentConceptEntities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaymentConceptEntity paymentConceptEntity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(paymentConceptEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(paymentConceptEntity);
        }

        // GET: PaymentConceptEntities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentConceptEntity = await _context.PaymentConcepts.FindAsync(id);
            if (paymentConceptEntity == null)
            {
                return NotFound();
            }
            return View(paymentConceptEntity);
        }

        // POST: PaymentConceptEntities/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PaymentConceptEntity paymentConceptEntity)
        {
            if (id != paymentConceptEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paymentConceptEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentConceptEntityExists(paymentConceptEntity.Id))
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
            return View(paymentConceptEntity);
        }

        // GET: PaymentConceptEntities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentConceptEntity = await _context.PaymentConcepts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paymentConceptEntity == null)
            {
                return NotFound();
            }

            return View(paymentConceptEntity);
        }

        // POST: PaymentConceptEntities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paymentConceptEntity = await _context.PaymentConcepts.FindAsync(id);
            if (paymentConceptEntity != null)
            {
                _context.PaymentConcepts.Remove(paymentConceptEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentConceptEntityExists(int id)
        {
            return _context.PaymentConcepts.Any(e => e.Id == id);
        }
    }
}
