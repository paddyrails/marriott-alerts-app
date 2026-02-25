# Marriott Alerts API — .NET 8 Backend

A production-ready .NET 8 Web API with JWT authentication, AlertDef CRUD, EF Core + Postgres, and Docker support.

## Quick Start

```bash
# Start API + Postgres
docker-compose up --build

# API available at http://localhost:8080
# Swagger UI at http://localhost:8080/swagger
```

## Project Structure

```
backend/
├── src/
│   ├── Api/            # Controllers, middleware, security, entry point
│   ├── Application/    # Business logic, DTOs, validators
│   ├── Domain/         # Entity models
│   └── Infrastructure/ # EF Core, repositories, persistence
├── tests/
│   └── Api.IntegrationTests/  # xUnit + Testcontainers
├── Dockerfile
├── docker-compose.yml
└── Backend.sln
```

## API Endpoints

| Method | Endpoint               | Auth     | Description           |
|--------|------------------------|----------|-----------------------|
| GET    | /health                | No       | Health check          |
| POST   | /api/auth/register     | No       | Register new user     |
| POST   | /api/auth/login        | No       | Login, get JWT        |
| POST   | /api/AlertDefs         | Bearer   | Create alert def      |
| GET    | /api/AlertDefs         | Bearer   | List alert defs       |
| GET    | /api/AlertDefs/{id}    | Bearer   | Get alert def by ID   |
| PATCH  | /api/AlertDefs/{id}    | Bearer   | Update alert def      |
| DELETE | /api/AlertDefs/{id}    | Bearer   | Delete alert def      |

## Development

### Prerequisites
- .NET 8 SDK
- Docker & Docker Compose
- (For tests) Docker running locally

### Run Locally Without Docker

```bash
# Start Postgres (or use docker-compose up db)
docker-compose up db -d

# Run the API
cd src/Api
dotnet run
```

### Run Tests

```bash
# Requires Docker running (Testcontainers spins up Postgres)
dotnet test
```

### EF Core Migrations

```bash
cd src/Api
dotnet ef migrations add <MigrationName> --project ../Infrastructure/Infrastructure.csproj
dotnet ef database update --project ../Infrastructure/Infrastructure.csproj
```

## Configuration

Environment variables override `appsettings.json`. See `.env.example` for all options.

| Variable                      | Default                           |
|-------------------------------|-----------------------------------|
| `ConnectionStrings__Default`  | Host=db;Port=5432;...             |
| `Jwt__Secret`                 | change_me_change_me_change_me_32  |
| `Jwt__ExpiresInMinutes`       | 15                                |

## Demo User (Development Only)

On first startup in Development, a seed user is created:
- **Email:** demo@example.com
- **Password:** password123
