using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System;
using _2FA.Data;
using _2FA.Data.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _2FA.Controllers
{
    public class DocumentTypesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DocumentTypesController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _context.DocumentTypes.OrderBy(d => d.Name).ToListAsync();
            return View(items);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var entity = await _context.DocumentTypes.FindAsync(id.Value);
            if (entity == null) return NotFound();
            return View(entity);
        }

        public IActionResult Create()
        {
            return View(new DocumentTypeEntity());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentTypeEntity documentType, IFormFile? signatureFile, string? signatureData)
        {
            if (!ModelState.IsValid) return View(documentType);

            _context.Add(documentType);
            await _context.SaveChangesAsync();

            byte[] signatureBytes = Array.Empty<byte>();
            string contentType = "image/png";
            string fileName = "signature.png";

            if (!string.IsNullOrEmpty(signatureData) && signatureData.StartsWith("data:image/"))
            {
                // Handle drawn signature
                var base64Data = signatureData.Split(',')[1];
                signatureBytes = Convert.FromBase64String(base64Data);
                contentType = "image/png";
                fileName = "drawn_signature.png";
            }
            else if (signatureFile != null && signatureFile.Length > 0)
            {
                // Handle uploaded file
                using var memoryStream = new MemoryStream();
                await signatureFile.CopyToAsync(memoryStream);
                signatureBytes = memoryStream.ToArray();
                contentType = signatureFile.ContentType;
                fileName = signatureFile.FileName;
            }

            if (signatureBytes.Length > 0)
            {
                var signature = new DocumentTypeSignaturesEntity
                {
                    DocumentTypeId = documentType.Id,
                    SignerName = documentType.SignerName ?? "Default Signer",
                    SignerTitle = documentType.SignerTitle,
                    SignatureData = signatureBytes,
                    ContentType = contentType,
                    FileName = fileName,
                    SecurityHash = ComputeSha256Hash(signatureBytes),
                    SignatureOrder = 1
                };

                _context.DocumentTypeSignatures.Add(signature);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var entity = await _context.DocumentTypes.FindAsync(id.Value);
            if (entity == null) return NotFound();
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DocumentTypeEntity documentType, IFormFile? signatureFile, string? signatureData)
        {
            if (id != documentType.Id) return NotFound();
            if (!ModelState.IsValid) return View(documentType);

            var entity = await _context.DocumentTypes.FindAsync(id);
            if (entity == null) return NotFound();

            entity.Name = documentType.Name;
            entity.Description = documentType.Description;
            entity.SignerName = documentType.SignerName;
            entity.SignerTitle = documentType.SignerTitle;
            entity.IsActive = documentType.IsActive;

            byte[] signatureBytes = Array.Empty<byte>();
            string contentType = "image/png";
            string fileName = "signature.png";

            if (!string.IsNullOrEmpty(signatureData) && signatureData.StartsWith("data:image/"))
            {
                // Handle drawn signature
                var base64Data = signatureData.Split(',')[1];
                signatureBytes = Convert.FromBase64String(base64Data);
                contentType = "image/png";
                fileName = "drawn_signature.png";
            }
            else if (signatureFile != null && signatureFile.Length > 0)
            {
                // Handle uploaded file
                using var memoryStream = new MemoryStream();
                await signatureFile.CopyToAsync(memoryStream);
                signatureBytes = memoryStream.ToArray();
                contentType = signatureFile.ContentType;
                fileName = signatureFile.FileName;
            }

            if (signatureBytes.Length > 0)
            {
                var existingSignature = await _context.DocumentTypeSignatures
                    .FirstOrDefaultAsync(s => s.DocumentTypeId == id && s.IsActive);

                if (existingSignature != null)
                {
                    existingSignature.SignatureData = signatureBytes;
                    existingSignature.ContentType = contentType;
                    existingSignature.FileName = fileName;
                    existingSignature.SecurityHash = ComputeSha256Hash(signatureBytes);
                    _context.Update(existingSignature);
                }
                else
                {
                    var signature = new DocumentTypeSignaturesEntity
                    {
                        DocumentTypeId = documentType.Id,
                        SignerName = documentType.SignerName ?? "Default Signer",
                        SignerTitle = documentType.SignerTitle,
                        SignatureData = signatureBytes,
                        ContentType = contentType,
                        FileName = fileName,
                        SecurityHash = ComputeSha256Hash(signatureBytes),
                        SignatureOrder = 1
                    };
                    _context.DocumentTypeSignatures.Add(signature);
                }
            }

            try
            {
                _context.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocumentTypeExists(documentType.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var entity = await _context.DocumentTypes.FindAsync(id.Value);
            if (entity == null) return NotFound();
            return View(entity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await _context.DocumentTypes.FindAsync(id);
            if (entity != null)
            {
                _context.DocumentTypes.Remove(entity);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: DocumentTypes/Signatures/5
        public async Task<IActionResult> Signatures(int? id)
        {
            if (id == null) return NotFound();
            var documentType = await _context.DocumentTypes.FindAsync(id.Value);
            if (documentType == null) return NotFound();

            var signatures = await _context.DocumentTypeSignatures
                .Where(s => s.DocumentTypeId == id.Value)
                .OrderBy(s => s.SignatureOrder)
                .ToListAsync();

            ViewBag.DocumentType = documentType;
            return View(signatures);
        }

        // GET: DocumentTypes/CreateSignature/5
        public async Task<IActionResult> CreateSignature(int? documentTypeId)
        {
            if (documentTypeId == null) return NotFound();
            var documentType = await _context.DocumentTypes.FindAsync(documentTypeId.Value);
            if (documentType == null) return NotFound();

            var signature = new DocumentTypeSignaturesEntity
            {
                DocumentTypeId = documentTypeId.Value,
                SignatureOrder = await _context.DocumentTypeSignatures
                    .Where(s => s.DocumentTypeId == documentTypeId.Value)
                    .MaxAsync(s => (int?)s.SignatureOrder) + 1 ?? 1
            };

            ViewBag.DocumentType = documentType;
            return View(signature);
        }

        // POST: DocumentTypes/CreateSignature
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSignature(DocumentTypeSignaturesEntity signature, IFormFile? signatureFile, string? signatureData)
        {
            if (!ModelState.IsValid) return View(signature);

            byte[] signatureBytes = Array.Empty<byte>();
            string contentType = "image/png";
            string fileName = "signature.png";

            if (!string.IsNullOrEmpty(signatureData) && signatureData.StartsWith("data:image/"))
            {
                // Handle drawn signature
                var base64Data = signatureData.Split(',')[1];
                signatureBytes = Convert.FromBase64String(base64Data);
                contentType = "image/png";
                fileName = "drawn_signature.png";
            }
            else if (signatureFile != null && signatureFile.Length > 0)
            {
                // Handle uploaded file
                using var memoryStream = new MemoryStream();
                await signatureFile.CopyToAsync(memoryStream);
                signatureBytes = memoryStream.ToArray();
                contentType = signatureFile.ContentType;
                fileName = signatureFile.FileName;
            }

            if (signatureBytes.Length > 0)
            {
                signature.SignatureData = signatureBytes;
                signature.ContentType = contentType;
                signature.FileName = fileName;
                signature.SecurityHash = ComputeSha256Hash(signatureBytes);
            }

            _context.Add(signature);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Signatures), new { id = signature.DocumentTypeId });
        }

        // GET: DocumentTypes/List
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var list = await _context.DocumentTypes
                .Where(d => d.IsActive)
                .OrderBy(d => d.Name)
                .Select(d => new { d.Id, d.Name })
                .ToListAsync();
            return Json(list);
        }

        // GET: DocumentTypes/GetPrimarySignatureForType/5
        [HttpGet]
        public async Task<IActionResult> GetPrimarySignatureForType(int? documentTypeId)
        {
            if (documentTypeId == null) return NotFound();

            var sig = await _context.DocumentTypeSignatures
                .Where(s => s.DocumentTypeId == documentTypeId.Value && s.IsActive)
                .OrderBy(s => s.SignatureOrder)
                .FirstOrDefaultAsync();

            if (sig == null)
            {
                var dtEmpty = await _context.DocumentTypes.FindAsync(documentTypeId.Value);
                return Json(new { signatureUrl = (string?)null, signerName = dtEmpty?.SignerName, signerTitle = dtEmpty?.SignerTitle });
            }

            var url = Url.Action("GetSignature", new { id = sig.Id });
            var docType = await _context.DocumentTypes.FindAsync(documentTypeId.Value);
            return Json(new { signatureUrl = url, signerName = docType?.SignerName, signerTitle = docType?.SignerTitle });
        }

        // GET: DocumentTypes/GetSignature/5
        public async Task<IActionResult> GetSignature(int? id)
        {
            if (id == null) return NotFound();

            var signature = await _context.DocumentTypeSignatures.FindAsync(id.Value);
            if (signature == null) return NotFound();

            return File(signature.SignatureData, signature.ContentType, signature.FileName);
        }

        private string ComputeSha256Hash(byte[] data)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(data);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        private bool DocumentTypeExists(int id)
        {
            return _context.DocumentTypes.Any(e => e.Id == id);
        }
    }
}
