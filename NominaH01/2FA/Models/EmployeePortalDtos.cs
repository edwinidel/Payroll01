using System;
using System.Collections.Generic;
using System.IO;

namespace _2FA.Models
{
    public record PaystubSummaryDto(int Id, DateOnly PeriodStart, DateOnly PeriodEnd, decimal Gross, decimal Net, DateTime CreatedAt);
    public record PaystubDownloadDto(string FileName, string ContentType, Stream Content);

    public record CertificateRequestDto(string Type, bool IncludeSalaryHistoryMonths = true, int SalaryHistoryMonths = 3);
    public record CertificateResponseDto(string FileName, string ContentType, Stream Content);

    public record LeaveBalanceDto(
        decimal VacationAvailableDays,
        decimal VacationPendingDays,
        decimal VacationTakenDays,
        decimal VacationDailyRate,
        decimal VacationBestMonthlyBase,
        decimal MedicalCertificatesAvailable);

    public record NotificationDto(int Id, string Type, string Title, string Body, string Severity, DateTime CreatedAt, DateTime? ReadAt, DateTime? ExpiresAt);
}
