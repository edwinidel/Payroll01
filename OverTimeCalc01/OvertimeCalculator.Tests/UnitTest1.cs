using Xunit;
using OvertimeCalculator.Models;
using OvertimeCalculator.Services;

namespace OvertimeCalculator.Tests;

/// <summary>
/// Unit tests for OvertimeCalculatorService
/// </summary>
public class OvertimeCalculatorServiceTests
{
    private readonly OvertimeCalculatorService _service;

    public OvertimeCalculatorServiceTests()
    {
        _service = new OvertimeCalculatorService();
    }

    #region Regular Hours Tests

    [Fact]
    public void Calculate_RegularDay_StandardHours_ReturnsCorrectRegularHours()
    {
        // Arrange: Empleado trabaja 8 horas normales (8:00-17:00 con 60 min comida)
        var input = new EmployeeInput
        {
            Código = "EMP001",
            Nombre = "Juan",
            Apellido = "Pérez",
            Cédula = "1-123-456",
            SalarioPorHora = 5.0m,
            Compañía = 1,
            Sucursal = 1,
            Departamento = 1,
            CentroDeCosto = 1,
            Proyecto = 1,
            Fase = 1,
            Actividad = 1,
            TipoDeDía = "Regular",
            TipoDeHorario = "Diurno",
            TiempoComida = 60,
            IniciaHorario = "08:00",
            FinHorario = "17:00",
            HoraEntrada = new DateTime(2023, 10, 1, 8, 0, 0),
            HoraSalida = new DateTime(2023, 10, 1, 17, 0, 0),
            PeriodoGraciaEntradaAntes = 5,
            PeriodoGraciaEntradaDespues = 5,
            PeriodoGraciaSalidaAntes = 5,
            PeriodoGraciaSalidaDespues = 5
        };

        // Act
        var result = _service.Calculate(input);

        // Assert
        var regularRange = result.Rangos.FirstOrDefault(r => r.Id == "Ordinaria");
        Assert.NotNull(regularRange);
        Assert.Equal(8.0, regularRange!.Horas);
        Assert.Equal(1.0, regularRange.Factor);
    }

    [Fact]
    public void Calculate_WithTardiness_ReducesRegularHours()
    {
        // Arrange: Empleado llega 15 minutos tarde
        var input = new EmployeeInput
        {
            Código = "EMP002",
            Nombre = "María",
            Apellido = "García",
            Cédula = "2-234-567",
            SalarioPorHora = 4.5m,
            Compañía = 1,
            Sucursal = 1,
            Departamento = 1,
            CentroDeCosto = 1,
            Proyecto = 1,
            Fase = 1,
            Actividad = 1,
            TipoDeDía = "Regular",
            TipoDeHorario = "Diurno",
            TiempoComida = 60,
            IniciaHorario = "08:00",
            FinHorario = "17:00",
            HoraEntrada = new DateTime(2023, 10, 1, 8, 15, 0), // 15 minutos de tardanza
            HoraSalida = new DateTime(2023, 10, 1, 17, 0, 0),
            PeriodoGraciaEntradaAntes = 5,
            PeriodoGraciaEntradaDespues = 5,
            PeriodoGraciaSalidaAntes = 5,
            PeriodoGraciaSalidaDespues = 5
        };

        // Act
        var result = _service.Calculate(input);

        // Assert
        Assert.Equal(15.0, result.Tardanza); // 15 minutos
        var regularRange = result.Rangos.FirstOrDefault(r => r.Id == "Ordinaria");
        Assert.NotNull(regularRange);
        Assert.Equal(7.75, regularRange!.Horas); // 8 - 0.25 horas
    }

    #endregion

    #region Extra Hours Tests

    [Fact]
    public void Calculate_WithExtraHours_CalculatesDiurnalAndNocturnalExtras()
    {
        // Arrange: Trabaja extra (17:00-21:00)
        var input = new EmployeeInput
        {
            Código = "EMP003",
            Nombre = "Carlos",
            Apellido = "López",
            Cédula = "3-345-678",
            SalarioPorHora = 5.0m,
            Compañía = 1,
            Sucursal = 1,
            Departamento = 1,
            CentroDeCosto = 1,
            Proyecto = 1,
            Fase = 1,
            Actividad = 1,
            TipoDeDía = "Regular",
            TipoDeHorario = "Diurno",
            TiempoComida = 60,
            IniciaHorario = "08:00",
            FinHorario = "17:00",
            HoraEntrada = new DateTime(2023, 10, 1, 8, 0, 0),
            HoraSalida = new DateTime(2023, 10, 1, 21, 0, 0), // 4 horas extra
            PeriodoGraciaEntradaAntes = 5,
            PeriodoGraciaEntradaDespues = 5,
            PeriodoGraciaSalidaAntes = 5,
            PeriodoGraciaSalidaDespues = 5
        };

        // Act
        var result = _service.Calculate(input);

        // Assert
        var diurnalExtra = result.Rangos.FirstOrDefault(r => r.Id == "Extra Diurna");
        var nocturnalExtra = result.Rangos.FirstOrDefault(r => r.Id == "Extra Nocturna");
        
        Assert.NotNull(diurnalExtra);
        Assert.NotNull(nocturnalExtra);
        Assert.Equal(1.0, diurnalExtra!.Horas); // 17:00-18:00
        Assert.Equal(2.0, nocturnalExtra!.Horas); // 18:00-20:00 (solo 2 horas incluidas en los 3 límite)
    }

    [Fact]
    public void Calculate_ExtraHoursExceedingThreeHours_CalculatesExcess()
    {
        // Arrange: Más de 3 horas extra (debe crear "Excedente")
        var input = new EmployeeInput
        {
            Código = "EMP004",
            Nombre = "Ana",
            Apellido = "Martínez",
            Cédula = "4-456-789",
            SalarioPorHora = 5.0m,
            Compañía = 1,
            Sucursal = 1,
            Departamento = 1,
            CentroDeCosto = 1,
            Proyecto = 1,
            Fase = 1,
            Actividad = 1,
            TipoDeDía = "Regular",
            TipoDeHorario = "Diurno",
            TiempoComida = 60,
            IniciaHorario = "08:00",
            FinHorario = "17:00",
            HoraEntrada = new DateTime(2023, 10, 1, 8, 0, 0),
            HoraSalida = new DateTime(2023, 10, 1, 22, 0, 0), // 5 horas extra
            PeriodoGraciaEntradaAntes = 5,
            PeriodoGraciaEntradaDespues = 5,
            PeriodoGraciaSalidaAntes = 5,
            PeriodoGraciaSalidaDespues = 5
        };

        // Act
        var result = _service.Calculate(input);

        // Assert
        var excedente = result.Rangos.FirstOrDefault(r => r.Id == "Excedente");
        Assert.NotNull(excedente);
        Assert.Equal(2.0, excedente!.Horas); // 2 horas excedentes (después de 3 horas de extras)
    }

    #endregion

    #region Special Days Tests

    [Fact]
    public void Calculate_SundayWorking_AppliesCorrectFactor()
    {
        // Arrange: Domingo (factor 1.5)
        var input = new EmployeeInput
        {
            Código = "EMP005",
            Nombre = "Pedro",
            Apellido = "Rodríguez",
            Cédula = "5-567-890",
            SalarioPorHora = 5.0m,
            Compañía = 1,
            Sucursal = 1,
            Departamento = 1,
            CentroDeCosto = 1,
            Proyecto = 1,
            Fase = 1,
            Actividad = 1,
            TipoDeDía = "Domingo",
            TipoDeHorario = "Diurno",
            TiempoComida = 60,
            IniciaHorario = "08:00",
            FinHorario = "17:00",
            HoraEntrada = new DateTime(2023, 10, 1, 8, 0, 0),
            HoraSalida = new DateTime(2023, 10, 1, 17, 0, 0),
            PeriodoGraciaEntradaAntes = 5,
            PeriodoGraciaEntradaDespues = 5,
            PeriodoGraciaSalidaAntes = 5,
            PeriodoGraciaSalidaDespues = 5
        };

        // Act
        var result = _service.Calculate(input);

        // Assert
        var regularRange = result.Rangos.FirstOrDefault(r => r.Id == "Ordinaria");
        Assert.NotNull(regularRange);
        Assert.Equal(1.5, regularRange!.Factor); // Factor multiplicado por día domingo
    }

    [Fact]
    public void Calculate_HolidayWorking_AppliesCorrectFactor()
    {
        // Arrange: Fiesta (factor 2.5)
        var input = new EmployeeInput
        {
            Código = "EMP006",
            Nombre = "Laura",
            Apellido = "Sánchez",
            Cédula = "6-678-901",
            SalarioPorHora = 5.0m,
            Compañía = 1,
            Sucursal = 1,
            Departamento = 1,
            CentroDeCosto = 1,
            Proyecto = 1,
            Fase = 1,
            Actividad = 1,
            TipoDeDía = "Fiesta",
            TipoDeHorario = "Diurno",
            TiempoComida = 60,
            IniciaHorario = "08:00",
            FinHorario = "17:00",
            HoraEntrada = new DateTime(2023, 10, 1, 8, 0, 0),
            HoraSalida = new DateTime(2023, 10, 1, 17, 0, 0),
            PeriodoGraciaEntradaAntes = 5,
            PeriodoGraciaEntradaDespues = 5,
            PeriodoGraciaSalidaAntes = 5,
            PeriodoGraciaSalidaDespues = 5
        };

        // Act
        var result = _service.Calculate(input);

        // Assert
        var regularRange = result.Rangos.FirstOrDefault(r => r.Id == "Ordinaria");
        Assert.NotNull(regularRange);
        Assert.Equal(2.5, regularRange!.Factor); // Factor multiplicado por fiesta
    }

    #endregion

    #region Edge Cases Tests

    [Fact]
    public void Calculate_NoExtraHours_ReturnsOnlyRegularRange()
    {
        // Arrange: Exactamente 8 horas
        var input = new EmployeeInput
        {
            Código = "EMP007",
            Nombre = "Diego",
            Apellido = "Díaz",
            Cédula = "7-789-012",
            SalarioPorHora = 5.0m,
            Compañía = 1,
            Sucursal = 1,
            Departamento = 1,
            CentroDeCosto = 1,
            Proyecto = 1,
            Fase = 1,
            Actividad = 1,
            TipoDeDía = "Regular",
            TipoDeHorario = "Diurno",
            TiempoComida = 60,
            IniciaHorario = "08:00",
            FinHorario = "17:00",
            HoraEntrada = new DateTime(2023, 10, 1, 8, 0, 0),
            HoraSalida = new DateTime(2023, 10, 1, 17, 0, 0),
            PeriodoGraciaEntradaAntes = 5,
            PeriodoGraciaEntradaDespues = 5,
            PeriodoGraciaSalidaAntes = 5,
            PeriodoGraciaSalidaDespues = 5
        };

        // Act
        var result = _service.Calculate(input);

        // Assert
        Assert.Single(result.Rangos);
        Assert.Equal("Ordinaria", result.Rangos[0].Id);
    }

    #endregion
}

