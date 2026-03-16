# Payroll01 Monorepo

Monorepo para la solución de nómina y cálculo de horas extras.

## Estructura

- NominaH01/
  - Aplicación principal de nómina `2FA` en ASP.NET Core MVC.
  - Incluye lógica de planillas, borradores, descuentos legales, acreedores, comprobantes y portal asociado.
- OverTimeCalc01/
  - API `OvertimeCalculator` en ASP.NET Core para cálculo de sobretiempo.
  - Usada por la aplicación principal para registrar y recalcular horas extras.
- PayrollFlow.sln
  - Solución raíz para trabajar ambos proyectos desde un solo repositorio.

## Proyectos principales

### Nómina
- Ruta: `NominaH01/2FA`
- Tipo: ASP.NET Core MVC
- Archivo: `NominaH01/2FA/2FA.csproj`

### API de horas extras
- Ruta: `OverTimeCalc01/OvertimeCalculator`
- Tipo: ASP.NET Core Web API
- Archivo: `OverTimeCalc01/OvertimeCalculator/OvertimeCalculator.csproj`

## Requisitos

- .NET SDK compatible con los proyectos
- SQL Server configurado para cada aplicación
- Archivos de configuración local no incluidos en Git, por ejemplo:
  - `appsettings.Development.json`

## Ejecución local

### Ambos proyectos a la vez
Desde la raíz del repositorio:

- `./scripts/run-payroll-stack.sh`

Modo watch opcional:

- `./scripts/run-payroll-stack.sh --watch`

### API de horas extras
Desde la raíz del repositorio:

- `dotnet run --project OverTimeCalc01/OvertimeCalculator/OvertimeCalculator.csproj`

### Aplicación de nómina
Desde la raíz del repositorio:

- `dotnet run --project NominaH01/2FA/2FA.csproj`

## Build

Desde la raíz del repositorio:

- `dotnet build PayrollFlow.sln`

O por proyecto:

- `dotnet build NominaH01/2FA/2FA.csproj`
- `dotnet build OverTimeCalc01/OvertimeCalculator/OvertimeCalculator.csproj`

## Notas de repositorio

- Este repositorio fue inicializado como un monorepo nuevo.
- `dotnet run` ya no funciona desde la raíz por sí solo, porque la raíz contiene una solución y no un proyecto ejecutable único.
- Los repositorios Git originales de los proyectos fueron preservados fuera de esta copia.
- Los archivos sensibles y de ambiente local están ignorados en `.gitignore`.

## Documentación adicional

- API de horas extras: `OverTimeCalc01/OvertimeCalculator/README.md`
- Documentación funcional y técnica adicional: carpeta `NominaH01/docs/`
