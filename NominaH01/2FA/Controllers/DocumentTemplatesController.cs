using System.IO;
using System.Threading.Tasks;
using _2FA.Data;
using _2FA.Data.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Collections.Generic;
using _2FA.Services;

namespace _2FA.Controllers
{
    public class DocumentTemplatesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly ITemplateRenderer _renderer;

        public DocumentTemplatesController(
            ApplicationDbContext db, 
            IWebHostEnvironment env,
            ITemplateRenderer renderer)
        {
            _db = db;
            _env = env;
            _renderer = renderer;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _db.DocumentTemplates.AsNoTracking().ToListAsync();
            return View(list);
        }

        public IActionResult Create()
        {
            return View(new DocumentTemplateEntity());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentTemplateEntity model, IFormFile? logo, IFormFile? signature)
        {
            if (!ModelState.IsValid)
                return View(model);

            // handle uploads
            var uploadsRoot = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "templates");
            if (!Directory.Exists(uploadsRoot)) Directory.CreateDirectory(uploadsRoot);

            if (logo != null && logo.Length > 0)
            {
                var fName = Path.GetRandomFileName() + Path.GetExtension(logo.FileName);
                var path = Path.Combine(uploadsRoot, fName);
                using var fs = System.IO.File.Create(path);
                await logo.CopyToAsync(fs);
                model.LogoPath = Path.Combine("uploads", "templates", fName).Replace("\\","/");
            }

            if (signature != null && signature.Length > 0)
            {
                var fName = Path.GetRandomFileName() + Path.GetExtension(signature.FileName);
                var path = Path.Combine(uploadsRoot, fName);
                using var fs = System.IO.File.Create(path);
                await signature.CopyToAsync(fs);
                model.SignaturePath = Path.Combine("uploads", "templates", fName).Replace("\\","/");
            }

            _db.DocumentTemplates.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var tpl = await _db.DocumentTemplates.FindAsync(id);
            if (tpl == null) return NotFound();
            return View(tpl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DocumentTemplateEntity model, IFormFile? logo, IFormFile? signature)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var tpl = await _db.DocumentTemplates.FindAsync(id);
            if (tpl == null) return NotFound();

            tpl.Name = model.Name;
            tpl.Content = model.Content;

            var uploadsRoot = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "templates");
            if (!Directory.Exists(uploadsRoot)) Directory.CreateDirectory(uploadsRoot);

            if (logo != null && logo.Length > 0)
            {
                var fName = Path.GetRandomFileName() + Path.GetExtension(logo.FileName);
                var path = Path.Combine(uploadsRoot, fName);
                using var fs = System.IO.File.Create(path);
                await logo.CopyToAsync(fs);
                tpl.LogoPath = Path.Combine("uploads", "templates", fName).Replace("\\","/");
            }

            if (signature != null && signature.Length > 0)
            {
                var fName = Path.GetRandomFileName() + Path.GetExtension(signature.FileName);
                var path = Path.Combine(uploadsRoot, fName);
                using var fs = System.IO.File.Create(path);
                await signature.CopyToAsync(fs);
                tpl.SignaturePath = Path.Combine("uploads", "templates", fName).Replace("\\","/");
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var tpl = await _db.DocumentTemplates.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            if (tpl == null) return NotFound();
            return View(tpl);
        }

        [HttpGet]
        public async Task<IActionResult> Render(int templateId, int employeeId)
        {
            var template = await _db.DocumentTemplates
                .Include(t => t.DocumentType)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == templateId);
            if (template == null) return NotFound("Plantilla no encontrada");

            var employee = await _db.Employees
                .Include(e => e.Company)
                .Include(e => e.Branch)
                .Include(e => e.Department)
                .Include(e => e.Division)
                .Include(e => e.Section)
                .Include(e => e.Project)
                .Include(e => e.Phase)
                .Include(e => e.CostCenter)
                .Include(e => e.Schedule)
                .Include(e => e.Position)
                .Include(e => e.EmployeeType)
                .Include(e => e.TypeOfWorker)
                .Include(e => e.PaymentGroup)
                .Include(e => e.IdentityDocumentType)
                .Include(e => e.EmployeeObservation)
                .Include(e => e.OriginCountry)
                .Include(e => e.Activity)
                .Include(e => e.Bank)
                .Include(e => e.PayingBank)
                .Include(e => e.PieceworkUnitType)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null) return NotFound("Empleado no encontrado");

            var html = await _renderer.RenderAsync(template, employee);
            return Json(new { html });
        }

        [HttpGet]
        public IActionResult Fields()
        {
            // Build dynamic field list from EmployeeEntity and its related simple navigation properties
            var result = new List<object>();
            var empType = typeof(_2FA.Data.Entities.EmployeeEntity);

            var simpleProps = new List<object>();
            foreach (var p in empType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var pt = p.PropertyType;
                Type underlying = Nullable.GetUnderlyingType(pt) ?? pt;
                if (underlying.IsPrimitive || underlying == typeof(string) || underlying == typeof(decimal) || underlying == typeof(DateTime) || underlying.IsEnum)
                {
                    simpleProps.Add(new { name = p.Name, placeholder = $"{{{{Employee.{p.Name}}}}}" });
                }
            }
            result.Add(new { group = "Employee", fields = simpleProps });

            // navigation properties (single-reference only)
            foreach (var nav in empType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var navType = nav.PropertyType;
                if (navType == typeof(string)) continue;
                if (typeof(System.Collections.IEnumerable).IsAssignableFrom(navType) && navType != typeof(string)) continue; // skip collections

                // consider only types in our Entities namespace
                if (navType.Namespace == "_2FA.Data.Entities" && navType != empType)
                {
                    var navFields = new List<object>();
                    foreach (var p in navType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        var pt = p.PropertyType;
                        Type underlying = Nullable.GetUnderlyingType(pt) ?? pt;
                        if (underlying.IsPrimitive || underlying == typeof(string) || underlying == typeof(decimal) || underlying == typeof(DateTime) || underlying.IsEnum)
                        {
                            navFields.Add(new { name = p.Name, placeholder = $"{{{{{nav.Name}.{p.Name}}}}}" });
                        }
                    }
                    if (navFields.Count > 0)
                    {
                        result.Add(new { group = nav.Name, fields = navFields });
                    }
                }
            }

            // add some global placeholders
            result.Add(new { group = "Global", fields = new[] { new { name = "Today", placeholder = "{{Today}}" }, new { name = "Signature", placeholder = "{{Signature}}" }, new { name = "Company.Logo", placeholder = "{{Company.Logo}}" } } });

            return Json(result);
        }
    }
}
