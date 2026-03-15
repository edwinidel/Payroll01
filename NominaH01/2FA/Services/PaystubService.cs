using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using _2FA.Data;
using _2FA.Data.Entities;
using _2FA.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace _2FA.Services
{
    public class PaystubService : IPaystubService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PaystubService> _logger;
        private readonly string _basePath;

        public PaystubService(ApplicationDbContext context, ILogger<PaystubService> logger, IHostEnvironment env)
        {
            _context = context;
            _logger = logger;
            _basePath = Path.Combine(env.ContentRootPath, "wwwroot", "paystubs");
            Directory.CreateDirectory(_basePath);
        }

        public async Task<IReadOnlyList<PaystubSummaryDto>> GetPaystubsAsync(int employeeId, DateOnly? from, DateOnly? to, int page, int pageSize)
        {
            var query = _context.Paystubs.AsNoTracking().Where(p => p.EmployeeId == employeeId);

            if (from.HasValue)
            {
                query = query.Where(p => p.PeriodStart >= from.Value.ToDateTime(TimeOnly.MinValue));
            }
            if (to.HasValue)
            {
                query = query.Where(p => p.PeriodEnd <= to.Value.ToDateTime(TimeOnly.MaxValue));
            }

            var items = await query
                .OrderByDescending(p => p.PeriodEnd)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PaystubSummaryDto(
                    p.Id,
                    DateOnly.FromDateTime(p.PeriodStart),
                    DateOnly.FromDateTime(p.PeriodEnd),
                    p.Gross,
                    p.Net,
                    p.Created))
                .ToListAsync();

            return items;
        }

        public async Task<PaystubDownloadDto> DownloadPaystubAsync(int employeeId, int paystubId)
        {
            var paystub = await _context.Paystubs.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == paystubId && p.EmployeeId == employeeId);

            if (paystub == null) throw new FileNotFoundException("Paystub not found");

            var fullPath = Path.IsPathRooted(paystub.FilePath)
                ? paystub.FilePath
                : Path.Combine(_basePath, paystub.FilePath);

            if (!File.Exists(fullPath)) throw new FileNotFoundException("Paystub file missing", fullPath);

            var stream = File.OpenRead(fullPath);
            var fileName = Path.GetFileName(fullPath);
            return new PaystubDownloadDto(fileName, "application/pdf", stream);
        }

        public async Task RegisterPaystubAsync(int employeeId, int payrollHeaderId, DateOnly start, DateOnly end, decimal gross, decimal net, Stream file, string fileName)
        {
            var safeName = Path.GetFileName(fileName);
            var destPath = Path.Combine(_basePath, safeName);
            using (var output = File.Create(destPath))
            {
                await file.CopyToAsync(output);
            }

            var entity = new PaystubEntity
            {
                EmployeeId = employeeId,
                PayrollHeaderId = payrollHeaderId,
                PeriodStart = start.ToDateTime(TimeOnly.MinValue),
                PeriodEnd = end.ToDateTime(TimeOnly.MaxValue),
                Gross = gross,
                Net = net,
                FilePath = safeName,
                Created = DateTime.UtcNow,
                IsLatest = true
            };

            // mark previous as not latest
            var previous = _context.Paystubs.Where(p => p.EmployeeId == employeeId && p.IsLatest);
            await previous.ForEachAsync(p => p.IsLatest = false);

            _context.Paystubs.Add(entity);
            await _context.SaveChangesAsync();
        }
    }
}
