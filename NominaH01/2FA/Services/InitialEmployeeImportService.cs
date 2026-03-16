using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using _2FA.Data;
using _2FA.Data.Entities;
using _2FA.Models;

namespace _2FA.Services
{
    public class InitialEmployeeImportService
    {
        private readonly ApplicationDbContext _context;

        private static readonly string[] TemplateHeaders =
        {
            "Codigo", "IdReloj", "Nombres", "Apellidos", "Cedula", "DV", "FechaIngreso", "FechaNacimiento",
            "Genero", "EstadoCivil", "SeguroSocial", "Correo", "TelefonoFijo", "Celular", "Direccion1", "Direccion2",
            "Estatus", "TipoSalario", "HorasRegulares", "SalarioPactado", "SalarioHora", "ValorUnidad", "UnidadDestajo",
            "RetenerISR", "MetodoISR", "ISRFijo", "ISRAdicional", "DeclaraISR", "PorcentajeDescuento", "Sindicalizado",
            "TipoCuentaPago", "CuentaPago", "Banco", "BancoPagador", "UsuarioApiOtc", "PasswordApiOtc",
            "TipoDocumento", "Horario", "Cargo", "TipoEmpleado", "Sucursal", "Departamento", "Seccion",
            "CentroCosto", "Division", "Proyecto", "Fase", "Actividad", "PaisOrigen", "TipoTrabajador",
            "GrupoPago", "ObservacionEmpleado", "EsContratista", "FinContrato", "FechaCesantia", "UltimoAumento"
        };

        public InitialEmployeeImportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GetPrerequisiteIssuesAsync(int companyId)
        {
            var issues = new List<string>();

            if (!await _context.Companies.AnyAsync(c => c.Id == companyId))
            {
                issues.Add("La compañía seleccionada no existe.");
                return issues;
            }

            if (!await _context.Branches.AnyAsync(x => x.CompanyId == companyId)) issues.Add("La compañía no tiene sucursales configuradas.");
            if (!await _context.Departments.AnyAsync(x => x.CompanyId == companyId)) issues.Add("La compañía no tiene departamentos configurados.");
            if (!await _context.Sections.AnyAsync(x => x.CompanyId == companyId)) issues.Add("La compañía no tiene secciones configuradas.");
            if (!await _context.CostCenters.AnyAsync(x => x.CompanyId == companyId)) issues.Add("La compañía no tiene centros de costo configurados.");
            if (!await _context.Divisions.AnyAsync(x => x.CompanyId == companyId)) issues.Add("La compañía no tiene divisiones configuradas.");
            if (!await _context.Projects.AnyAsync(x => x.CompanyId == companyId)) issues.Add("La compañía no tiene proyectos configurados.");
            if (!await _context.Phases.AnyAsync(x => x.CompanyId == companyId)) issues.Add("La compañía no tiene fases configuradas.");
            if (!await _context.Positions.AnyAsync()) issues.Add("No hay cargos configurados.");
            if (!await _context.Schedules.AnyAsync()) issues.Add("No hay horarios configurados.");
            if (!await _context.EmployeeTypes.AnyAsync()) issues.Add("No hay tipos de empleados configurados.");
            if (!await _context.IdentityDocumentTypes.AnyAsync()) issues.Add("No hay tipos de documento configurados.");
            if (!await _context.TypeOfWorkers.AnyAsync()) issues.Add("No hay tipos de trabajador configurados.");
            if (!await _context.PaymentGroups.AnyAsync()) issues.Add("No hay grupos de pago configurados.");
            if (!await _context.Countries.AnyAsync()) issues.Add("No hay países configurados.");
            if (!await _context.Banks.AnyAsync()) issues.Add("No hay bancos configurados.");

            return issues;
        }

        public async Task<byte[]> BuildTemplateAsync(int companyId)
        {
            var lookups = await LoadLookupsAsync(companyId);

            using var workbook = new XLWorkbook();

            var instructionsSheet = workbook.Worksheets.Add("Instrucciones");
            instructionsSheet.Cell(1, 1).Value = "Importación inicial de empleados";
            instructionsSheet.Cell(2, 1).Value = "Campos mínimos recomendados:";
            instructionsSheet.Cell(3, 1).Value = "Codigo, Nombres, Apellidos, Cedula, FechaIngreso, TipoSalario, HorasRegulares y el salario correspondiente.";
            instructionsSheet.Cell(4, 1).Value = "Las columnas de catálogos aceptan nombre o ID. Si se dejan en blanco, el sistema usa el primer valor configurado para la compañía.";
            instructionsSheet.Cell(5, 1).Value = "Fechas: use celdas de fecha de Excel o formato yyyy-MM-dd.";
            instructionsSheet.Cell(6, 1).Value = "Booleanos: Sí/Si/No, True/False, 1/0.";
            instructionsSheet.Cell(7, 1).Value = "TipoSalario: Mensual, Hora, Destajo.";
            instructionsSheet.Cell(8, 1).Value = "MetodoISR: P, T o F.";
            instructionsSheet.Columns().AdjustToContents();

            var employeeSheet = workbook.Worksheets.Add("Empleados");
            for (var i = 0; i < TemplateHeaders.Length; i++)
            {
                employeeSheet.Cell(1, i + 1).Value = TemplateHeaders[i];
                employeeSheet.Cell(1, i + 1).Style.Font.Bold = true;
                employeeSheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
            }

            employeeSheet.SheetView.FreezeRows(1);
            employeeSheet.Columns().AdjustToContents();

            var catalogsSheet = workbook.Worksheets.Add("Catalogos");
            WriteCatalogColumn(catalogsSheet, 1, "Horario", lookups.Schedules);
            WriteCatalogColumn(catalogsSheet, 2, "Sucursal", lookups.Branches);
            WriteCatalogColumn(catalogsSheet, 3, "Departamento", lookups.Departments);
            WriteCatalogColumn(catalogsSheet, 4, "Seccion", lookups.Sections);
            WriteCatalogColumn(catalogsSheet, 5, "CentroCosto", lookups.CostCenters);
            WriteCatalogColumn(catalogsSheet, 6, "Division", lookups.Divisions);
            WriteCatalogColumn(catalogsSheet, 7, "Proyecto", lookups.Projects);
            WriteCatalogColumn(catalogsSheet, 8, "Fase", lookups.Phases);
            WriteCatalogColumn(catalogsSheet, 9, "Actividad", lookups.Activities);
            WriteCatalogColumn(catalogsSheet, 10, "Cargo", lookups.Positions);
            WriteCatalogColumn(catalogsSheet, 11, "TipoEmpleado", lookups.EmployeeTypes);
            WriteCatalogColumn(catalogsSheet, 12, "PaisOrigen", lookups.Countries);
            WriteCatalogColumn(catalogsSheet, 13, "TipoTrabajador", lookups.TypeOfWorkers);
            WriteCatalogColumn(catalogsSheet, 14, "Banco", lookups.Banks);
            WriteCatalogColumn(catalogsSheet, 15, "TipoDocumento", lookups.IdentityDocumentTypes);
            WriteCatalogColumn(catalogsSheet, 16, "GrupoPago", lookups.PaymentGroups);
            WriteCatalogColumn(catalogsSheet, 17, "UnidadDestajo", lookups.PieceworkUnitTypes);
            catalogsSheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<List<InitialEmployeeImportRowViewModel>> PreviewAsync(Stream stream, int companyId)
        {
            var lookups = await LoadLookupsAsync(companyId);
            var rows = new List<InitialEmployeeImportRowViewModel>();

            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.FirstOrDefault(w => NormalizeKey(w.Name) == NormalizeKey("Empleados")) ?? workbook.Worksheet(1);
            var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 0;
            if (lastRow < 2)
            {
                return rows;
            }

            var headerMap = BuildHeaderMap(worksheet.Row(1));
            ValidateRequiredHeaders(headerMap);

            var existingCodes = await _context.Employees
                .Where(e => e.CompanyId == companyId && !e.IsDeleted && !string.IsNullOrWhiteSpace(e.Code))
                .Select(e => e.Code)
                .ToListAsync();
            var existingDocuments = await _context.Employees
                .Where(e => e.CompanyId == companyId && !e.IsDeleted)
                .Select(e => e.IdDocument)
                .ToListAsync();
            var existingClockIds = await _context.Employees
                .Where(e => e.CompanyId == companyId && !e.IsDeleted && !string.IsNullOrWhiteSpace(e.CodOfClock))
                .Select(e => e.CodOfClock)
                .ToListAsync();

            var existingCodeSet = new HashSet<string>(existingCodes.Where(x => !string.IsNullOrWhiteSpace(x)).Select(NormalizeValue));
            var existingDocumentSet = new HashSet<string>(existingDocuments.Where(x => !string.IsNullOrWhiteSpace(x)).Select(NormalizeValue));
            var existingClockSet = new HashSet<string>(existingClockIds.Where(x => !string.IsNullOrWhiteSpace(x)).Select(NormalizeValue));
            var fileCodeSet = new HashSet<string>();
            var fileDocumentSet = new HashSet<string>();
            var fileClockSet = new HashSet<string>();

            for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
            {
                var row = worksheet.Row(rowNumber);
                if (IsRowEmpty(row, headerMap))
                {
                    continue;
                }

                var importRow = new InitialEmployeeImportRowViewModel { RowNumber = rowNumber };
                var errors = new List<string>();
                var notes = new List<string>();

                importRow.Code = ReadString(row, headerMap, "CODIGO");
                importRow.CodOfClock = ReadString(row, headerMap, "IDRELOJ");
                importRow.FirstName = ReadString(row, headerMap, "NOMBRES");
                importRow.LastName = ReadString(row, headerMap, "APELLIDOS");
                importRow.IdDocument = ReadString(row, headerMap, "CEDULA");
                importRow.Dv = ReadString(row, headerMap, "DV");
                importRow.Genere = ReadString(row, headerMap, "GENERO");
                importRow.CivilStatus = ResolveString(ReadString(row, headerMap, "ESTADOCIVIL"), "Soltero", "EstadoCivil", notes);
                importRow.SocSecNum = ReadString(row, headerMap, "SEGUROSOCIAL");
                importRow.CEMAIL = ReadString(row, headerMap, "CORREO");
                importRow.FixedPhone = ReadString(row, headerMap, "TELEFONOFIJO");
                importRow.CellPhone = ReadString(row, headerMap, "CELULAR");
                importRow.CDIR1 = ReadString(row, headerMap, "DIRECCION1");
                importRow.CDIR2 = ReadString(row, headerMap, "DIRECCION2");
                importRow.Status = ResolveString(ReadString(row, headerMap, "ESTATUS"), "Activo", "Estatus", notes);
                importRow.SalaryType = ResolveString(ReadString(row, headerMap, "TIPOSALARIO"), "Mensual", "TipoSalario", notes);
                importRow.BloodType = ResolveString(ReadString(row, headerMap, "TIPOSANGRE"), "N/A", "TipoSangre", notes);
                importRow.IsrGroup = ResolveString(ReadString(row, headerMap, "GRUPOISR"), "A0", "GrupoISR", notes);
                importRow.IsrMethod = ResolveString(ReadString(row, headerMap, "METODOISR"), "P", "MetodoISR", notes);
                importRow.PayAccountType = ResolveString(ReadString(row, headerMap, "TIPOCUENTAPAGO"), "Ahorro", "TipoCuentaPago", notes);
                importRow.PayAccount = ReadString(row, headerMap, "CUENTAPAGO");
                importRow.ApiOtcUsername = ReadString(row, headerMap, "USUARIOAPIOTC");
                importRow.ApiOtcPassword = ReadString(row, headerMap, "PASSWORDAPIOTC");
                importRow.UnitType = string.Empty;

                var hiringDate = ReadDate(row, headerMap, "FECHAINGRESO", errors, true, "FechaIngreso");
                importRow.HiringDate = hiringDate ?? DateTime.Today;
                importRow.DateOfBird = ReadDate(row, headerMap, "FECHANACIMIENTO", errors, false, "FechaNacimiento");
                importRow.EndOfContract = ReadDate(row, headerMap, "FINCONTRATO", errors, false, "FinContrato") ?? DateTime.MinValue;
                importRow.LiquidationDate = ReadDate(row, headerMap, "FECHACESANTIA", errors, false, "FechaCesantia") ?? DateTime.MinValue;
                importRow.LastSalaryIncrease = ReadDate(row, headerMap, "ULTIMOAUMENTO", errors, false, "UltimoAumento") ?? importRow.HiringDate;

                importRow.RegularHours = ReadDecimal(row, headerMap, "HORASREGULARES", errors, true, "HorasRegulares") ?? 0m;
                importRow.AgreeSalary = ReadDecimal(row, headerMap, "SALARIOPACTADO", errors, false, "SalarioPactado") ?? 0m;
                importRow.HourSalary = ReadDecimal(row, headerMap, "SALARIOHORA", errors, false, "SalarioHora") ?? 0m;
                importRow.UnitValue = ReadDecimal(row, headerMap, "VALORUNIDAD", errors, false, "ValorUnidad") ?? 0m;
                importRow.IsrFixed = ReadDecimal(row, headerMap, "ISRFIJO", errors, false, "ISRFijo") ?? 0m;
                importRow.AdditionalIsr = ReadDecimal(row, headerMap, "ISRADICIONAL", errors, false, "ISRAdicional") ?? 0m;
                importRow.DiscountPercentage = ReadDecimal(row, headerMap, "PORCENTAJEDESCUENTO", errors, false, "PorcentajeDescuento") ?? 0m;

                importRow.RetainISR = ReadBool(row, headerMap, "RETENERISR", false, notes, "RetenerISR");
                importRow.DeclareIsr = ReadBool(row, headerMap, "DECLARAISR", false, notes, "DeclaraISR");
                importRow.Unionized = ReadBool(row, headerMap, "SINDICALIZADO", false, notes, "Sindicalizado");
                importRow.IsContractor = ReadBool(row, headerMap, "ESCONTRATISTA", false, notes, "EsContratista");

                if (string.IsNullOrWhiteSpace(importRow.Code)) errors.Add("El código es obligatorio.");
                if (string.IsNullOrWhiteSpace(importRow.FirstName)) errors.Add("El nombre es obligatorio.");
                if (string.IsNullOrWhiteSpace(importRow.LastName)) errors.Add("El apellido es obligatorio.");
                if (string.IsNullOrWhiteSpace(importRow.IdDocument)) errors.Add("La cédula es obligatoria.");

                var normalizedSalaryType = NormalizeValue(importRow.SalaryType);
                if (normalizedSalaryType != NormalizeValue("Mensual") && normalizedSalaryType != NormalizeValue("Hora") && normalizedSalaryType != NormalizeValue("Destajo"))
                {
                    errors.Add("TipoSalario debe ser Mensual, Hora o Destajo.");
                }

                if (normalizedSalaryType == NormalizeValue("Hora") && importRow.HourSalary <= 0)
                {
                    errors.Add("SalarioHora es obligatorio cuando TipoSalario es Hora.");
                }

                if (normalizedSalaryType == NormalizeValue("Destajo"))
                {
                    if (importRow.UnitValue <= 0)
                    {
                        errors.Add("ValorUnidad es obligatorio cuando TipoSalario es Destajo.");
                    }
                }
                else if (importRow.AgreeSalary <= 0)
                {
                    errors.Add("SalarioPactado debe ser mayor que cero para salarios Mensual u Hora.");
                }

                if (!string.IsNullOrWhiteSpace(importRow.Code))
                {
                    var normalizedCode = NormalizeValue(importRow.Code);
                    if (existingCodeSet.Contains(normalizedCode)) errors.Add("El código ya existe en la compañía.");
                    if (!fileCodeSet.Add(normalizedCode)) errors.Add("El código está duplicado dentro del archivo.");
                }

                if (!string.IsNullOrWhiteSpace(importRow.IdDocument))
                {
                    var normalizedIdDocument = NormalizeValue(importRow.IdDocument);
                    if (existingDocumentSet.Contains(normalizedIdDocument)) errors.Add("La cédula ya existe en la compañía.");
                    if (!fileDocumentSet.Add(normalizedIdDocument)) errors.Add("La cédula está duplicada dentro del archivo.");
                }

                if (!string.IsNullOrWhiteSpace(importRow.CodOfClock))
                {
                    var normalizedClock = NormalizeValue(importRow.CodOfClock);
                    if (existingClockSet.Contains(normalizedClock)) errors.Add("El IdReloj ya existe en la compañía.");
                    if (!fileClockSet.Add(normalizedClock)) errors.Add("El IdReloj está duplicado dentro del archivo.");
                }

                importRow.ScheduleId = ResolveLookup(ReadString(row, headerMap, "HORARIO"), "Horario", lookups.Schedules, errors, notes);
                importRow.BranchId = ResolveLookup(ReadString(row, headerMap, "SUCURSAL"), "Sucursal", lookups.Branches, errors, notes);
                importRow.DepartmentId = ResolveLookup(ReadString(row, headerMap, "DEPARTAMENTO"), "Departamento", lookups.Departments, errors, notes);
                importRow.SectionId = ResolveLookup(ReadString(row, headerMap, "SECCION"), "Seccion", lookups.Sections, errors, notes);
                importRow.CostCenterId = ResolveLookup(ReadString(row, headerMap, "CENTROCOSTO"), "CentroCosto", lookups.CostCenters, errors, notes);
                importRow.DivisionId = ResolveLookup(ReadString(row, headerMap, "DIVISION"), "Division", lookups.Divisions, errors, notes);
                importRow.ProjectId = ResolveLookup(ReadString(row, headerMap, "PROYECTO"), "Proyecto", lookups.Projects, errors, notes);
                importRow.PhaseId = ResolveLookup(ReadString(row, headerMap, "FASE"), "Fase", lookups.Phases, errors, notes);
                importRow.ActivityId = ResolveLookupNullable(ReadString(row, headerMap, "ACTIVIDAD"), "Actividad", lookups.Activities, notes);
                importRow.PositionId = ResolveLookup(ReadString(row, headerMap, "CARGO"), "Cargo", lookups.Positions, errors, notes);
                importRow.EmployeeTypeId = ResolveLookup(ReadString(row, headerMap, "TIPOEMPLEADO"), "TipoEmpleado", lookups.EmployeeTypes, errors, notes);
                importRow.OriginCountryId = ResolveLookup(ReadString(row, headerMap, "PAISORIGEN"), "PaisOrigen", lookups.Countries, errors, notes);
                importRow.TypeOfWorkerId = ResolveLookup(ReadString(row, headerMap, "TIPOTRABAJADOR"), "TipoTrabajador", lookups.TypeOfWorkers, errors, notes);
                importRow.BankId = ResolveLookup(ReadString(row, headerMap, "BANCO"), "Banco", lookups.Banks, errors, notes);
                importRow.PayingBankId = ResolveLookupNullable(ReadString(row, headerMap, "BANCOPAGADOR"), "BancoPagador", lookups.Banks, notes) ?? importRow.BankId;
                importRow.IdentityDocumentTypeId = ResolveLookup(ReadString(row, headerMap, "TIPODOCUMENTO"), "TipoDocumento", lookups.IdentityDocumentTypes, errors, notes);
                importRow.PaymentGroupId = ResolveLookup(ReadString(row, headerMap, "GRUPOPAGO"), "GrupoPago", lookups.PaymentGroups, errors, notes);
                importRow.EmployeeObservationId = 0;
                if (!string.IsNullOrWhiteSpace(ReadString(row, headerMap, "OBSERVACIONEMPLEADO")))
                {
                    notes.Add("ObservacionEmpleado se omite en la importación inicial y queda en 0.");
                }

                if (normalizedSalaryType == NormalizeValue("Destajo"))
                {
                    importRow.PieceworkUnitTypeId = ResolveLookupNullable(ReadString(row, headerMap, "UNIDADDESTAJO"), "UnidadDestajo", lookups.PieceworkUnitTypes, notes);
                    if (!importRow.PieceworkUnitTypeId.HasValue)
                    {
                        importRow.PieceworkUnitTypeId = lookups.PieceworkUnitTypes.FirstOrDefault()?.Id;
                    }

                    importRow.UnitType = lookups.PieceworkUnitTypes.FirstOrDefault(x => x.Id == importRow.PieceworkUnitTypeId)?.Name ?? string.Empty;
                }
                else
                {
                    var noAplica = lookups.PieceworkUnitTypes.FirstOrDefault(x => NormalizeValue(x.Name) == NormalizeValue("No Aplica"));
                    importRow.PieceworkUnitTypeId = noAplica?.Id;
                    importRow.UnitType = noAplica?.Name ?? string.Empty;
                }

                if (string.IsNullOrWhiteSpace(importRow.CEMAIL))
                {
                    notes.Add("Correo quedó vacío.");
                }

                importRow.IsValid = !errors.Any();
                importRow.ErrorMessage = string.Join(" ", errors);
                importRow.Notes = string.Join(" | ", notes.Distinct());

                rows.Add(importRow);
            }

            return rows;
        }

        public async Task<(int ImportedCount, List<string> Errors)> SaveAsync(int companyId, IEnumerable<InitialEmployeeImportRowViewModel> rows, string userName)
        {
            var validRows = rows.Where(r => r.IsValid).ToList();
            var errors = new List<string>();

            if (!validRows.Any())
            {
                errors.Add("No hay filas válidas para importar.");
                return (0, errors);
            }

            var company = await _context.Companies
                .Include(c => c.BusinessGroup)
                .FirstOrDefaultAsync(c => c.Id == companyId);

            if (company == null)
            {
                errors.Add("No se encontró la compañía seleccionada.");
                return (0, errors);
            }

            var groupId = company.BusinessGroupId;
            if (groupId != 0)
            {
                var maxEmployees = company.BusinessGroup?.MaxEmployees;
                if (!maxEmployees.HasValue)
                {
                    maxEmployees = await _context.BusinessGroups
                        .Where(bg => bg.Id == groupId)
                        .Select(bg => bg.MaxEmployees)
                        .FirstOrDefaultAsync();
                }

                if (maxEmployees.HasValue)
                {
                    var companiesInGroup = await _context.Companies
                        .Where(c => c.BusinessGroupId == groupId && c.IsActive && !c.IsDeleted)
                        .Select(c => c.Id)
                        .ToListAsync();

                    var currentActiveEmployees = await _context.Employees
                        .CountAsync(e => companiesInGroup.Contains(e.CompanyId) && !e.IsDeleted && !string.Equals(e.Status, "cesante", StringComparison.OrdinalIgnoreCase));

                    if (currentActiveEmployees + validRows.Count > maxEmployees.Value)
                    {
                        errors.Add($"La importación supera el límite del grupo de negocio. Límite: {maxEmployees.Value}, actuales: {currentActiveEmployees}, a importar: {validRows.Count}.");
                        return (0, errors);
                    }
                }
            }

            var importedCount = 0;
            var utcNow = DateTime.UtcNow;

            foreach (var row in validRows)
            {
                try
                {
                    var duplicateExists = await _context.Employees.AnyAsync(e =>
                        e.CompanyId == companyId &&
                        !e.IsDeleted &&
                        (e.IdDocument == row.IdDocument || (!string.IsNullOrWhiteSpace(row.Code) && e.Code == row.Code)));

                    if (duplicateExists)
                    {
                        errors.Add($"Fila {row.RowNumber}: el empleado ya existe en la compañía.");
                        continue;
                    }

                    var employee = new EmployeeEntity
                    {
                        CompanyId = companyId,
                        Code = row.Code,
                        ScheduleId = row.ScheduleId,
                        CodOfClock = row.CodOfClock,
                        FirstName = row.FirstName,
                        LastName = row.LastName,
                        BranchId = row.BranchId,
                        DepartmentId = row.DepartmentId,
                        CostCenterId = row.CostCenterId,
                        SectionId = row.SectionId,
                        DivisionId = row.DivisionId,
                        ProjectId = row.ProjectId,
                        PhaseId = row.PhaseId,
                        ActivityId = row.ActivityId,
                        PositionId = row.PositionId,
                        DateOfBird = row.DateOfBird,
                        IdDocument = row.IdDocument,
                        Dv = row.Dv,
                        CivilStatus = row.CivilStatus,
                        Genere = row.Genere,
                        SocSecNum = row.SocSecNum,
                        IsrGroup = row.IsrGroup,
                        EmployeeTypeId = row.EmployeeTypeId,
                        OriginCountryId = row.OriginCountryId,
                        BloodType = row.BloodType,
                        CDIR1 = row.CDIR1,
                        CDIR2 = row.CDIR2,
                        CEMAIL = row.CEMAIL,
                        FixedPhone = row.FixedPhone,
                        CellPhone = row.CellPhone,
                        Status = row.Status,
                        SalaryType = row.SalaryType,
                        RegularHours = row.RegularHours,
                        AgreeSalary = row.AgreeSalary,
                        HourSalary = row.HourSalary,
                        UnitValue = row.UnitValue,
                        PieceworkUnitTypeId = row.PieceworkUnitTypeId,
                        UnitType = row.UnitType,
                        RetainISR = row.RetainISR,
                        IsrMethod = row.IsrMethod,
                        IsrFixed = row.IsrFixed,
                        AdditionalIsr = row.AdditionalIsr,
                        DeclareIsr = row.DeclareIsr,
                        DiscountPercentage = row.DiscountPercentage,
                        Unionized = row.Unionized,
                        TypeOfWorkerId = row.TypeOfWorkerId,
                        PayAccountType = row.PayAccountType,
                        PayAccount = row.PayAccount,
                        BankId = row.BankId,
                        PaymentMethod = row.PaymentMethod,
                        PayingBankId = row.PayingBankId,
                        ApiOtcUsername = row.ApiOtcUsername,
                        ApiOtcPassword = row.ApiOtcPassword,
                        IdentityDocumentTypeId = row.IdentityDocumentTypeId,
                        HiringDate = row.HiringDate,
                        LiquidationDate = row.LiquidationDate,
                        EndOfContract = row.EndOfContract,
                        LastSalaryIncrease = row.LastSalaryIncrease,
                        EmployeeObservationId = row.EmployeeObservationId,
                        PaymentGroupId = row.PaymentGroupId,
                        IsContractor = row.IsContractor,
                        Created = utcNow,
                        CreatedBy = userName,
                        IsDeleted = false
                    };

                    _context.Employees.Add(employee);
                    importedCount++;
                }
                catch (Exception ex)
                {
                    errors.Add($"Fila {row.RowNumber}: {ex.Message}");
                }
            }

            if (importedCount > 0)
            {
                await _context.SaveChangesAsync();
            }

            return (importedCount, errors);
        }

        private static void WriteCatalogColumn(IXLWorksheet sheet, int column, string title, IReadOnlyList<LookupValue> values)
        {
            sheet.Cell(1, column).Value = title;
            sheet.Cell(1, column).Style.Font.Bold = true;
            sheet.Cell(1, column).Style.Fill.BackgroundColor = XLColor.LightGreen;

            for (var i = 0; i < values.Count; i++)
            {
                sheet.Cell(i + 2, column).Value = $"{values[i].Id} - {values[i].Name}";
            }
        }

        private async Task<LookupContext> LoadLookupsAsync(int companyId)
        {
            return new LookupContext
            {
                Branches = await _context.Branches.Where(x => x.CompanyId == companyId).Select(x => new LookupValue(x.Id, x.Name)).ToListAsync(),
                Departments = await _context.Departments.Where(x => x.CompanyId == companyId).Select(x => new LookupValue(x.Id, x.Name)).ToListAsync(),
                Sections = await _context.Sections.Where(x => x.CompanyId == companyId).Select(x => new LookupValue(x.Id, x.Name)).ToListAsync(),
                CostCenters = await _context.CostCenters.Where(x => x.CompanyId == companyId).Select(x => new LookupValue(x.Id, x.Name)).ToListAsync(),
                Divisions = await _context.Divisions.Where(x => x.CompanyId == companyId).Select(x => new LookupValue(x.Id, x.Name)).ToListAsync(),
                Projects = await _context.Projects.Where(x => x.CompanyId == companyId).Select(x => new LookupValue(x.Id, x.Name)).ToListAsync(),
                Phases = await _context.Phases.Where(x => x.CompanyId == companyId).Select(x => new LookupValue(x.Id, x.Name)).ToListAsync(),
                PieceworkUnitTypes = await _context.PieceworkUnitTypes.Where(x => x.CompanyId == companyId).Select(x => new LookupValue(x.Id, x.Name)).ToListAsync(),
                Activities = await _context.Activities.Select(x => new LookupValue(x.Id, x.Name)).ToListAsync(),
                Banks = await _context.Banks.Select(x => new LookupValue(x.Id, x.Name)).ToListAsync(),
                EmployeeObservations = new List<LookupValue>(),
                EmployeeTypes = await _context.EmployeeTypes.Select(x => new LookupValue(x.Id, x.Name)).ToListAsync(),
                IdentityDocumentTypes = await _context.IdentityDocumentTypes.Select(x => new LookupValue(x.Id, x.Name)).ToListAsync(),
                Positions = await _context.Positions.Select(x => new LookupValue(x.Id, x.Name)).ToListAsync(),
                Schedules = await _context.Schedules.Select(x => new LookupValue(x.Id, x.Name)).ToListAsync(),
                TypeOfWorkers = await _context.TypeOfWorkers.Select(x => new LookupValue(x.Id, x.Name)).ToListAsync(),
                Countries = await _context.Countries.Select(x => new LookupValue(x.Id, x.Name_es)).ToListAsync(),
                PaymentGroups = await _context.PaymentGroups.Select(x => new LookupValue(x.Id, x.Name)).ToListAsync(),
            };
        }

        private static Dictionary<string, int> BuildHeaderMap(IXLRow headerRow)
        {
            var headerMap = new Dictionary<string, int>();

            foreach (var cell in headerRow.CellsUsed())
            {
                var key = NormalizeKey(cell.GetString());
                if (!string.IsNullOrWhiteSpace(key))
                {
                    headerMap[key] = cell.Address.ColumnNumber;
                }
            }

            return headerMap;
        }

        private static void ValidateRequiredHeaders(Dictionary<string, int> headerMap)
        {
            var requiredHeaders = new[] { "CODIGO", "NOMBRES", "APELLIDOS", "CEDULA", "FECHAINGRESO", "TIPOSALARIO", "HORASREGULARES" };
            var missingHeaders = requiredHeaders.Where(x => !headerMap.ContainsKey(x)).ToList();
            if (missingHeaders.Any())
            {
                throw new InvalidDataException($"Faltan columnas requeridas en la hoja Empleados: {string.Join(", ", missingHeaders)}.");
            }
        }

        private static bool IsRowEmpty(IXLRow row, Dictionary<string, int> headerMap)
        {
            return headerMap.Values.All(column => string.IsNullOrWhiteSpace(row.Cell(column).GetFormattedString()));
        }

        private static string ReadString(IXLRow row, Dictionary<string, int> headerMap, string key)
        {
            if (!headerMap.TryGetValue(key, out var column))
            {
                return string.Empty;
            }

            return row.Cell(column).GetFormattedString().Trim();
        }

        private static DateTime? ReadDate(IXLRow row, Dictionary<string, int> headerMap, string key, List<string> errors, bool required, string label)
        {
            var raw = ReadString(row, headerMap, key);
            if (string.IsNullOrWhiteSpace(raw))
            {
                if (required)
                {
                    errors.Add($"{label} es obligatoria.");
                }

                return null;
            }

            if (headerMap.TryGetValue(key, out var column))
            {
                var cell = row.Cell(column);
                if (cell.DataType == XLDataType.DateTime)
                {
                    return cell.GetDateTime();
                }

                if (cell.DataType == XLDataType.Number && DateTime.TryParse(cell.Value.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, DateTimeStyles.None, out var numericDate))
                {
                    return numericDate;
                }
            }

            if (DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var parsed) ||
                DateTime.TryParse(raw, new CultureInfo("es-PA"), DateTimeStyles.AssumeLocal, out parsed))
            {
                return parsed.Date;
            }

            errors.Add($"{label} tiene un formato inválido.");
            return null;
        }

        private static decimal? ReadDecimal(IXLRow row, Dictionary<string, int> headerMap, string key, List<string> errors, bool required, string label)
        {
            var raw = ReadString(row, headerMap, key);
            if (string.IsNullOrWhiteSpace(raw))
            {
                if (required)
                {
                    errors.Add($"{label} es obligatorio.");
                }

                return null;
            }

            raw = raw.Replace(",", string.Empty);
            if (decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed) ||
                decimal.TryParse(raw, NumberStyles.Any, new CultureInfo("es-PA"), out parsed))
            {
                return parsed;
            }

            errors.Add($"{label} debe ser numérico.");
            return null;
        }

        private static bool ReadBool(IXLRow row, Dictionary<string, int> headerMap, string key, bool defaultValue, List<string> notes, string label)
        {
            var raw = ReadString(row, headerMap, key);
            if (string.IsNullOrWhiteSpace(raw))
            {
                notes.Add($"{label} tomó el valor por defecto {(defaultValue ? "Sí" : "No")}.");
                return defaultValue;
            }

            var normalized = NormalizeValue(raw);
            return normalized is "SI" or "SÍ" or "YES" or "TRUE" or "1";
        }

        private static string ResolveString(string rawValue, string defaultValue, string label, List<string> notes)
        {
            if (!string.IsNullOrWhiteSpace(rawValue))
            {
                return rawValue.Trim();
            }

            notes.Add($"{label} tomó el valor por defecto '{defaultValue}'.");
            return defaultValue;
        }

        private static int ResolveLookup(string rawValue, string label, IReadOnlyList<LookupValue> catalog, List<string> errors, List<string> notes, bool defaultToZeroWhenEmpty = false)
        {
            if (!catalog.Any())
            {
                if (!defaultToZeroWhenEmpty)
                {
                    errors.Add($"No hay valores configurados para {label}.");
                }

                return 0;
            }

            if (string.IsNullOrWhiteSpace(rawValue))
            {
                if (defaultToZeroWhenEmpty)
                {
                    notes.Add($"{label} quedó en 0 porque no se especificó un valor.");
                    return 0;
                }

                notes.Add($"{label} tomó el valor por defecto '{catalog[0].Name}'.");
                return catalog[0].Id;
            }

            if (int.TryParse(rawValue, out var numericId))
            {
                var itemById = catalog.FirstOrDefault(x => x.Id == numericId);
                if (itemById != null)
                {
                    return itemById.Id;
                }
            }

            var normalized = NormalizeValue(rawValue);
            var itemByName = catalog.FirstOrDefault(x => NormalizeValue(x.Name) == normalized);
            if (itemByName != null)
            {
                return itemByName.Id;
            }

            errors.Add($"{label} no coincide con un valor configurado ({rawValue}).");
            return 0;
        }

        private static int? ResolveLookupNullable(string rawValue, string label, IReadOnlyList<LookupValue> catalog, List<string> notes)
        {
            if (!catalog.Any() || string.IsNullOrWhiteSpace(rawValue))
            {
                return null;
            }

            if (int.TryParse(rawValue, out var numericId))
            {
                var itemById = catalog.FirstOrDefault(x => x.Id == numericId);
                return itemById?.Id;
            }

            var normalized = NormalizeValue(rawValue);
            var itemByName = catalog.FirstOrDefault(x => NormalizeValue(x.Name) == normalized);
            if (itemByName != null)
            {
                return itemByName.Id;
            }

            notes.Add($"{label} no se pudo resolver y se omitió.");
            return null;
        }

        private static string NormalizeKey(string value)
        {
            return new string(RemoveDiacritics(value ?? string.Empty)
                .ToUpperInvariant()
                .Where(char.IsLetterOrDigit)
                .ToArray());
        }

        private static string NormalizeValue(string value)
        {
            return RemoveDiacritics((value ?? string.Empty).Trim()).ToUpperInvariant();
        }

        private static string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        private sealed class LookupContext
        {
            public List<LookupValue> Branches { get; set; } = new List<LookupValue>();
            public List<LookupValue> Departments { get; set; } = new List<LookupValue>();
            public List<LookupValue> Sections { get; set; } = new List<LookupValue>();
            public List<LookupValue> CostCenters { get; set; } = new List<LookupValue>();
            public List<LookupValue> Divisions { get; set; } = new List<LookupValue>();
            public List<LookupValue> Projects { get; set; } = new List<LookupValue>();
            public List<LookupValue> Phases { get; set; } = new List<LookupValue>();
            public List<LookupValue> PieceworkUnitTypes { get; set; } = new List<LookupValue>();
            public List<LookupValue> Activities { get; set; } = new List<LookupValue>();
            public List<LookupValue> Banks { get; set; } = new List<LookupValue>();
            public List<LookupValue> EmployeeObservations { get; set; } = new List<LookupValue>();
            public List<LookupValue> EmployeeTypes { get; set; } = new List<LookupValue>();
            public List<LookupValue> IdentityDocumentTypes { get; set; } = new List<LookupValue>();
            public List<LookupValue> Positions { get; set; } = new List<LookupValue>();
            public List<LookupValue> Schedules { get; set; } = new List<LookupValue>();
            public List<LookupValue> TypeOfWorkers { get; set; } = new List<LookupValue>();
            public List<LookupValue> Countries { get; set; } = new List<LookupValue>();
            public List<LookupValue> PaymentGroups { get; set; } = new List<LookupValue>();
        }

        private sealed class LookupValue
        {
            public LookupValue(int id, string name)
            {
                Id = id;
                Name = name ?? string.Empty;
            }

            public int Id { get; }
            public string Name { get; }
        }
    }
}