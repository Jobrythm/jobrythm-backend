# Jobrythm Backend

Professional Job Management SaaS for small businesses and tradespeople.

## Prerequisites
- .NET 10 SDK
- Docker Desktop
- PostgreSQL (if running locally)

## Quick Start (Docker)
1. Ensure Docker is running.
2. Run `docker compose up -d --build` from the root directory.
3. API will be available at [http://localhost:8080](http://localhost:8080).
4. pgAdmin will be available at [http://localhost:5050](http://localhost:5050) (Login: admin@jobrythm.com / admin).
5. Frontend integration notes and endpoint usage live in [`api-endpoints.txt`](./api-endpoints.txt).

## Database Migrations
When running via Docker, migrations are applied automatically on startup in Development environment.
The development seed creates a default admin user:
- Email: `admin@example.com`
- Password: `adminpassword`
To apply manually:
```bash
dotnet ef database update --project src/Jobrythm.Infrastructure --startup-project src/Jobrythm.Api
```

## Running Tests
Run all tests in the solution:
```bash
dotnet test
```

## Environment Variables (Production)
For production deployment, ensure the following secrets are set:
- `JwtSettings__Key`
- `ConnectionStrings__DefaultConnection`
- `Stripe__ApiKey`
- `Stripe__WebhookSecret`
- `Resend__ApiKey`

## Project Structure
- `src/Jobrythm.Domain`: Core entities and business logic.
- `src/Jobrythm.Application`: MediatR commands, queries, and business use cases.
- `src/Jobrythm.Infrastructure`: Persistence (EF Core), External Services (Stripe, Resend, QuestPDF).
- `src/Jobrythm.Api`: RESTful API, Controllers, and Middleware.
- `tests/`: xUnit test projects for Domain and Application layers.
