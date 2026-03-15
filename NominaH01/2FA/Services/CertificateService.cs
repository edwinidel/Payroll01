using System;
using System.IO;
using System.Threading.Tasks;
using _2FA.Data;
using _2FA.Data.Entities;
using _2FA.Models;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace _2FA.Services
{
    public class CertificateService : ICertificateService
    {
        private readonly ApplicationDbContext _context;
        private readonly string _basePath;

        public CertificateService(ApplicationDbContext context, IHostEnvironment env)
        {
            _context = context;
            _basePath = Path.Combine(env.ContentRootPath, "wwwroot", "certificates");
            Directory.CreateDirectory(_basePath);
        }

        public async Task<CertificateResponseDto> GenerateAsync(int employeeId, CertificateRequestDto request)
        {
            var employee = await _context.Employees
                .Include(e => e.Company)
                .Include(e => e.Position)
                .Include(e => e.Department)
                .Include(e => e.Section)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == employeeId)
                ?? throw new InvalidOperationException("Empleado no encontrado");

            var pdfBytes = BuildPdf(employee, request);
            var fileName = $"cert-{employee.Code}-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
            var destPath = Path.Combine(_basePath, fileName);
            await File.WriteAllBytesAsync(destPath, pdfBytes);

            var entity = new CertificateRequestEntity
            {
                EmployeeId = employeeId,
                Type = request.Type,
                SalaryHistoryMonths = request.SalaryHistoryMonths,
                FilePath = fileName,
                Created = DateTime.UtcNow
            };

            _context.CertificateRequests.Add(entity);
            await _context.SaveChangesAsync();

            var stream = new MemoryStream(pdfBytes);
            stream.Position = 0;
            return new CertificateResponseDto(fileName, "application/pdf", stream);
        }

        private static byte[] BuildPdf(EmployeeEntity employee, CertificateRequestDto request)
        {
            using var ms = new MemoryStream();
            using var writer = new PdfWriter(ms);
            using var pdf = new PdfDocument(writer);
            using var doc = new Document(pdf);

            var regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            doc.SetMargins(60, 60, 60, 60);
            doc.SetFont(regularFont);

            var companyName = employee.Company?.Name ?? "";
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                var companyParagraph = new Paragraph(companyName)
                    .SetFont(boldFont)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(14);
                doc.Add(companyParagraph);
            }

            doc.Add(new Paragraph("Certificado Laboral")
                .SetFont(boldFont)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(12));

            doc.Add(new Paragraph($"Ciudad y fecha: {DateTime.UtcNow:yyyy-MM-dd}")
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetFontSize(10));

            var fullName = employee.FullName;
            var position = employee.Position?.Name ?? "";
            var department = employee.Department?.Name ?? employee.Section?.Name ?? string.Empty;
            var hireDate = employee.HiringDate == default ? "" : employee.HiringDate.ToString("yyyy-MM-dd");
            var salary = employee.AgreeSalary.ToString("N2");

            var body = new Paragraph(
                $"A quien pueda interesar:\n\n" +
                $"Se certifica que {fullName} (cédula {employee.IdDocument}) labora en {companyName} " +
                $"desde {hireDate}, desempeñando el cargo de {position}. Su salario pactado es B/. {salary}.")
                .SetTextAlignment(TextAlignment.JUSTIFIED)
                .SetFontSize(11);
            doc.Add(body);

            if (request.IncludeSalaryHistoryMonths && request.SalaryHistoryMonths > 0)
            {
                var salaryNote = new Paragraph(
                    $"A solicitud del colaborador se deja constancia del promedio salarial de los últimos {request.SalaryHistoryMonths} meses.")
                    .SetTextAlignment(TextAlignment.JUSTIFIED)
                    .SetFontSize(10);
                doc.Add(salaryNote);
            }

            if (!string.IsNullOrWhiteSpace(department))
            {
                doc.Add(new Paragraph($"Área/Departamento: {department}").SetFontSize(10));
            }

            doc.Add(new Paragraph("\nEste certificado se expide para los fines que el interesado estime conveniente."));

            doc.Add(new Paragraph("\n\n______________________________\nRecursos Humanos"));

            doc.Close();
            return ms.ToArray();
        }
    }
}
