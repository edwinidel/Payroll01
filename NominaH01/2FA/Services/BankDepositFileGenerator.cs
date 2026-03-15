using _2FA.Data;
using _2FA.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace _2FA.Services
{
    public class BankDepositFileGenerator
    {
        private readonly ApplicationDbContext _context;

        public BankDepositFileGenerator(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<byte[]> GenerateFileAsync(int structureId, int payrollId)
        {
            var payrollHeader = await _context.PayrollTmpHeaders
                .FirstOrDefaultAsync(ph => ph.Id == payrollId);
        
            if (payrollHeader == null)
                throw new ArgumentException("Payroll header not found");
        
            int companyId = payrollHeader.CompanyId;
        
            var structure = await _context.BankDepositStructures
                .Include(s => s.Fields.Where(f => f.CompanyId == companyId))
                .FirstOrDefaultAsync(s => s.Id == structureId);
        
            if (structure == null)
                throw new ArgumentException("Structure not found");
        
            var fields = structure.Fields.OrderBy(f => f.Order).ToList();
        
            // Get payroll data for the specific payroll header
            var payrollData = await _context.PayrollTmpEmployees
                .Include(p => p.Employee)
                    .ThenInclude(e => e.Bank)
                .Include(p => p.PayrollTmpHeader)
                .Where(p => p.PayrollTmpHeaderId == payrollId)
                .ToListAsync();
        
            var content = new System.Text.StringBuilder();
        
            if (structure.FileType == FileType.CSV)
            {
                // CSV header
                var headers = fields.Select(f => f.FieldName).ToArray();
                content.AppendLine(string.Join(",", headers));
        
                // Data rows
                foreach (var payroll in payrollData)
                {
                    var values = new List<string>();
                    foreach (var field in fields)
                    {
                        if (field.Ignore)
                        {
                            values.Add("");
                            continue;
                        }
        
                        var value = GetFieldValue(field.FieldContent, payroll);
                        // Escape commas in CSV
                        if (value.Contains(","))
                        {
                            value = $"\"{value.Replace("\"", "\"\"")}\"";
                        }
                        values.Add(value);
                    }
                    content.AppendLine(string.Join(",", values));
                }
            }
            else if (structure.FileType == FileType.TXT)
            {
                // Fixed width TXT
                foreach (var payroll in payrollData)
                {
                    foreach (var field in fields)
                    {
                        if (field.Ignore) continue;
        
                        var value = GetFieldValue(field.FieldContent, payroll);
                        // Assume fixed width, pad/truncate as needed
                        content.Append(value.PadRight(20).Substring(0, 20));
                    }
                    content.AppendLine();
                }
            }
            // For XLSX, would need a library like EPPlus, but for now return CSV
            else
            {
                throw new NotImplementedException("XLSX not implemented");
            }
        
            return System.Text.Encoding.UTF8.GetBytes(content.ToString());
        }

        private string GetFieldValue(string fieldContent, PayrollTmpEmployeeEntity payroll)
        {
            // Map field content to actual values
            switch (fieldContent)
            {
                case "Employee.FullName":
                    return payroll.Employee?.FullName ?? "";
                case "Employee.IdDocument":
                    return payroll.Employee?.IdDocument ?? "";
                case "Employee.Code":
                    return payroll.Employee?.Code ?? "";
                case "Employee.CEMAIL":
                    return payroll.Employee?.CEMAIL ?? "";
                case "Employee.PayAccount":
                    return payroll.Employee?.PayAccount ?? "";
                case "Banks.Name":
                    return payroll.Employee?.Bank?.Name ?? "";
                case "Banks.TransitBankId":
                    return payroll.Employee?.Bank?.TransitBankId.ToString() ?? "";
                case "PayrollTmpHeader.PaymentGroupId":
                    return payroll.PayrollTmpHeader?.PaymentGroupId.ToString() ?? "";
                case "PayrollTmpHeader.EndDate":
                    return payroll.PayrollTmpHeader?.EndDate.ToString("yyyy-MM-dd") ?? "";
                case "PayrollTmpEmployees.NetPay":
                    return payroll.NetPay.ToString("F2");
                default:
                    return "";
            }
        }
    }
}