# Employee Portal API draft (Sprint 1)

## Scope (Sprint 1)
- Recibos: listar y descargar paystubs del empleado autenticado.
- Certificados: generar PDF de trabajo/ingresos bajo plantillas parametrizadas.
- Saldos: vacaciones y certificados médicos disponibles.
- Notificaciones: bandeja in-app y generación automática (contratos, docs renovables, licencias, cumpleaños).

## DTOs (propuestos)
```csharp
public record PaystubSummaryDto(int Id, DateOnly PeriodStart, DateOnly PeriodEnd, decimal Gross, decimal Net, DateTime CreatedAt);
public record PaystubDownloadDto(string FileName, string ContentType, Stream Content);

public record CertificateRequestDto(string Type, bool IncludeSalaryHistoryMonths, int SalaryHistoryMonths = 3);
public record CertificateResponseDto(string FileName, string ContentType, Stream Content);

public record LeaveBalanceDto(decimal VacationAvailableDays, decimal VacationPendingDays, decimal MedicalCertificatesAvailable);

public record NotificationDto(int Id, string Type, string Title, string Body, string Severity, DateTime CreatedAt, DateTime? ReadAt, DateTime? ExpiresAt);
public record NotificationReadRequestDto(int Id);
```

## Entities (a agregar)
```csharp
public class PaystubEntity : BaseEntity
{
    public int EmployeeId { get; set; }
    public int PayrollHeaderId { get; set; }
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
    public decimal Gross { get; set; }
    public decimal Net { get; set; }
    public string FilePath { get; set; } = string.Empty; // blob/local
    public string Hash { get; set; } = string.Empty;
    public bool IsLatest { get; set; }
}

public class CertificateRequestEntity : BaseEntity
{
    public int EmployeeId { get; set; }
    public string Type { get; set; } = string.Empty; // work|income
    public int SalaryHistoryMonths { get; set; }
    public string FilePath { get; set; } = string.Empty;
}

public class NotificationEntity : BaseEntity
{
    public int EmployeeId { get; set; }
    public string Type { get; set; } = string.Empty; // contract-expiry, doc-expiry, leave-reminder, birthday
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Severity { get; set; } = "info"; // info|warning|critical
    public DateTime? ReadAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string Channel { get; set; } = "in-app"; // in-app|email
}

public class NotificationRuleEntity : BaseEntity
{
    public string Type { get; set; } = string.Empty;
    public int DaysBefore { get; set; }
    public bool Enabled { get; set; } = true;
    public string Channel { get; set; } = "in-app";
}
```

## Services (interfaces de dominio)
```csharp
public interface IPaystubService
{
    Task<IReadOnlyList<PaystubSummaryDto>> GetPaystubsAsync(int employeeId, DateOnly? from, DateOnly? to, int page, int pageSize);
    Task<PaystubDownloadDto> DownloadPaystubAsync(int employeeId, int paystubId);
    Task RegisterPaystubAsync(int employeeId, int payrollHeaderId, DateOnly start, DateOnly end, decimal gross, decimal net, Stream file, string fileName);
}

public interface ICertificateService
{
    Task<CertificateResponseDto> GenerateAsync(int employeeId, CertificateRequestDto request);
}

public interface IBalanceService
{
    Task<LeaveBalanceDto> GetBalancesAsync(int employeeId);
}

public interface INotificationService
{
    Task<IReadOnlyList<NotificationDto>> GetAsync(int employeeId, bool unreadOnly, int page, int pageSize);
    Task MarkAsReadAsync(int employeeId, int notificationId);
    Task EnqueueAsync(NotificationDto notification);
}
```

## API controllers (borradores)
```csharp
[ApiController]
[Route("api/employee/paystubs")]
[Authorize(Roles = "Empleado")]
public class EmployeePaystubsController : ControllerBase
{
    private readonly IPaystubService _service;
    public EmployeePaystubsController(IPaystubService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PaystubSummaryDto>>> List([FromQuery] DateOnly? from, [FromQuery] DateOnly? to, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var employeeId = User.RequireEmployeeId();
        return Ok(await _service.GetPaystubsAsync(employeeId, from, to, page, pageSize));
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(int id)
    {
        var employeeId = User.RequireEmployeeId();
        var file = await _service.DownloadPaystubAsync(employeeId, id);
        return File(file.Content, file.ContentType, file.FileName);
    }
}
```

```csharp
[ApiController]
[Route("api/employee/certificates")]
[Authorize(Roles = "Empleado")]
public class CertificatesController : ControllerBase
{
    private readonly ICertificateService _service;
    public CertificatesController(ICertificateService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CertificateRequestDto request)
    {
        var employeeId = User.RequireEmployeeId();
        var file = await _service.GenerateAsync(employeeId, request);
        return File(file.Content, file.ContentType, file.FileName);
    }
}
```

```csharp
[ApiController]
[Route("api/employee/balances")]
[Authorize(Roles = "Empleado")]
public class BalancesController : ControllerBase
{
    private readonly IBalanceService _service;
    public BalancesController(IBalanceService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<LeaveBalanceDto>> Get()
    {
        var employeeId = User.RequireEmployeeId();
        return Ok(await _service.GetBalancesAsync(employeeId));
    }
}
```

```csharp
[ApiController]
[Route("api/employee/notifications")]
[Authorize(Roles = "Empleado")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _service;
    public NotificationsController(INotificationService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<NotificationDto>>> List([FromQuery] bool unreadOnly = false, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var employeeId = User.RequireEmployeeId();
        return Ok(await _service.GetAsync(employeeId, unreadOnly, page, pageSize));
    }

    [HttpPost("{id}/read")]
    public async Task<IActionResult> MarkRead(int id)
    {
        var employeeId = User.RequireEmployeeId();
        await _service.MarkAsReadAsync(employeeId, id);
        return NoContent();
    }
}
```

## Notification scheduler (HostedService borrador)
```csharp
public class NotificationScheduler : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<NotificationScheduler> _logger;

    public NotificationScheduler(IServiceProvider sp, ILogger<NotificationScheduler> logger)
    {
        _sp = sp;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _sp.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<INotificationGenerationService>();
                await service.RunAsync(stoppingToken); // consulta contratos/docs/licencias y genera NotificationEntity
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "NotificationScheduler failed");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // daily
        }
    }
}
```

## Auth helper
```csharp
public static class ClaimsPrincipalExtensions
{
    public static int RequireEmployeeId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst("EmployeeId")?.Value;
        if (string.IsNullOrEmpty(value)) throw new UnauthorizedAccessException("EmployeeId claim missing");
        return int.Parse(value);
    }
}
```

## Open questions
- ¿Fuente de PDFs actuales de recibos? (si existen, sólo registrar/enlazar). Si no, decidir plantilla y renderizador (Razor -> PDF).
- ¿Almacenamiento de archivos? (local vs blob). Presigned links o streaming directo.
- ¿Canal de notificación email en Sprint 1 o sólo in-app? (in-app obligatorio).
- Frecuencia del scheduler (diario) y reglas por compañía.
- Claim `EmployeeId`: agregar en login (Identity) según usuario/empleado vinculado.
