# Manual del Desarrollador - OvertimeCalculator

## 1. Visión general
- API ASP.NET Core con versionamiento por segmento de URL: `api/v{version}/...` (versión vigente: `v1`).
- Autenticación JWT Bearer: obtén el token con `/api/v1/Auth/login` y envíalo en `Authorization: Bearer <token>`.
- Swagger disponible en desarrollo: `https://<host>/swagger`.

## 2. Conexión y configuración
- Base URL (ejemplo local con HTTPS): `https://localhost:5001/api/v1/` (ajusta host/puerto según despliegue).
- Encabezados comunes: `Content-Type: application/json` y, salvo en `register`/`login`, `Authorization: Bearer <token>`.
- Cadena de conexión SQL Server (appsettings.json): `Server=192.168.0.15,1433;Database=OTC;User Id=sa;Password=Yanaris1624*;MultipleActiveResultSets=True;TrustServerCertificate=True;Encrypt=True;Connection Timeout=30`.
  - Cámbiala vía variables de entorno o `appsettings.*.json` antes de desplegar.
- JWT (appsettings.json): `Jwt:Issuer=OvertimeCalculator`, `Jwt:Audience=OvertimeCalculatorUsers`, clave `Jwt:Key` debe mantenerse secreta.

## 3. Endpoints

### 3.1 Autenticación (públicos)
**POST** `/api/v1/Auth/register`
- Cuerpo:
```json
{
  "username": "demo",
  "password": "secret123",
  "confirmPassword": "secret123"
}
```
- Respuestas: `200 OK` texto plano "User registered successfully"; `400 BadRequest` si el usuario existe o validación falla.

**POST** `/api/v1/Auth/login`
- Cuerpo:
```json
{
  "username": "demo",
  "password": "secret123"
}
```
- Respuesta `200 OK`:
```json
{
  "token": "<jwt>",
  "username": "demo",
  "userId": 1,
  "expiresAt": "2026-03-10T12:00:00Z"
}
```
- Errores: `401 Unauthorized` credenciales inválidas.

### 3.2 Cálculo de sobretiempo (requiere JWT)
**POST** `/api/v1/Overtime/calculate`
- Encabezado: `Authorization: Bearer <jwt>`.
- Cuerpo mínimo de ejemplo:
```json
{
  "Código": "EMP-001",
  "Nombre": "Ana",
  "Apellido": "Pérez",
  "Cédula": "001-0000000-0",
  "Salario por Hora": 120.5,
  "Compañía": 1,
  "Sucursal": 1,
  "Departamento": 1,
  "Centro de Costo": 10,
  "Proyecto": 20,
  "Fase": 1,
  "Actividad": 1,
  "Tipo de Día": "Regular", // valores válidos según validador
  "Tipo de Horario": "Diurno", // Diurno | Mixto | Nocturno
  "Tiempo Comida": 60,
  "Inicia Horario": "08:00",
  "Fin Horario": "17:00",
  "Marcaciones": [
    { "Id": "Entrada", "Hora": "2026-03-10T08:02:00" },
    { "Id": "Salida",  "Hora": "2026-03-10T18:15:00" }
  ],
  "Periodo de Gracia Entrada Antes": 5,
  "Periodo de Gracia Entrada Después": 5,
  "Periodo de Gracia Salida Antes": 5,
  "Periodo de Gracia Salida Después": 5
}
```
- Alternativa: si no envías `Marcaciones`, debes enviar `Hora Entrada` y `Hora Salida` (`DateTime`).
- Respuesta `200 OK`:
```json
{
  "id": 123,                // Id del cálculo guardado
  "rangos": [
    { "id": "Ordinaria", "horas": 8, "factor": 1 },
    { "id": "ExtraDiurna", "horas": 1.25, "factor": 1.35 }
  ],
  "fechaMarcacion": "2026-03-10T00:00:00",
  "tardanza": 2.0,
  "mensaje": "..." // opcional, p.ej. aviso de traslape
}
```
- Validaciones clave: ModelState completo, tiempos en formato `HH:mm` para horario, periodos de gracia 0-60, salario 0.01-10000. Si un cálculo se traslapa con uno previo para el mismo empleado/fecha, el anterior se marca inactivo y se devuelve un mensaje.
- Errores: `400 BadRequest` con detalles de validación o mensaje genérico; `401 Unauthorized` si falta/expira el token.

### 3.3 Weather (demo)
**GET** `/weatherforecast`
- Sin autenticación. Endpoint de ejemplo generado por plantilla.

## 4. Flujo típico de consumo
1) Registrar usuario (una sola vez). 2) Login y guardar `token`. 3) Consumir `/Overtime/calculate` con el token. 4) Revisar `id` y `mensaje` para auditoría o traslapes.

## 5. Notas operativas
- Versionamiento: usa `v1` en rutas; `v2` está marcado como obsoleto.
- Logs: Serilog escribe en consola y en `logs/app-<fecha>.txt`.
- Swagger/OpenAPI: incluye esquema de seguridad Bearer; activa el lock en la UI y pega el token completo.
- Errores JWT: la API retorna JSON explicando causa (expirado, firma inválida, emisor/audiencia incorrectos).
- Persistencia: cada cálculo guarda cabecera/detalles de marcación y resultado serializado para trazabilidad.
