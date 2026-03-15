# Overtime Calculator Web Service

This ASP.NET Core Web API calculates overtime factors for employees based on Panamanian labor laws. The service includes user authentication, historical tracking, and tardiness calculation.

## Authentication

The API uses JWT (JSON Web Tokens) for authentication. You must register and login to obtain a token for accessing protected endpoints.

### Register User
- **POST** `/api/auth/register`

```json
{
  "username": "your_username",
  "password": "your_password"
}
```

### Login
- **POST** `/api/auth/login`

```json
{
  "username": "your_username",
  "password": "your_password"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "your_username"
}
```

Use the token in subsequent requests by including it in the `Authorization` header:
```
Authorization: Bearer <your_token_here>
```

## Endpoints

### Calculate Overtime
- **POST** `/api/overtime/calculate` (Requires Authentication)

## Request Example

Include the JWT token in the Authorization header:

```
POST /api/overtime/calculate
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

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
  "Hora Entrada": "2023-10-01T07:55:00",
  "Hora Salida": "2023-10-01T21:00:00",
  "Periodo de Gracia Entrada Antes": 5,
  "Periodo de Gracia Entrada Después": 10,
  "Periodo de Gracia Salida Antes": 5,
  "Periodo de Gracia Salida Después": 10
}
```

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
  "Hora Entrada": "2023-10-01T07:55:00",
  "Hora Salida": "2023-10-01T21:00:00",
  "Periodo de Gracia Entrada Antes": 5,
  "Periodo de Gracia Entrada Después": 10,
  "Periodo de Gracia Salida Antes": 5,
  "Periodo de Gracia Salida Después": 10
}
```

**New Fields:**
- `Hora Entrada` and `Hora Salida`: Now accept full ISO date-time strings
- `Periodo de Gracia Entrada/Salida Antes/Después`: Grace periods in minutes for tardiness calculation

## Output

JSON object with calculation results:

```json
{
  "Id": 1,
  "Rangos": [
    {
      "Id": "Ordinaria",
      "Horas": 8.0,
      "Factor": 1.0
    },
    {
      "Id": "Extra Diurna",
      "Horas": 1.0,
      "Factor": 1.25
    },
    {
      "Id": "Extra Nocturna",
      "Horas": 2.0,
      "Factor": 1.5
    },
    {
      "Id": "Excedente",
      "Horas": 1.0,
      "Factor": 2.625
    }
  ],
  "FechaMarcacion": "2023-10-01T00:00:00",
  "Tardanza": 0.0
}
```

**New Fields:**
- `Id`: Unique identifier for the calculation record
- `FechaMarcacion`: Date of the time entry
- `Tardanza`: Tardiness in minutes (0 if on time)

## Features

- **Overtime Calculation**: Based on Panamanian labor laws with different factors for day types and schedule types
- **Tardiness Tracking**: Calculates late arrivals based on grace periods
- **Historical Storage**: All calculations are stored in SQL Server with user tracking
- **User Authentication**: JWT-based security for API access
- **Concurrent Processing**: Thread-safe for multiple simultaneous requests

## Running the Application

1. Ensure SQL Server is running and update the connection string in `appsettings.json` if needed.
2. Navigate to the project directory.
3. Run `dotnet run`.
4. The API will be available at `http://localhost:5031`.

Use tools like Postman or the included `.http` file to test the endpoints. Remember to authenticate first by registering and logging in to obtain a JWT token.