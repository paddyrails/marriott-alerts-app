# Backend Build Instructions (Cursor IDE) — .NET 8 Web API

## Goal
Generate a production-ready **.NET 8** backend API with:
- JWT authentication (register/login)
- CRUD endpoints (sample domain: AlertDefs)
- Validation + consistent error responses
- Clean layered architecture (Controllers → Services → Repositories)
- Entity Framework Core + Postgres
- Migrations + seed data
- Swagger/OpenAPI
- Logging + request correlation ID
- Tests (unit + integration)
- Docker + docker-compose

---

## Target Stack
- .NET: 8 (ASP.NET Core Web API)
- Language: C#
- DB: Postgres
- ORM: EF Core (Npgsql provider)
- Auth: ASP.NET Core Authentication + JWT Bearer
- Validation: DataAnnotations (minimum) + FluentValidation (preferred)
- Docs: Swagger (Swashbuckle)
- Mapping: AutoMapper (optional)
- Tests: xUnit + FluentAssertions + WebApplicationFactory + Testcontainers (preferred)
- Lint/format: dotnet format (optional)

---

## What Cursor Should Produce
A backend project that:
1. Runs locally with one command (docker-compose)
2. Exposes REST APIs with Swagger UI
3. Implements register/login and protected endpoints
4. Implements AlertDef CRUD scoped to authenticated user
5. Uses EF Core migrations and optional seeding
6. Has global exception handling with a standard error schema
7. Has integration tests against Postgres (Testcontainers or docker)
8. Uses environment-based configuration with secure defaults

---

## Required Project Structure
Create the following structure:


backend/
src/
Api/
Api.csproj
Program.cs
appsettings.json
appsettings.Development.json

  Controllers/
    HealthController.cs
    AuthController.cs
    AlertDefController.cs

  Middleware/
    CorrelationIdMiddleware.cs
    ExceptionHandlingMiddleware.cs

  Security/
    JwtOptions.cs
    JwtTokenService.cs
    CurrentUserService.cs

  Common/
    ApiError.cs
    ApiResponse.cs
    ErrorCodes.cs

Application/
  Application.csproj
  Auth/
    AuthService.cs
    Dtos/
      RegisterRequest.cs
      LoginRequest.cs
      AuthResponse.cs
  AlertDefs/
    AlertDefService.cs
    Dtos/
      AlertDefCreateRequest.cs
      AlertDefUpdateRequest.cs
      AlertDefResponse.cs
  Validation/
    RegisterRequestValidator.cs
    LoginRequestValidator.cs
    AlertDefCreateRequestValidator.cs
    AlertDefUpdateRequestValidator.cs

Domain/
  Domain.csproj
  Entities/
    User.cs
    AlertDef.cs

Infrastructure/
  Infrastructure.csproj
  Persistence/
    AppDbContext.cs
    Migrations/ (generated)
    DbSeeder.cs
  Repositories/
    IUserRepository.cs
    IAlertDefRepository.cs
    UserRepository.cs
    AlertDefRepository.cs

tests/
Api.IntegrationTests/
Api.IntegrationTests.csproj
AuthTests.cs
AlertDefTests.cs
TestWebAppFactory.cs

Dockerfile
docker-compose.yml
.env.example
README.md
Backend.sln


> Notes:
> - Keep **Api** thin (routing/controllers/middleware).
> - Put business logic in **Application**.
> - Put EF Core + repos in **Infrastructure**.
> - Put entities in **Domain**.

---

## API Requirements

### Health
- `GET /health`
Response:
```json
{ "status": "ok" }
Auth

POST /api/auth/register
Body:

{ "email": "string", "password": "string", "name": "string (optional)" }

POST /api/auth/login
Body:

{ "email": "string", "password": "string" }

Response:

{
  "accessToken": "string",
  "user": { "id": "uuid", "email": "string", "name": "string|null" }
}

Rules:

Password min length: 8

Hash passwords using BCrypt (BCrypt.Net-Next)

Email unique constraint

AlertDefs (Protected)

All require header:
Authorization: Bearer <token>

POST /api/AlertDefs
Body:

{ "name": "string", "awsAccountId": "string", "maxBillAmount": "integer", "alertRecipientEmails": "string" }

GET /api/AlertDefs?page=&limit=
Response:

{ "items": [AlertDef], "page": 1, "limit": 20, "total": 0 }

GET /api/AlertDefs/{id} → AlertDef

PATCH /api/AlertDefs/{id}
Body:

{ "name": "string", "awsAccountId": "string", "maxBillAmount": "integer", "alertRecipientEmails": "string" }

DELETE /api/AlertDefs/{id} → 204

AlertDef model:

{
  "id": "uuid",
  "name": "string",
  "awsAccountId": "string",
  "maxBillAmount": "integer",
  "alertRecipientEmails": "string",
  "createdAt": "ISO-8601",
  "updatedAt": "ISO-8601"
}

---

Domain Model (Entities)
User

Id: Guid

Email: string (unique)

PasswordHash: string

Name: string?

CreatedAt, UpdatedAt: DateTimeOffset

Navigation: AlertDefs

AlertDef

Id: Guid

Name: string

AWSAccountId: string

MaxBillAmount: integer

AlertRecipientEmails: string

CreatedAt, UpdatedAt: DateTimeOffset

Persistence Requirements (EF Core + Postgres)

Use Npgsql.EntityFrameworkCore.Postgres

Use Fluent API to enforce:

users.email unique index

AlertDef.user_id FK

index on AlertDefs.user_id

Use DateTimeOffset for timestamps

Add EF Core migrations

Disable EnsureCreated; always use migrations

Optional seed:

DbSeeder creates 1 demo user + a few AlertDefs (only in Development)

Security Requirements (JWT)

Implement:

JwtOptions bound from config:

Issuer, Audience, Secret, ExpiresInMinutes

JwtTokenService to generate tokens with:

sub = user id

email claim

Configure JWT Bearer authentication in Program.cs

Configure authorization for protected endpoints

Validation Requirements

Preferred: FluentValidation

Register/Login validation:

email required + valid format

password required + min 8

AlertDef validation:

title required, max 200

If FluentValidation is not used, implement with DataAnnotations at minimum.

Error Handling (Standard Error Shape)

All errors must return:

{
  "error": {
    "code": "string",
    "message": "string",
    "details": {}
  }
}

Use ExceptionHandlingMiddleware to handle:

Validation failures → 400 (code: validation_error)

Not found → 404 (code: not_found)

Unauthorized → 401 (code: unauthorized)

Forbidden/ownership → 403 (code: forbidden)

Duplicate email → 409 (code: conflict)

Unhandled → 500 (code: internal_error)

Logging + Correlation ID

Add middleware:

If X-Correlation-Id header exists, reuse it

Else generate a GUID and return it as response header

Include correlation id in logs scope

Use built-in ILogger (Serilog optional)

Swagger/OpenAPI

Enable Swagger in Development

Add JWT Bearer security scheme

Swagger UI should allow authorizing with Bearer token

Configuration
.env.example
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:8080
ConnectionStrings__Default=Host=db;Port=5432;Database=app;Username=Postgres;Password=Postgres
Jwt__Issuer=app
Jwt__Audience=app
Jwt__Secret=change_me_change_me_change_me
Jwt__ExpiresInMinutes=15
appsettings.json

Bind JWT options and connection string.
Use environment overrides.

Docker Requirements
Dockerfile

Multi-stage build

Build and publish Api project

Run on port 8080

docker-compose.yml

Services:

db: Postgres

api: builds Dockerfile
api depends_on db

Expose:

API: 8080

Include a healthcheck for Postgres (preferred).

Testing Requirements
Integration Tests (preferred)

Use WebApplicationFactory + Testcontainers.Postgres:

Register + login returns accessToken

POST /api/AlertDefs without token → 401

Create a AlertDef with token → 201/200 and returns AlertDef

User A cannot read/update/delete User B’s AlertDef → 403/404 (choose one and keep consistent)

Unit Tests (optional)

JwtTokenService creates token with expected claims

AlertDefService enforces ownership

Cursor Execution Steps

When generating code:

Create the solution and projects:

Api, Application, Domain, Infrastructure

Wire DI in Api:

DbContext

Repositories

Services

CurrentUserService

Implement Auth flow (BCrypt + JWT)

Implement AlertDef CRUD with ownership checks

Add migrations and ensure startup applies migrations automatically in Development (optional)

Add middleware (exception + correlation id)

Add Swagger with JWT auth

Add docker-compose and verify app starts

Add tests and ensure they pass

Acceptance Checklist

 docker-compose up starts API + Postgres

 GET /health returns ok

 Swagger UI loads and supports Bearer auth

 Register/login works and returns JWT

 AlertDefs CRUD works and is user-scoped

 Validation and errors follow standard error shape

 EF Core migrations apply cleanly

 Integration tests pass

Notes

Never return password hashes in responses.

Never log JWT secrets or tokens.

Use async/await everywhere.

Avoid dynamic and minimize exceptions leaking raw messages.