using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Linq;
using System.Threading.Tasks;
using _2FA.Data;
using _2FA.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _2FA.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class OtcConfigurationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OtcConfigurationsController> _logger;

        public OtcConfigurationsController(
            ApplicationDbContext context,
            IHttpClientFactory httpClientFactory,
            ILogger<OtcConfigurationsController> logger)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        private string BuildSessionKey(string? key)
        {
            return $"OtcValidationToken-{(string.IsNullOrWhiteSpace(key) ? "new" : key)}";
        }

        private bool TokenIsValid(string token, string? sessionKey)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            var stored = HttpContext.Session.GetString(BuildSessionKey(sessionKey));
            return stored != null && stored == token;
        }

        private void ClearValidationState(string? sessionKey)
        {
            HttpContext.Session.Remove(BuildSessionKey(sessionKey));
        }

        private string ApplyVersionSegment(string endpoint, string version)
        {
            if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(version))
            {
                return endpoint;
            }

            // Replace placeholder forms /v{version}/ or v{version} case-insensitively
            var withSlash = endpoint.Replace("/v{version}/", $"/v{version}/", StringComparison.OrdinalIgnoreCase);
            return withSlash.Replace("v{version}", $"v{version}", StringComparison.OrdinalIgnoreCase);
        }

        // GET: OtcConfigurations
        public async Task<IActionResult> Index()
        {
            var items = await _context.OtcConfigurations
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(items);
        }

        // GET: OtcConfigurations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var otcConfigurationEntity = await _context.OtcConfigurations
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (otcConfigurationEntity == null)
            {
                return NotFound();
            }

            return View(otcConfigurationEntity);
        }

        // GET: OtcConfigurations/Create
        public IActionResult Create()
        {
            var sessionKey = Guid.NewGuid().ToString("N");
            ViewBag.ValidationSessionKey = sessionKey;
            return View(new OtcConfigurationEntity
            {
                IsActive = true,
                Version = "1",
                ValidationSessionKey = sessionKey
            });
        }

        // POST: OtcConfigurations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OtcConfigurationEntity otcConfigurationEntity)
        {
            otcConfigurationEntity.ValidationSessionKey = string.IsNullOrWhiteSpace(otcConfigurationEntity.ValidationSessionKey)
                ? "new"
                : otcConfigurationEntity.ValidationSessionKey;

            if (!TokenIsValid(otcConfigurationEntity.ValidationToken, otcConfigurationEntity.ValidationSessionKey))
            {
                ModelState.AddModelError(string.Empty, "Debe validar las credenciales del endpoint antes de guardar.");
            }

            if (ModelState.IsValid)
            {
                otcConfigurationEntity.Created = DateTime.UtcNow;
                otcConfigurationEntity.CreatedBy = User?.Identity?.Name ?? "system";

                _context.Add(otcConfigurationEntity);
                await _context.SaveChangesAsync();

                ClearValidationState(otcConfigurationEntity.ValidationSessionKey);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ValidationSessionKey = otcConfigurationEntity.ValidationSessionKey;
            return View(otcConfigurationEntity);
        }

        // GET: OtcConfigurations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var otcConfigurationEntity = await _context.OtcConfigurations.FindAsync(id);
            if (otcConfigurationEntity == null)
            {
                return NotFound();
            }

            var sessionKey = $"{otcConfigurationEntity.Id}-{Guid.NewGuid():N}";
            otcConfigurationEntity.ValidationSessionKey = sessionKey;
            ViewBag.ValidationSessionKey = sessionKey;
            return View(otcConfigurationEntity);
        }

        // POST: OtcConfigurations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OtcConfigurationEntity otcConfigurationEntity)
        {
            if (id != otcConfigurationEntity.Id)
            {
                return NotFound();
            }

            otcConfigurationEntity.ValidationSessionKey = string.IsNullOrWhiteSpace(otcConfigurationEntity.ValidationSessionKey)
                ? otcConfigurationEntity.Id.ToString()
                : otcConfigurationEntity.ValidationSessionKey;

            if (!TokenIsValid(otcConfigurationEntity.ValidationToken, otcConfigurationEntity.ValidationSessionKey))
            {
                ModelState.AddModelError(string.Empty, "Debe validar las credenciales del endpoint antes de guardar.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.OtcConfigurations.FirstOrDefaultAsync(c => c.Id == id);
                    if (existing == null)
                    {
                        return NotFound();
                    }

                    existing.Name = otcConfigurationEntity.Name;
                    existing.Version = otcConfigurationEntity.Version;
                    existing.RegisterEndpoint = otcConfigurationEntity.RegisterEndpoint;
                    existing.LoginEndpoint = otcConfigurationEntity.LoginEndpoint;
                    existing.CalculateEndpoint = otcConfigurationEntity.CalculateEndpoint;
                    existing.Username = otcConfigurationEntity.Username;
                    existing.Password = otcConfigurationEntity.Password;
                    existing.IsActive = otcConfigurationEntity.IsActive;
                    existing.Modified = DateTime.UtcNow;
                    existing.ModifiedBy = User?.Identity?.Name ?? "system";

                    await _context.SaveChangesAsync();
                    ClearValidationState(otcConfigurationEntity.ValidationSessionKey);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OtcConfigurationEntityExists(otcConfigurationEntity.Id))
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

            ViewBag.ValidationSessionKey = otcConfigurationEntity.ValidationSessionKey;
            return View(otcConfigurationEntity);
        }

        // GET: OtcConfigurations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var otcConfigurationEntity = await _context.OtcConfigurations
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (otcConfigurationEntity == null)
            {
                return NotFound();
            }

            return View(otcConfigurationEntity);
        }

        // POST: OtcConfigurations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var otcConfigurationEntity = await _context.OtcConfigurations.FindAsync(id);
            if (otcConfigurationEntity != null)
            {
                _context.OtcConfigurations.Remove(otcConfigurationEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidateAccess([FromBody] ValidateAccessRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.LoginEndpoint))
            {
                return BadRequest(new { success = false, message = "Debe especificar el endpoint de inicio de sesión." });
            }

            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { success = false, message = "Debe ingresar el usuario y la contraseña." });
            }

            if (string.IsNullOrWhiteSpace(request.Version))
            {
                return BadRequest(new { success = false, message = "Debe especificar la versión a utilizar." });
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var loginEndpoint = ApplyVersionSegment(request.LoginEndpoint, request.Version);

                using var httpRequest = new HttpRequestMessage(HttpMethod.Post, loginEndpoint)
                {
                    Content = JsonContent.Create(new { username = request.Username, password = request.Password, version = request.Version })
                };

                var response = await client.SendAsync(httpRequest);
                if (!response.IsSuccessStatusCode)
                {
                    var failMessage = $"El endpoint devolvió el estado {(int)response.StatusCode} ({response.ReasonPhrase}).";
                    return StatusCode((int)response.StatusCode, new { success = false, message = failMessage });
                }

                var validationToken = Guid.NewGuid().ToString("N");
                var sessionKey = BuildSessionKey(request.SessionKey);
                HttpContext.Session.SetString(sessionKey, validationToken);

                return Json(new { success = true, validationToken });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar acceso al endpoint de OTC");
                return StatusCode(500, new { success = false, message = "No se pudo validar el acceso al endpoint." });
            }
        }

        private bool OtcConfigurationEntityExists(int id)
        {
            return _context.OtcConfigurations.Any(e => e.Id == id);
        }

        public class ValidateAccessRequest
        {
            public string LoginEndpoint { get; set; } = string.Empty;
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string Version { get; set; } = string.Empty;
            public string SessionKey { get; set; } = string.Empty;
        }
    }
}
