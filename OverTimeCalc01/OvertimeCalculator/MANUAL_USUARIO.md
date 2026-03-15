# Manual de Usuario - Calculador de Sobretiempo

Servicio Web ASP.NET Core que calcula sobretiempo según la ley laboral panameña. A continuación se detallan los pasos de instalación, ejecución y uso del endpoint de cálculo con ejemplos de JSON listos para enviar.

## Requisitos Previos

- .NET 8.0 SDK instalado (https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server accesible; ajustar la cadena `DefaultConnection` en `appsettings.json` si aplica
- Cliente HTTP (Postman, curl o el archivo `OvertimeCalculator.http` de la solución)

## Preparación del Proyecto

1) Restaurar dependencias
```
dotnet restore
```
2) Aplicar migraciones y crear/actualizar la base de datos
```
dotnet ef database update
```
3) Compilar
```
dotnet build
```
4) Ejecutar el servicio (por defecto expone http://localhost:5031)
```
dotnet run
```

## Flujo Completo para Calcular Sobretiempo

1) Registrar usuario (solo una vez)
- POST http://localhost:5031/api/auth/register
- Body ejemplo:
```json
{
  "username": "operador",
  "password": "P4ssw0rd!"
}
```

2) Iniciar sesión y obtener token JWT
- POST http://localhost:5031/api/auth/login
- Body ejemplo:
```json
{
  "username": "operador",
  "password": "P4ssw0rd!"
}
```
- Copie el valor de `token` de la respuesta y úselo en `Authorization: Bearer <token>`.

3) Preparar el payload del cálculo
- Endpoint: POST http://localhost:5031/api/v1.0/overtime/calculate (requiere JWT)
- Content-Type: application/json
- Campos obligatorios: identificación del empleado, salario, compañía/sucursal/departamento/centro de costo/proyecto/fase/actividad, `Tipo de Día`, `Tipo de Horario`, `Tiempo Comida`, `Inicia Horario`, `Fin Horario`, periodos de gracia, y al menos un par Entrada/Salida (ya sea con `Hora Entrada`/`Hora Salida` o dentro de `Marcaciones`).
- Periodos de gracia (todos requeridos, en minutos):
  - `Periodo de Gracia Entrada Antes`
  - `Periodo de Gracia Entrada Después`
  - `Periodo de Gracia Salida Antes`
  - `Periodo de Gracia Salida Después`
- Horas pueden ir en ISO 8601 (`2026-01-14T07:55:00`) o HH:mm cuando corresponda.

4) Enviar el cálculo con un cliente HTTP
- Incluya el header `Authorization: Bearer <token>` y el JSON según uno de los ejemplos siguientes.

## JSON de Entrada (Ejemplos)

### A) Formato tradicional (una entrada y una salida)
```json
{
  "Código": "DEL001",
  "Nombre": "Edwin",
  "Apellido": "Delgado",
  "Cédula": "7-94-199",
  "Salario por Hora": 3.75,
  "Compañía": 1,
  "Sucursal": 1,
  "Departamento": 1,
  "Centro de Costo": 1,
  "Proyecto": 1,
  "Fase": 1,
  "Actividad": 1,
  "Tipo de Día": "Regular",
  "Tipo de Horario": "Diurno",
  "Tiempo Comida": 60,
  "Inicia Horario": "08:00",
  "Fin Horario": "17:00",
  "Hora Entrada": "2026-01-14T07:55:00",
  "Hora Salida": "2026-01-14T21:00:00",
  "Periodo de Gracia Entrada Antes": 5,
  "Periodo de Gracia Entrada Después": 10,
  "Periodo de Gracia Salida Antes": 5,
  "Periodo de Gracia Salida Después": 10
}
```

### B) Formato con múltiples marcaciones
```json
{
  "Código": "DEL001",
  "Nombre": "Edwin",
  "Apellido": "Delgado",
  "Cédula": "7-94-199",
  "Salario por Hora": 3.75,
  "Compañía": 1,
  "Sucursal": 1,
  "Departamento": 1,
  "Centro de Costo": 1,
  "Proyecto": 1,
  "Fase": 1,
  "Actividad": 1,
  "Tipo de Día": "Regular",
  "Tipo de Horario": "Diurno",
  "Tiempo Comida": 60,
  "Inicia Horario": "08:00",
  "Fin Horario": "17:00",
  "Marcaciones": [
    {
      "Id": "Entrada",
      "Hora": "2026-01-14T07:55:00"
    },
    {
      "Id": "Salida",
      "Hora": "2026-01-14T21:00:00"
    }
  ],
  "Periodo de Gracia Entrada Antes": 5,
  "Periodo de Gracia Entrada Después": 10,
  "Periodo de Gracia Salida Antes": 5,
  "Periodo de Gracia Salida Después": 10
}
```

Notas sobre campos:
- `Tipo de Día`: Regular, Domingo, Fiesta o Duelo Nacional.
- `Tipo de Horario`: Diurno, Mixto o Nocturno.
- `Tiempo Comida`: minutos de descanso.
- Si se usa `Marcaciones`, debe incluir al menos `Entrada` y `Salida`; de lo contrario use `Hora Entrada` y `Hora Salida`.

## Respuesta de la API

Ejemplo de respuesta exitosa:
```json
{
  "Id": 42,
  "Rangos": [
    { "Id": "Ordinaria", "Horas": 8.0, "Factor": 1.0 },
    { "Id": "Extra Diurna", "Horas": 1.0, "Factor": 1.25 },
    { "Id": "Extra Nocturna", "Horas": 2.0, "Factor": 1.5 },
    { "Id": "Excedente", "Horas": 1.0, "Factor": 2.625 }
  ],
  "FechaMarcacion": "2026-01-14T00:00:00",
  "Tardanza": 0.0,
  "Mensaje": "Se desactivó un cálculo anterior (entrada: 07:55, salida: 17:00) porque se traslapaba con el nuevo cálculo."
}
```
- `Id`: identificador del cálculo guardado en histórico.
- `Rangos`: detalle de horas ordinarias y extras con su factor multiplicador.
- `FechaMarcacion`: fecha base del cálculo.
- `Tardanza`: minutos de tardanza considerando periodos de gracia.
- `Mensaje`: se incluye cuando se desactiva un cálculo previo por traslape.

## Pruebas Rápidas

- Visual Studio Code: abra `OvertimeCalculator.http`, actualice el token en `Authorization` y use "Send Request".
- Postman/curl: envíe cualquiera de los JSON anteriores con el header `Authorization: Bearer <token>`.

## Solución de Problemas

- 400 Bad Request: faltan campos requeridos o formatos inválidos.
- 401 Unauthorized: token ausente, inválido o expirado; vuelva a iniciar sesión.
- 403 Forbidden: usuario sin permisos (no hay roles configurados por defecto).
- 500 Error interno: revise la consola y los logs en `logs/`.
- Sin respuesta: confirme que el servicio esté en el puerto correcto y que la base de datos sea accesible.

## Referencias

- Controlador del endpoint de cálculo: [Controllers/OvertimeController.cs](Controllers/OvertimeController.cs)
- Modelo de entrada: [Models/EmployeeInput.cs](Models/EmployeeInput.cs)
- Modelo de salida: [Models/CalculationResult.cs](Models/CalculationResult.cs)